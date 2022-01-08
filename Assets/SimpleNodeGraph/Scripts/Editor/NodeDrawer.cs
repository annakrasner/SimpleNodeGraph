using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace SimpleNodeGraph
{
    public class NodeDrawer
    {
        public Node target;

        public Rect rect;
        public string title;

        public GUIStyle style;

        public bool isDragged;
        public bool isSelected;

        public List<PinDrawer> pinDrawers;

        public Action<NodeDrawer> onRemoveNode;
        public Action<NodeDrawer> onCopyNode;
        protected Action<PinDrawer> pinClickCallback;

        protected SerializedObject so;

        public NodeDrawer() { }
        public NodeDrawer(Node target, Vector2 position, float width, float height, 
            Action<PinDrawer> pinClickCallback, Action<NodeDrawer> nodeRemoveCallback, Action<NodeDrawer> nodeCopyCallback)
        {
            this.target = target;
            

            rect = new Rect(position.x, position.y, width, height);
            this.pinClickCallback = pinClickCallback;
            onRemoveNode = nodeRemoveCallback;
            onCopyNode = nodeCopyCallback;

            TitleAttribute titleAttr = Attribute.GetCustomAttribute(target.GetType(), typeof(TitleAttribute)) as TitleAttribute;
            if (titleAttr != null)
                title = titleAttr.title;
            else
                title = target.GetType().Name;

            pinDrawers = new List<PinDrawer>();
            var pins = target.GetPins();



            foreach (var p in pins)
            {
                pinDrawers.Add(
                    CreatePinDrawer(p, this, pinClickCallback)
                );
            }

            SetupPinDrawersOnCreate();
          
            so = new SerializedObject(target);

            var prop_it = so.GetIterator();
            float neededHeight = EditorGUIUtility.singleLineHeight * 4 + 3f;
            while (prop_it.NextVisible(true))
            {
                bool shouldDraw = false;
                var fi = target.GetType().GetField(prop_it.propertyPath);
                if (fi != null)
                {
                    shouldDraw = target.GetType() == fi.DeclaringType && !typeof(Pin).IsAssignableFrom(fi.FieldType);
                }

                if (shouldDraw)
                {
                    neededHeight += EditorGUI.GetPropertyHeight(prop_it);
                }
            }

            var titles = NodeHelper.GetPinTitles(target);
            for(int i = 0; i < titles.Count; i++)
            {
                pinDrawers[i].title = titles[i];
            }

            neededHeight += GetPinHeightMargin() * (EditorGUIUtility.singleLineHeight);
            rect.height = Mathf.Max(neededHeight, rect.height);
            style = NodeGraphStyles.defaultNodeStyle;
        }


        protected virtual void SetupPinDrawersOnCreate()
        {

            List<PinDrawer> inPins = pinDrawers.FindAll(item => item.target.pinType == PinType.In || item.target.pinType == PinType.DataIn);
            List<PinDrawer> outPins = pinDrawers.FindAll(item => item.target.pinType == PinType.Out || item.target.pinType == PinType.DataOut);

            for(int i = 0; i < inPins.Count; ++i)
            {
                inPins[i].drawFraction = (i + 0.5f) /inPins.Count;
                inPins[i].sideIndex = i;
            }

            for (int i = 0; i < outPins.Count; ++i)
            {
                outPins[i].drawFraction = (i + 0.5f) / outPins.Count;
                outPins[i].sideIndex = i;
            }



        }
       

        public void Draw()
        {
  
            var posRect = rect;
            GUI.Box(posRect, "", style);



            posRect.height = EditorGUIUtility.singleLineHeight;
           

            var prop_it = so.GetIterator();

            var titleBarRect = posRect;
            titleBarRect.height *= 2f;
            GUI.Box(titleBarRect, Title(), style);


            posRect.y += titleBarRect.height /*+ EditorGUIUtility.singleLineHeight + 3f*/;

            var fieldRect = posRect;
            fieldRect.x += 15f; //margin
            fieldRect.width -= 40f;


            EditorGUIUtility.labelWidth = 75;


            fieldRect.y += (EditorGUIUtility.singleLineHeight + 3) * (GetPinHeightMargin() + 1);

            while (prop_it.NextVisible(true))
            {
                bool shouldDraw = false;
                var fi = target.GetType().GetField(prop_it.propertyPath);
                if(fi != null)
                {
                    shouldDraw = target.GetType() == fi.DeclaringType && !fi.FieldType.IsSubclassOf(typeof(Pin));
                }

                if(shouldDraw)
                {
                   EditorGUI.PropertyField(fieldRect, prop_it);
                   var h = EditorGUI.GetPropertyHeight(prop_it);
                   fieldRect.y += h;
                }

            }


            if (pinDrawers != null)
            {
                foreach (var p in pinDrawers)
                {
                    p.Draw(posRect);
                }
            }

            EditorGUIUtility.labelWidth = 0;

            so.ApplyModifiedProperties();


        }

        protected virtual int GetPinHeightMargin()
        {
            int inMargin = 0;
            int outMargin = 0;
            foreach(var p in pinDrawers)
            {
                switch(p.target.pinType)
                {
                    case PinType.In:
                    case PinType.DataIn:
                        inMargin++;
                        break;
                    case PinType.Out:
                    case PinType.DataOut:
                        outMargin ++;
                        break;
                }
            }

            return Mathf.Max(inMargin, outMargin);
        }

        protected virtual string Title()
        {
            return title;
            //return target.GetType().Name;
        }

        public void Drag(Vector2 delta)
        {
            rect.position += delta;
        }

        public void DragFinished()
        {
            so.FindProperty("graphPosition").vector2Value = rect.position;
            so.ApplyModifiedProperties();

        }

        public virtual bool ProcessEvents(Event e)
        {
            switch(e.type)
            {
                case EventType.MouseDown:
                    if(e.button == 0) //leftclick
                    {
                        if(rect.Contains(e.mousePosition))
                        {
                            isDragged = true;
                            GUI.changed = true;
                            isSelected = true;
                            style = NodeGraphStyles.selectedNodeStyle;
                        }
                        else
                        {
                            GUI.changed = true;
                            isSelected = false;
                            style = NodeGraphStyles.defaultNodeStyle;
                        }
                    }

                    else if(e.button == 1 /*&& isSelected*/ && rect.Contains(e.mousePosition))
                    {
                        ProcessContextMenu();
                        e.Use();
                    }

                    break;
                case EventType.MouseUp:
                    if (isDragged)
                        DragFinished();
                    isDragged = false;
                    break;
                case EventType.MouseDrag:
                    if(e.button == 0 && isDragged)
                    {
                        Drag(e.delta);
                        e.Use();
                        return true;
                    }
                    break;
                case EventType.KeyUp:
                    if(e.keyCode == KeyCode.Delete && isSelected)
                    {
                        OnClickRemoveNode();
                        GUI.changed = true;
                    }
                    break;
            }

            return false;
        }

        private void ProcessContextMenu()
        {
            var genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
            genericMenu.AddItem(new GUIContent("Copy node"), false, OnClickCopyNode);
            CreateContextMenu(genericMenu);
            genericMenu.ShowAsContext();
        }

        protected virtual void CreateContextMenu(GenericMenu menu)
        {
        }

        private void OnClickRemoveNode()
        {
            onRemoveNode?.Invoke(this);
        }

        private void OnClickCopyNode()
        {
            onCopyNode?.Invoke(this);

        }


        protected virtual PinDrawer CreatePinDrawer(Pin pin, NodeDrawer nodeDrawer, Action<PinDrawer> clickPinCallback)
        {
            return new PinDrawer(pin, nodeDrawer, clickPinCallback);
        }
    }

}
