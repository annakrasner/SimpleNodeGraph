using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SimpleNodeGraph
{
    [System.Serializable]
    public abstract class Node : ScriptableObject
    {
        [SerializeField, HideInInspector]
        public int index { get; private set; }

        [SerializeField, HideInInspector]
        public Vector2 graphPosition;


        protected List<Pin> pinCache;

        //[HideInInspector]
       // [SerializeField]
        public List<Connection> connections = new List<Connection>();

        public NodeGraph parent;

        protected Action<Pin> onReturnOutput;


        public virtual void Init(NodeGraph parent, List<Connection> connections)
        {
            this.parent = parent;
            this.connections = connections;
            pinCache = NodeHelper.GetPins(this);
            var pinNames = NodeHelper.GetPinNames(this);
            for (int i = 0; i < pinCache.Count; i++)
            {
                pinCache[i].node = this;
                pinCache[i].name = pinNames[i];
            }
        }


        protected abstract void OnEnter();

        public abstract void OnTick();

   
        public void SetIndex(int idx)
        {
            index = idx;
        }

        protected void OutputPin(Pin result)
        {
            onReturnOutput?.Invoke(result);
        }

        public void UpdateNode()
        {
            OnTick();
        }

        public T PullData<T>(InDataPin<T> pin)
        {
            if(pin != null)
            {
                if (connections != null)
                {
                    for (int i = 0; i < connections.Count; i++) //this will fail horribly if we don't wait until each branch execution finishes. For now try to keep one to one connections
                    {
                        if (connections[i].node1 == pin.node && connections[i].pin1Name == pin.name)
                        {
                            var otherPin = connections[i].node2.GetPinWithName(connections[i].pin2Name);
                            return connections[i].node2.PushData<T>(otherPin as OutDataPin<T>);
                        }
                        else if (connections[i].node2 == pin.node && connections[i].pin2Name == pin.name)
                        {
                            var otherPin = connections[i].node1.GetPinWithName(connections[i].pin1Name);
                            return connections[i].node1.PushData<T>(otherPin as OutDataPin<T>);

                        }

                    }

                    //parent.EndExecution();

                }
                else
                {
                    Debug.Log("Nothing connected to node!"); //probably need a loop detection if possible
                }
            }

            return default(T);
        }

        public T PushData<T>(OutDataPin<T> pin)
        {
            OnRequestData();
            if(pin != null)
            {
                return pin.Data;
            }

            return default(T);
        }

        protected virtual void OnRequestData()
        {

        }

        public void Execute()
        {
            parent.curNode = this;
            onReturnOutput = (Pin result) =>
            {
                parent.curNode = null;
                if (result != null)
                {
                    if (connections != null)
                    {
                        for (int i = 0; i < connections.Count; i++) //this will fail horribly if we don't wait until each branch execution finishes. For now try to keep one to one connections
                        {
                            if (connections[i].node1 == result.node && connections[i].pin1Name == result.name)
                            {
                                connections[i].node2.Execute();
                                return;
                            }
                            else if (connections[i].node2 == result.node && connections[i].pin2Name == result.name)
                            {
                                connections[i].node1.Execute();
                                return;

                            }

                        }

                        parent.EndExecution();

                    }
                    else
                    {
                        Debug.Log("Nothing connected to node. Ending execution."); //probably need a loop detection if possible
                    }
                }
                else
                {
                    Debug.LogError("Node did not recieve a result! Execution stopped");

                }
            };

            OnEnter();
        }

        protected abstract void OnExit();


        public List<Pin> GetPins()
        {
            return pinCache;
        }

        public Pin GetPinWithName(string name)
        {
            return pinCache.Find(item => item.name == name);
        }
    }

  

}
