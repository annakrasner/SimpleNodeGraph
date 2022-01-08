using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

//based on tutorial from https://gram.gs/gramlog/creating-node-based-editor-unity/

namespace SimpleNodeGraph
{
    public class NodeGraphEditorWindow : EditorWindow
    {
        private const string BEZIER_KEY = "SimpleNodeGraph_UseBezierConnections";

        private List<NodeDrawer> nodeDrawers;
        private List<ConnectionDrawer> connectionDrawers = new List<ConnectionDrawer>();

        private PinDrawer lastSelectedPinDrawer;
        private PinDrawer curSelectedPinDrawer;

        private Node nodeClipboard;
        public Vector2 nodePasteOffset = new Vector2(10, 10);

        private Vector2 drag;
        private Vector2 offset;
        private float zoomScrollVal = 0;
        public float Zoom
        {
            get
            {
                if (zoomScrollVal < 0)
                    return Mathf.Clamp(1f / (-zoomScrollVal), 0.1f, 1f);
                else if (zoomScrollVal > 0)
                    return Mathf.Clamp(zoomScrollVal, 1f, 10f);
                return 1;
            }
        }

        protected NodeGraph targetNodeGraph;

        public static bool bezierConnections { get; private set; } = true;

        private bool DrawingConnection { get { return curSelectedPinDrawer != null && lastSelectedPinDrawer == null; } }
        private bool isDragged;


        private EditorSkin editorSkin = new EditorSkin();
        private GUISkin guiSkin;

        public static void OpenEditor(NodeGraph graph)
        {
            NodeGraphEditorWindow window = GetWindow<NodeGraphEditorWindow>();
            window.titleContent = new GUIContent(graph.name);
            window.Init(graph);
        }


        [MenuItem("Tools/Node Graph Editor Base")]
        protected static void OpenWindow()
        {
            NodeGraphEditorWindow window = GetWindow<NodeGraphEditorWindow>();
            window.titleContent = new GUIContent("Node Graph Editor");

        }

        protected void Init(NodeGraph graph)
        {
            bezierConnections = EditorPrefs.GetBool(BEZIER_KEY);
            guiSkin = EditorGUIUtility.GetBuiltinSkin(editorSkin);
            targetNodeGraph = graph;
            //create nodes accordingly here
            nodeDrawers = new List<NodeDrawer>();
            foreach(var n in targetNodeGraph.nodes)
            {
               

                if (n != null)
                {
                    n.Init(targetNodeGraph);
                    nodeDrawers.Add(CreateNodeDrawer(n, n.graphPosition, NodeGraphStyles.nodeWidth, NodeGraphStyles.nodeHeight, OnPinClick, OnClickRemoveNode, OnClickCopyNode));
                }


            }

            foreach(var c in targetNodeGraph.connections)
            {
                if(c != null)
                {
                    PinDrawer pin1Drawer = null;
                    PinDrawer pin2Drawer = null;

                    var node1Drawer = nodeDrawers.Find(item => item.target == c.node1);
                //    pin1Drawer = node1Drawer.pinDrawers.Find(p_item => p_item.target == c.pin1);
                foreach(var pd in node1Drawer.pinDrawers)
                    {
                        if (pd.target.index == c.node1Ind)
                            pin1Drawer = pd;
                    }

                    var node2Drawer = nodeDrawers.Find(item => item.target == c.node2);
                    //pin2Drawer = node2Drawer.pinDrawers.Find(p_item => p_item.target == c.pin2);

                    foreach (var pd in node2Drawer.pinDrawers)
                    {
                        if (pd.target.index == c.node2Ind)
                            pin2Drawer = pd;
                    }

                    connectionDrawers.Add(new ConnectionDrawer(c, pin1Drawer, pin2Drawer, OnClickRemoveConnection));

                }
            }

            GUI.changed = true;

        }



        private void OnEnable()
        {
           

        

        }


        private void OnGUI()
        {
            DrawGrid();

            BeginZoomed(position, Zoom, 22f);
            DrawNodes();
            DrawConnections();


            ProcessNodeEvents(Event.current);

            ProcessEvents(Event.current);
            EndZoomed(position, Zoom, 22f);

            DrawToolbar();


            if (GUI.changed)
                Repaint();
        }

