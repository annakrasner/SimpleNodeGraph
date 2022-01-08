using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace SimpleNodeGraph
{


    public class NodeSearchPopupEditor : EditorWindow
    {
        Dictionary<Type, string> nodeToName = new Dictionary<Type, string>();
        List<Type> nodeList = new List<Type>();
        Action<Type> nodeSelectedCallback;
        string searchTerm = "";
        Vector2 scrollPos;

        public static void Init(Vector2 position, List<Type> nodeList, Action<Type> nodeSelectedCallback)
        {
            NodeSearchPopupEditor window = EditorWindow.CreateInstance<NodeSearchPopupEditor>();
            window.position = new Rect(position, new Vector2(250, 150));
            window.nodeList = nodeList;
            window.nodeSelectedCallback = nodeSelectedCallback;

            foreach (var n in window.nodeList)
            {
                string cat = "";
                string title = n.Name;
                NodeCategoryAttribute catAttr = Attribute.GetCustomAttribute(n, typeof(NodeCategoryAttribute)) as NodeCategoryAttribute;
                if (catAttr != null && !string.IsNullOrEmpty(catAttr.category))
                    cat = catAttr.category /*+ "/"*/;
                TitleAttribute titleAttr = Attribute.GetCustomAttribute(n, typeof(TitleAttribute)) as TitleAttribute;
                if (titleAttr != null && !string.IsNullOrEmpty(titleAttr.title))
                    title = titleAttr.title;

                window.nodeToName[n] = title;
            }

            window.ShowPopup();

        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("Search:");
            searchTerm = EditorGUILayout.TextField(searchTerm);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true);

            foreach (var n in nodeList)
            {
                string nodeName = nodeToName[n];
                if (nodeName.ToLower().StartsWith(searchTerm.ToLower()))
                {
                    if (GUILayout.Button(nodeName))
                    {
                        nodeSelectedCallback?.Invoke(n);
                        Close();
                    }
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void OnLostFocus()
        {
            Close();
        }


    }
}