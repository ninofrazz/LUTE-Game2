using UnityEditor;
using UnityEngine;

namespace LoGaCulture.LUTE
{
    [CustomEditor(typeof(ShowLocationInfoPanel_Nino))]
    public class ShowLocationInfoPanel_NinoEditor : OrderEditor
    {
        protected SerializedProperty executeNodeProp;
        protected SerializedProperty buttonEventProp;

        public override void OnEnable()
        {
            base.OnEnable();

            executeNodeProp = serializedObject.FindProperty("executeNode");
            buttonEventProp = serializedObject.FindProperty("buttonEvent");
        }

        public override void DrawOrderGUI()
        {
            base.DrawOrderGUI();

            ShowLocationInfoPanel_Nino t = target as ShowLocationInfoPanel_Nino;
            var engine = (BasicFlowEngine)t.GetEngine();
            if (engine == null)
            {
                return;
            }

            serializedObject.Update();
            NodeEditor.NodeField(executeNodeProp,
                                 new GUIContent("Execute Node", "Node to call when button is clicked"),
                                 new GUIContent("<None>"),
                                 engine);

            EditorGUILayout.PropertyField(buttonEventProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}