using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SimpleNodeGraph
{
    [CustomEditor(typeof(NodeGraph))]
    public class NodeGraphAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Open Graph"))
            {
                NodeGraphEditorWindow.OpenEditor(target as NodeGraph);
            }

            DrawDefaultInspector();
            //SerializedProperty prop;
        }
    }

}
