using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace SimpleNodeGraph
{
    public class ConnectionDrawer
    {
        public PinDrawer inPin;
        public PinDrawer outPin;

        public Connection target;

        public Action<ConnectionDrawer> onClickRemoveConnection;

        private Color color;

        public ConnectionDrawer(Connection target, PinDrawer inPin, PinDrawer outPin, Action<ConnectionDrawer> clickRemoveConnectionCallback)
        {
            this.target = target;
            this.inPin = inPin;
            this.outPin = outPin;
            this.onClickRemoveConnection = clickRemoveConnectionCallback;

            color = Color.white;
            switch (this.inPin.type)
            {
                case PinType.In:
                    color = Color.yellow;
                    break;
                case PinType.DataIn:
                    color = Color.white;
                    break;
                case PinType.Out:
                    color = Color.yellow;
                    break;
                case PinType.DataOut:
                    color = Color.white;
                    break;
            }

        }

        public void Draw()
        {
            if(NodeGraphEditorWindow.bezierConnections)
            {
                Handles.DrawBezier(   
                inPin.rect.center,
                outPin.rect.center,
                inPin.rect.center + Vector2.left * 50f,
                outPin.rect.center - Vector2.left * 50f,
                color,
                texture: null,
                width: 2f);
             }
            else
            {
                Handles.color = color;
                Handles.DrawLine(
                    inPin.rect.center,
                    outPin.rect.center
                    //Color.white,
                    //texture: null,
                    //width: 2f
                    );
                Handles.color = Color.white;
            }


            if(Handles.Button((inPin.rect.center + outPin.rect.center) * 0.5f, direction: Quaternion.identity, size: 4, pickSize: 8, capFunction: Handles.RectangleHandleCap))
            {
                onClickRemoveConnection?.Invoke(this);
            }
        }
    }

}