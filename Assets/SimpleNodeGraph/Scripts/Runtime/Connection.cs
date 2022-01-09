using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SimpleNodeGraph
{
    [System.Serializable]
    public class Connection : IDisposable
    {
        [SerializeField]
        public Node node1;
        [SerializeField]
        public string pin1Name;

        [SerializeField]
        public Node node2;
        [SerializeField]
        public string pin2Name;


        public Connection(Pin pin1, Pin pin2)
        {
            node1 = pin1.node;
            pin1Name = pin1.name;

            node2 = pin2.node;
            pin2Name = pin2.name;
        }

        public void Clear()
        {
        }

        public void Dispose()
        {

        }
    }
}