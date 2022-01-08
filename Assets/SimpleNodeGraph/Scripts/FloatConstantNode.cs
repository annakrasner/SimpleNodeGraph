using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleNodeGraph
{
    [Title("Float Value")]
    [NodeCategory("Constants")]

    public class FloatConstantNode : Node
    {
        [SerializeField, Title("Value")]
        public OutDataPin<float> outPin = new OutDataPin<float>();

        [SerializeField]
        public float constant = 0f;

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
