using UnityEngine;

namespace LoGaCulture.LUTE
{
    [VariableInfo("", "SaveKey")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class SaveKeyVariable : BaseVariable<string>
    {
        public override bool Evaluate(ComparisonOperator comparisonOperator, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                // Compare two save key strings (useful for comparing save keys only)
                return (base.Evaluate(comparisonOperator, value));
            }

            // Otherwise compare the value of key provided with the save manager's current save key (i.e., check if save exists)
            if (!string.IsNullOrEmpty(Value))
            {
                var saveManager = LogaManager.Instance.SaveManager;
                return saveManager.HasSaveData(Value);
            }

            // If no key is provided, return false
            return false;
        }
    }

    [System.Serializable]
    public struct SaveKeyData
    {
        [SerializeField]
        [VariableProperty("<Value>", typeof(SaveKeyVariable))]
        public SaveKeyVariable saveKeyRef;

        [SerializeField]
        public string saveKeyVal;

        public SaveKeyData(string v)
        {
            saveKeyVal = v;
            saveKeyRef = null;
        }

        public static implicit operator string(SaveKeyData stringData)
        {
            return stringData.Value;
        }

        public string Value
        {
            get
            {
                if (saveKeyVal == null) saveKeyVal = "";
                return (saveKeyRef == null) ? saveKeyVal : saveKeyRef.Value;
            }
            set { if (saveKeyRef == null) { saveKeyVal = value; } else { saveKeyRef.Value = value; } }
        }

        public string GetDescription()
        {
            if (saveKeyRef == null)
            {
                return saveKeyVal != null ? saveKeyVal : string.Empty;
            }
            else
            {
                return saveKeyRef.Key;
            }
        }
    }
}