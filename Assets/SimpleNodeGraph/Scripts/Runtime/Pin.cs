using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SimpleNodeGraph
{
    public enum PinType
    {
        None,
        //action pins
        In,
        Out,
        InOut,
        //data pins
        DataIn,
        DataOut
    }


    [System.Serializable]
    public class Pin
    {
        [SerializeField]
        public PinType pinType { get; protected set; }

        [SerializeField]
        public Node node;

        [SerializeField]
        public string name;
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

    [System.Serializable]
    public class DataPin<T> : Pin
    {
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

   
}