        private void DrawNodes()
        {
            if (nodeDrawers != null)
            {
                for (int i = 0; i < nodeDrawers.Count; i++)
                {
                    nodeDrawers[i].Draw();
                }
            }
        }

        private void DrawConnections()
        {
            if (connectionDrawers != null)
            {
                for (int i = 0; i < connectionDrawers.Count; i++)
                {
                    connectionDrawers[i].Draw();
                }
            }
        }

        protected virtual void ProcessEvents(Event e)
        {
            drag = Vector2.zero;

            if (DrawingConnection)
            {
                if (bezierConnections)
                {

                    Handles.DrawBezier(
                        curSelectedPinDrawer.rect.center,
                        e.mousePosition,
                        curSelectedPinDrawer.rect.center + Vector2.left * 50f,
                        e.mousePosition - Vector2.left * 50f,
                        Color.white,
                        null,
                        2f
                    );
                }
                else
                {
                    Handles.DrawLine(
                   curSelectedPinDrawer.rect.center,
                   e.mousePosition
                   //Color.white,
                   //texture: null,
                   //width: 2f
                   );
                }

                GUI.changed = true;
            }

            switch (e.type)
            {
                case EventType.MouseDown:
                    if(e.button == 1) //rightclick
                    {
                        if(DrawingConnection)
                        {
                            curSelectedPinDrawer = null;
                            lastSelectedPinDrawer = null;
                        }
                        else 
                            ProcessContextMenu(e.mousePosition);
                    }
                    break;
                case EventType.MouseDrag:
                    if(e.button == 2)
                    {
                        isDragged = true;
                        OnDrag(e); 
                    }
                    break;
                case EventType.DragUpdated:
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    Event.current.Use();

                    break;
                case EventType.DragPerform:
                    ProcessDragIntoWindow(e);
                    break;
                case EventType.MouseUp:
                    if(e.button == 0 && isDragged)
                    {
                        DragFinished();
                    }
                    isDragged = false;
                    break;

                case EventType.ScrollWheel:
                    zoomScrollVal += e.delta.y * 0.1f;
                    GUI.changed = true;
                    break;

                case EventType.KeyUp:
                    if(e.keyCode == KeyCode.Space)
                    {
                        ProcessNodeSearchWindow(e.mousePosition);

                    }
                    break;
            }
        }


        private void ProcessNodeSearchWindow(Vector2 mousePosition)
        {
            NodeSearchPopupEditor.Init(mousePosition + position.position, GetNodes(), (Type nodeType) =>
            {
                OnClickAddNode(mousePosition, nodeType);
            });
        }

        private void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            foreach (var nodeType in GetNodes())
            {
                string cat = "";
                string title = nodeType.Name;
                NodeCategoryAttribute catAttr = Attribute.GetCustomAttribute(nodeType, typeof(NodeCategoryAttribute)) as NodeCategoryAttribute;
                if (catAttr != null && !string.IsNullOrEmpty(catAttr.category))
                    cat = catAttr.category + "/";
                TitleAttribute titleAttr = Attribute.GetCustomAttribute(nodeType, typeof(TitleAttribute)) as TitleAttribute;
                if (titleAttr != null && !string.IsNullOrEmpty(titleAttr.title))
                    title = titleAttr.title;

                genericMenu.AddItem(new GUIContent("Create Node/" + cat + title), false, () =>
                {
                    OnClickAddNode(mousePosition, nodeType);
                });

                if(nodeClipboard != null)
                {
                    genericMenu.AddItem(new GUIContent("Paste Node"), false, () =>
                    {
                        OnClickPasteNode(mousePosition);
                    });
                }
                else
                {
                    genericMenu.AddDisabledItem(new GUIContent("Paste Node"));
                }
            }
            genericMenu.ShowAsContext();
        }

       protected virtual List<Type> GetNodes()
        {
            return NodeProvider.GetNodesOfBaseClass<Node>();
        }

        private void ProcessNodeEvents(Event e)
        {
            if(nodeDrawers != null)
            {
                for(int i = nodeDrawers.Count - 1; i >= 0; i--)
                {
                    bool guiChanged = nodeDrawers[i].ProcessEvents(e);

                    if (guiChanged)
                    {
                        GUI.changed = true;

                    }
                }
            }
        }

