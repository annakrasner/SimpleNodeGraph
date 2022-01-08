using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleNodeGraph
{
    [Title("String Value")]
    [NodeCategory("Constants")]

    public class StringConstantNode : Node
    {
        [SerializeField, Title("Value")]
        public OutDataPin<string> outPin = new OutDataPin<string>();

        [SerializeField]
        public string constant = "";

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
