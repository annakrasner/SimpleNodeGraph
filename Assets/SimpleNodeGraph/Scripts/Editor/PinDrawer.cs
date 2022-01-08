using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;


namespace SimpleNodeGraph
{

    public class PinDrawer
    {
        public Pin target;
        public Rect rect;
        public GUIStyle style;
        public GUIStyle labelStyle;
        public Color bgColor;

        public PinType type;
        public NodeDrawer nodeDrawer;

        public Action<PinDrawer> onClickPin;

        public float drawFraction = 0.5f;
        public int sideIndex = 0;
        public string title;

        public PinDrawer() { }
        public PinDrawer(Pin pin, NodeDrawer nodeDrawer, Action<PinDrawer> clickPinCallback)
        {
            this.target = pin;
            this.nodeDrawer = nodeDrawer;
            this.type = target.pinType;
            this.onClickPin = clickPinCallback;
            style = NodeGraphStyles.leftPinStyle;
            rect = new Rect(0, 0, 10f, 20f); //this should probably be an external style?


            switch (type)
            {
                case PinType.In:
                    style = NodeGraphStyles.leftPinStyle;
                    labelStyle = NodeGraphStyles.leftPinLabelStyle;
                    bgColor = Color.yellow;
                    break;
                case PinType.DataIn:
                    style = NodeGraphStyles.leftDataPinStyle;
                    labelStyle = NodeGraphStyles.leftPinLabelStyle;
                    bgColor = Color.white;
                    break;
                case PinType.Out:
                    style = NodeGraphStyles.rightPinStyle;
                    labelStyle = NodeGraphStyles.rightPinLabelStyle;
                    bgColor = Color.yellow;
                    break;
                case PinType.DataOut:
                    style = NodeGraphStyles.rightDataPinStyle;
                    labelStyle = NodeGraphStyles.rightPinLabelStyle;
                    bgColor = Color.white;
                    break;
            }

        }

        public virtual void Draw(Rect posRect, float padding = 0)
        {
            rect.y = nodeDrawer.rect.y + (nodeDrawer.rect.height * 0.5f) - rect.height * 0.5f;
            switch (type)
            {
                case PinType.In:
                case PinType.DataIn:
                    rect.x = nodeDrawer.rect.x - rect.width + rect.width;
                    rect.y = posRect.y + (EditorGUIUtility.singleLineHeight + 3) * sideIndex;
                    posRect.x = rect.x + 15f;
                    break;

                case PinType.Out:
                case PinType.DataOut:
                    rect.x = nodeDrawer.rect.x + nodeDrawer.rect.width - rect.width;
                    rect.y = posRect.y + (EditorGUIUtility.singleLineHeight+3) * sideIndex;
                    posRect.x = rect.x - 85f ;
                    break;
            }

            Color savedCol = GUI.color;
            GUI.color = bgColor;
            if (GUI.Button(rect, "",  style))
            {

                onClickPin?.Invoke(this);
                
            }

            GUI.color = savedCol;

            posRect.y = rect.y;
            posRect.width = EditorGUIUtility.labelWidth;

            GUI.Label(posRect, title, labelStyle);
        }


    }
}


