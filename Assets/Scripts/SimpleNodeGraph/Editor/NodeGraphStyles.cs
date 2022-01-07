using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
namespace SimpleNodeGraph
{

    public static class NodeGraphStyles
    {
        public static int nodeHeight = 100;
        public static int nodeWidth = 200;

        public static GUIStyle defaultNodeStyle;
        public static GUIStyle selectedNodeStyle;

        public static Color gridColor = Color.gray;
        public static int gridSpacing = 100;
        public static float gridOpacity = 0.4f;

        public static GUIStyle leftPinStyle;
        public static GUIStyle rightPinStyle;
        public static GUIStyle leftDataPinStyle;
        public static GUIStyle rightDataPinStyle;



        public static GUIStyle leftPinLabelStyle;
        public static GUIStyle rightPinLabelStyle;

        static NodeGraphStyles()
        {
            defaultNodeStyle = new GUIStyle();
            defaultNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node0.png") as Texture2D; //node bg
            defaultNodeStyle.border = new RectOffset(12, 12, 12, 12);
            defaultNodeStyle.alignment = TextAnchor.MiddleCenter;
            defaultNodeStyle.normal.textColor = Color.white;

            selectedNodeStyle = new GUIStyle();
            selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node0 on.png") as Texture2D; //node bg
            selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
            selectedNodeStyle.alignment = TextAnchor.MiddleCenter;
            selectedNodeStyle.normal.textColor = Color.white;




            leftPinStyle = new GUIStyle();
            leftPinStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
            leftPinStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
            leftPinStyle.border = new RectOffset(4, 4, 12, 12);
            leftPinStyle.alignment = TextAnchor.MiddleLeft;

            leftDataPinStyle = new GUIStyle();
            leftDataPinStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
            leftDataPinStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
            leftDataPinStyle.border = new RectOffset(4, 4, 12, 12);
            leftDataPinStyle.alignment = TextAnchor.MiddleLeft;

            leftPinLabelStyle = new GUIStyle();
            leftPinLabelStyle.alignment = TextAnchor.MiddleLeft;
            leftPinLabelStyle.normal.textColor = Color.white;


            rightPinStyle = new GUIStyle();
            rightPinStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/slider thumb.png") as Texture2D;
            rightPinStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
            rightPinStyle.border = new RectOffset(4, 4, 12, 12);
            rightPinStyle.alignment = TextAnchor.MiddleRight;

            rightDataPinStyle = new GUIStyle();
            rightDataPinStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
            rightDataPinStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
            rightDataPinStyle.border = new RectOffset(4, 4, 12, 12);
            rightDataPinStyle.alignment = TextAnchor.MiddleRight;



            rightPinLabelStyle = new GUIStyle();
            rightPinLabelStyle.alignment = TextAnchor.MiddleRight;
            rightPinLabelStyle.normal.textColor = Color.white;
            


        }

    }

}