        private void OnDrag(Event e)
        {
            drag = e.delta;

            if (nodeDrawers != null)
            {
                for (int i = 0; i < nodeDrawers.Count; i++)
                {
                    nodeDrawers[i].Drag(e.delta);
                }
            }

            GUI.changed = true;
        }

        private void DragFinished()
        {
            if (nodeDrawers != null)
            {
                for (int i = 0; i < nodeDrawers.Count; i++)
                {
                    nodeDrawers[i].DragFinished();
                }
            }
        }

        protected virtual void ProcessDragIntoWindow(Event e)
        {

        }





        private void OnPinClick(PinDrawer pin)
        {
            lastSelectedPinDrawer = curSelectedPinDrawer;
            curSelectedPinDrawer = pin;

            if (lastSelectedPinDrawer != null)
            {
                if (curSelectedPinDrawer.nodeDrawer != lastSelectedPinDrawer.nodeDrawer)
                {
                    //create connection
                    if(CheckPinsCompatible(curSelectedPinDrawer, lastSelectedPinDrawer)
                        && CheckPinConnectionCount(curSelectedPinDrawer) && CheckPinConnectionCount(lastSelectedPinDrawer))
                    {
                        var connection = CreateConnection(curSelectedPinDrawer, lastSelectedPinDrawer);
                        connectionDrawers.Add(new ConnectionDrawer(connection, curSelectedPinDrawer, lastSelectedPinDrawer, OnClickRemoveConnection));

                    }

                }

                lastSelectedPinDrawer = null;
                curSelectedPinDrawer = null;
            }
        }

        private void OnClickRemoveConnection(ConnectionDrawer connection)
        {
            targetNodeGraph.RemoveConnection(connection.target);
            connectionDrawers.Remove(connection);
            EditorUtility.SetDirty(targetNodeGraph);

        }

        protected void OnClickAddNode(Vector2 mousePosition, System.Type type)
        {
            if (nodeDrawers == null)
            {
                nodeDrawers = new List<NodeDrawer>();
            }

            Node newNode = CreateNodeFromUI(mousePosition, type);
            CreateAndAddNodeDrawer(newNode, mousePosition);


        }

        private void OnClickRemoveNode(NodeDrawer nodeDrawer)
        {
            connectionDrawers.RemoveAll(item => nodeDrawer.pinDrawers.Contains(item.inPin) || nodeDrawer.pinDrawers.Contains(item.outPin));
            AssetDatabase.RemoveObjectFromAsset(nodeDrawer.target);
            targetNodeGraph.RemoveNode(nodeDrawer.target);
            nodeDrawers.Remove(nodeDrawer);
          //  AssetDatabase.SaveAssets();
        }

        private void OnClickCopyNode(NodeDrawer nodeDrawer)
        {
            nodeClipboard = CloneNode(nodeDrawer.target, nodeDrawer.target.GetType());
        }

        private void OnClickPasteNode(Vector2 mousePosition)
        {
            if (nodeClipboard != null)
            {

                nodeClipboard.graphPosition = mousePosition;
                nodeClipboard.Init(targetNodeGraph);
                AssetDatabase.AddObjectToAsset(nodeClipboard, targetNodeGraph);
                CreateAndAddNodeDrawer(nodeClipboard, nodeClipboard.graphPosition);
            }

            nodeClipboard = null;
        }

        protected virtual bool CheckPinsCompatible(PinDrawer pin1, PinDrawer pin2)
        {

            if (pin1.type == PinType.DataIn && pin2.type == PinType.DataOut)
                return true;
            if (pin1.type == PinType.DataOut && pin2.type == PinType.DataIn)
                return true;
            if (pin1.type == PinType.In && pin2.type == PinType.Out)
                return true;
            if (pin1.type == PinType.Out && pin2.type == PinType.In)
                return true;
            if (pin1.type == PinType.InOut && pin2.type == PinType.InOut)
                return true;

            return false;
        }

        protected virtual bool CheckPinConnectionCount(PinDrawer pinDrawer)
        {
            if (pinDrawer.type == PinType.DataIn)
                return GetConnectionCount(pinDrawer) < 1;
            if (pinDrawer.type == PinType.DataOut)
                return true;
            if (pinDrawer.type == PinType.In)
                return GetConnectionCount(pinDrawer) < 1;
            if (pinDrawer.type == PinType.Out)
                return GetConnectionCount(pinDrawer) < 1;
            if (pinDrawer.type == PinType.InOut)
                return GetConnectionCount(pinDrawer) < 1; ;

            return true;
        }


