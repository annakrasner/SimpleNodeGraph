using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleNodeGraph
{
    [Title("Boolean Value")]
    public class BoolConstantNode : Node
    {
        [SerializeField, Title("Value")]
        public OutDataPin<bool> outPin = new OutDataPin<bool>();

        [SerializeField]
        public bool constant = false;

        public override void OnTick()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnEnter()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnExit()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnRequestData()
        {
            outPin.Data = constant;
        }
    }

}
