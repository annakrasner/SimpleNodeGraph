using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleNodeGraph
{
    [CreateAssetMenu(menuName = "SimpleNodeGraph/Node Graph Base")]

    [System.Serializable]
    public class NodeGraph : ScriptableObject
    {
        [SerializeField]
        public List<Node> nodes = new List<Node>();

        [SerializeField]
        public List<Connection> connections = new List<Connection>();


        [SerializeField]
        private int nodeIdxCounter;

        public Node curNode;

        public virtual void AddNode(Node n)
        {
            n.SetIndex(nodeIdxCounter);
            nodes.Add(n);
            nodeIdxCounter++;

        }

        public virtual void RemoveNode(Node n)
        {
            var connsToRemove = connections.FindAll(item => n == item.node1 || n == item.node2);
            foreach(var c in connsToRemove)
            {
                RemoveConnection(c);
            }
            nodes.Remove(n);
            DestroyImmediate(n, true);
        }    


        public void RemoveConnection(Connection c)
        {
            c.Clear();
            connections.Remove(c);
            c.Dispose();
        }

        [ContextMenu("RUN")]
        public virtual void Run()
        {
            Updater.Instance();
            Updater.onUpdate -= Tick;
            Updater.onUpdate += Tick;

            Node startNode = null;

            foreach (var n in nodes)
            {
                List<Connection> nodeConnects = new List<Connection>();
                foreach(var c in connections)
                {
                    if(c.node1 == n || c.node2 == n)
                    {
                        nodeConnects.Add(c);
                    }
                }
                n.Init(this, nodeConnects);
                if (n is EntryNode)
                {
                    startNode = n;
                }

            }



            if (startNode != null)
            {
                curNode = startNode;
                startNode.Execute();
            }
        }

        public virtual void EndExecution()
        {
            curNode = null;
            Updater.onUpdate -= Tick;
            Debug.Log("Stop execution");
        }

        [ExecuteInEditMode]
        public void Tick()
        {
            Debug.Log("notegraph tick");
            curNode?.UpdateNode();
        }
    }

}
