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

        [HideInInspector]
        public List<Connection> connections = new List<Connection>();

        public NodeGraph parent;

        protected Action<Pin> onReturnOutput;


        public virtual void Init(NodeGraph parent)
        {
            this.parent = parent;
            pinCache = NodeHelper.GetPins(this);
            for (int i = 0; i < pinCache.Count; i++)
            {
                pinCache[i].node = this;
                pinCache[i].index = i;
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
                        if (connections[i].node1 == pin.node && connections[i].node1Ind == pin.index)
                        {
                            var otherPin = connections[i].node2.GetPinAtIndex(connections[i].node2Ind);
                            return connections[i].node2.PushData<T>(otherPin as OutDataPin<T>);
                        }
                        else if (connections[i].node2 == pin.node && connections[i].node2Ind == pin.index)
                        {
                            var otherPin = connections[i].node1.GetPinAtIndex(connections[i].node1Ind);
                            return connections[i].node1.PushData<T>(otherPin as OutDataPin<T>);

                        }

                    }

                    parent.EndExecution();

                }
                else
                {
                    Debug.Log("Nothing connected to node. Ending execution."); //probably need a loop detection if possible
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
                            if (connections[i].node1 == result.node && connections[i].node1Ind == result.index)
                            {
                                connections[i].node2.Execute();
                                return;
                            }
                            else if (connections[i].node2 == result.node && connections[i].node2Ind == result.index)
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


        public Pin GetPinAtIndex(int i)
        {
            if (i < 0 || i >= pinCache.Count)
                Debug.LogError("Asked for out of range pin!");
            return pinCache[i];
        }
    }

    public enum PinType { None, 
        //impulse pins
        In, 
        Out, 
        InOut,
        DataIn,
        DataOut
    }


    [System.Serializable]
    public class Pin
    {
        [SerializeField]
        public int index = -1;

        [SerializeField]
        public PinType pinType { get; protected set; }

        [SerializeField]
        public Node node;


    }


    [System.Serializable]
    public class InPin : Pin
    {
        public InPin()
        {
            pinType = PinType.In;
        }
    }

    [System.Serializable]
    public class OutPin : Pin
    {


        public OutPin()
        {
            pinType = PinType.Out;
        }
    }

    public class DataPin<T>: Pin
    {   
        //public virtual T GetValue<T>()
        //{
        //    switch (true)
        //    {
        //        case true when typeof(T) == typeof(string):
                    
        //            break;
        //        case true when typeof(T) == typeof(int):
        //            break;
        //        case true when typeof(T) == typeof(float):
        //            break;
        //        case true when typeof(T) == typeof(Vector2):
        //            break;
        //        case true when typeof(T) == typeof(Vector3):
        //            break;
        //        case true when typeof(T) == typeof(Vector4):
        //            break;
        //        case true when typeof(T) == typeof(Color):
        //            break;
        //        case true when !typeof(T).IsValueType:
        //            break;


        //    }
        //}
    }

    [System.Serializable]
    public class InDataPin<T> : DataPin<T>
    {
        public InDataPin()
        {
            pinType = PinType.DataIn;
        }

        public T Data
        {
            get
            {
                return node.PullData<T>(this);
            }
        }
    }

    [System.Serializable]
    public class OutDataPin<T> : DataPin<T>
    {
        protected T data;
        
        public T Data
        {
            get
            {
                return data;

            }

            set
            {
                data = value;
            }
        }

        public OutDataPin()
        {
            pinType = PinType.DataOut;
        }
    }

    [System.Serializable]
    public class Connection
    {
        [SerializeField]
        public Node node1;
        public int node1Ind;

        [SerializeField]
        public Node node2;
        public int node2Ind;


        public Connection(Pin pin1, Pin pin2)
        {
            node1 = pin1.node;
            node1Ind = pin1.index;

            node2 = pin2.node;
            node2Ind = pin2.index;

            node1.connections.Add(this);
            node2.connections.Add(this);
        }

        public void Clear()
        {
            node1.connections.Remove(this);
            node2.connections.Remove(this);
        }
    }

}