        private int GetConnectionCount(PinDrawer pinDrawer)
        {
            int result = 0;

            foreach(var c in connectionDrawers)
            {
                if (c.inPin == pinDrawer || c.outPin == pinDrawer)
                    result ++;
            }

            return result;
        }

        protected virtual NodeDrawer CreateNodeDrawer(Node target, Vector2 position, float width, float height,
    Action<PinDrawer> pinClickCallback, Action<NodeDrawer> nodeRemoveCallback, Action<NodeDrawer> nodeCopyCallback)
        {
            var result = new NodeDrawer(target, position, width, height, pinClickCallback, nodeRemoveCallback, nodeCopyCallback);
            return result;
        }

        protected virtual Node CreateNodeFromUI(Vector2 position, System.Type type)
        {
            if(type.IsSubclassOf(typeof(Node)))
            {

                var obj = ScriptableObject.CreateInstance(type.Name);
                var node = obj as Node;
                node.Init(targetNodeGraph);
                node.graphPosition = position;

                AssetDatabase.AddObjectToAsset(node, targetNodeGraph);
               // AssetDatabase.SaveAssets();
                return node;
            }
            return null;
        }

        private Node CloneNode(Node source, System.Type nodeType)
        {
            string data = JsonUtility.ToJson(source);

            var result =  ScriptableObject.CreateInstance(nodeType.Name);
            JsonUtility.FromJsonOverwrite(data, result);
            return result as Node;
        }

        protected void CreateAndAddNodeDrawer(Node newNode, Vector2 mousePosition)
        {
            if (newNode != null)
            {
                nodeDrawers.Add(CreateNodeDrawer(newNode, mousePosition, 200, 100,  OnPinClick, OnClickRemoveNode, OnClickCopyNode));
                targetNodeGraph.AddNode(newNode);

                EditorUtility.SetDirty(targetNodeGraph);
            }
        }

        protected virtual Connection CreateConnection(PinDrawer pinDrawer1, PinDrawer pinDrawer2)
        {
            var conn =  new Connection(pinDrawer1.target, pinDrawer2.target);
            targetNodeGraph.connections.Add(conn);

            return conn;
        }



        private void DrawGrid()
        {
            float gridSpacing = NodeGraphStyles.gridSpacing / Zoom;
            float gridOpacity = NodeGraphStyles.gridOpacity;
            Color gridColor = NodeGraphStyles.gridColor;

            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            offset += drag * 0.5f;
            Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
            }

            for (int j = 0; j < heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }

        private void DrawToolbar()
        {
            var rect = new Rect(0,0, position.width, position.height);
            rect.height = EditorGUIUtility.singleLineHeight * 2f;

            GUI.Box(rect, "", guiSkin.box);
            EditorGUILayout.BeginHorizontal();
            bezierConnections = EditorGUILayout.Toggle("Use Bezier", bezierConnections);
            EditorPrefs.SetBool(BEZIER_KEY, bezierConnections);
            if(GUILayout.Button("Reset Zoom"))
            {
                zoomScrollVal = 0;
                GUI.changed = true;
            }
            EditorGUILayout.EndHorizontal();
        }


        public static void BeginZoomed(Rect rect, float zoom, float topPadding)
        {
            GUI.EndClip();

            GUIUtility.ScaleAroundPivot(Vector2.one / zoom, rect.size * 0.5f);
            Vector4 padding = new Vector4(0, topPadding, 0, 0);
            padding *= zoom;
            GUI.BeginClip(new Rect(-((rect.width * zoom) - rect.width) * 0.5f, -(((rect.height * zoom) - rect.height) * 0.5f) + (topPadding * zoom),
                rect.width * zoom,
                rect.height * zoom));
        }

        public static void EndZoomed(Rect rect, float zoom, float topPadding)
        {
            GUIUtility.ScaleAroundPivot(Vector2.one * zoom, rect.size * 0.5f);
            Vector3 offset = new Vector3(
                (((rect.width * zoom) - rect.width) * 0.5f),
                (((rect.height * zoom) - rect.height) * 0.5f) + (-topPadding * zoom) + topPadding,
                0);
            GUI.matrix = Matrix4x4.TRS(offset, Quaternion.identity, Vector3.one);
        }

    }
}
