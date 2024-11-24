using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TextVariationHandler
{
    public class Section
    {
        public enum VaryType
        {
            Sequence,
            Cycle,
            Once,
            Random,
            Conditional
        }

        public VaryType type = VaryType.Sequence;
        public string entire = string.Empty;
        public List<string> elements = new List<string>();

        // Conditional properties
        public string variableName = string.Empty;
        public string compareValue = string.Empty;
        public string trueResult = string.Empty;
        public string falseResult = string.Empty;

        public string Select(ref int index)
        {
            if (type == VaryType.Conditional)
            {
                // Remove the '=' prefix from variable name before finding it
                string cleanVarName = variableName.TrimStart('=');

                var engine = GameObject.FindObjectsOfType<BasicFlowEngine>().ToList().Where(x => !x.gameObject.name.Contains("GlobalVariablesEngine")).FirstOrDefault();

                var variable = engine?.GetVariable(cleanVarName);

                return variable != null && variable.Evaluate(ComparisonOperator.Equals, compareValue)
                    ? trueResult
                    : falseResult;
            }

            switch (type)
            {
                case VaryType.Sequence:
                    index = Mathf.Min(index, elements.Count - 1);
                    break;
                case VaryType.Cycle:
                    index = index % elements.Count;
                    break;
                case VaryType.Once:
                    index = Mathf.Min(index, elements.Count);
                    break;
                case VaryType.Random:
                    index = UnityEngine.Random.Range(0, elements.Count);
                    break;
            }

            if (index >= 0 && index < elements.Count)
            {
                return elements[index];
            }
            return string.Empty;
        }
    }

    static Dictionary<int, int> hashedSections = new Dictionary<int, int>();

    public static void ClearHistory()
    {
        hashedSections.Clear();
    }

    public static bool TokeniseVarySections(string input, List<Section> varyingSections)
    {
        varyingSections.Clear();
        int currentDepth = 0;
        int curStartIndex = 0;
        int curPipeIndex = 0;
        Section curSection = null;

        for (int i = 0; i < input.Length; i++)
        {
            if (i < input.Length - 1 && input[i] == '[' && input[i + 1] == '*')
            {
                // Handle conditional statement
                if (ParseConditional(input, i, out Section conditionalSection))
                {
                    varyingSections.Add(conditionalSection);
                    i += conditionalSection.entire.Length - 1; // Skip to end of conditional
                    continue;
                }
            }

            switch (input[i])
            {
                case '[':
                    if (currentDepth == 0)
                    {
                        curSection = new Section();
                        varyingSections.Add(curSection);

                        // Determine type and skip control char if needed
                        if (i + 1 < input.Length)
                        {
                            var typedIndicatingChar = input[i + 1];
                            switch (typedIndicatingChar)
                            {
                                case '~':
                                    curSection.type = Section.VaryType.Random;
                                    curPipeIndex = i + 2;
                                    break;
                                case '&':
                                    curSection.type = Section.VaryType.Cycle;
                                    curPipeIndex = i + 2;
                                    break;
                                case '!':
                                    curSection.type = Section.VaryType.Once;
                                    curPipeIndex = i + 2;
                                    break;
                                default:
                                    curPipeIndex = i + 1;
                                    break;
                            }
                        }

                        // Mark start
                        curStartIndex = i;
                    }
                    currentDepth++;
                    break;
                case ']':
                    if (currentDepth == 1)
                    {
                        // Extract, including the ]
                        curSection.entire = input.Substring(curStartIndex, i - curStartIndex + 1);

                        // Add the last element
                        if (curPipeIndex < i)
                        {
                            curSection.elements.Add(input.Substring(curPipeIndex, i - curPipeIndex));
                        }
                    }
                    currentDepth--;
                    break;
                case '|':
                    if (currentDepth == 1)
                    {
                        // Split
                        curSection.elements.Add(input.Substring(curPipeIndex, i - curPipeIndex));

                        // Over the | on the next one
                        curPipeIndex = i + 1;
                    }
                    break;
            }
        }

        if (varyingSections.Count == 1)
        {
            if (varyingSections[0].type == Section.VaryType.Sequence && varyingSections[0].elements.Count == 0)
            {
                return false;
            }
        }
        return varyingSections.Count > 0;
    }

    private static bool ParseConditional(string input, int startIndex, out Section section)
    {
        section = new Section { type = Section.VaryType.Conditional };

        // Find the end of the conditional statement
        int endIndex = input.IndexOf(']', startIndex);
        if (endIndex == -1) return false;

        // Extract the content between [* and ]
        string content = input.Substring(startIndex + 2, endIndex - (startIndex + 2));

        // Split into parts: variable=value ? true : false
        string[] mainParts = content.Split('?');
        if (mainParts.Length != 2) return false;

        // Parse condition part
        string[] conditionParts = mainParts[0].Trim().Split(',');
        if (conditionParts.Length != 2) return false;

        // Store the variable name as is (with potential '=' prefix)
        section.variableName = conditionParts[0].Trim();
        section.compareValue = conditionParts[1].Trim();

        // Parse results part
        string[] resultParts = mainParts[1].Split(':');
        if (resultParts.Length != 2) return false;

        section.trueResult = resultParts[0].Trim();
        section.falseResult = resultParts[1].Trim();

        // Store the entire matched section
        section.entire = input.Substring(startIndex, endIndex - startIndex + 1);

        return true;
    }

    public static string SelectVariations(string input, int parentHash = 0)
    {
        List<Section> sections = new List<Section>();
        bool foundSections = TokeniseVarySections(input, sections);

        if (!foundSections)
        {
            return input;
        }

        StringBuilder sb = new StringBuilder();
        sb.Length = 0;
        sb.Append(input);

        for (int i = 0; i < sections.Count; i++)
        {
            var curSection = sections[i];
            string selected = string.Empty;

            if (curSection.type == Section.VaryType.Conditional)
            {
                int dummyIndex = 0;
                selected = curSection.Select(ref dummyIndex);
            }
            else
            {
                int index = -1;

                int hash = input.GetHashCode();
                hash ^= (hash << 13);
                int curSecHash = curSection.entire.GetHashCode();
                curSecHash ^= (curSecHash >> 17);
                int key = hash ^ curSecHash ^ parentHash;

                if (hashedSections.TryGetValue(key, out int foundVal))
                {
                    index = foundVal;
                }

                index++;

                selected = curSection.Select(ref index);
                hashedSections[key] = index;
            }

            selected = SelectVariations(selected, parentHash);
            sb.Replace(curSection.entire, selected);
        }
        return sb.ToString();
    }
}