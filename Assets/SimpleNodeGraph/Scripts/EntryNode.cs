using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleNodeGraph
{
    [System.Serializable, Title("Entry")]
    [NodeCategory("Action")]
    public class EntryNode : Node
    {
        [SerializeField, Title("Out")]
        public OutPin outPin = new OutPin();
        protected override void OnEnter()
        {
            //throw new System.NotImplementedException();
            Debug.Log("Start?");
            OutputPin(outPin);
        }

        public override void OnTick()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnExit()
        {
            throw new System.NotImplementedException();
        }

    }

}
