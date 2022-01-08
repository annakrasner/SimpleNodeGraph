using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleNodeGraph
{
    [System.Serializable, Title("Branch")]
    [NodeCategory("Action")]

    public class BranchNode : Node
    {

        [SerializeField, Title("In")]
        public InPin inPin = new InPin();

        [SerializeField, Title("True")]
        public OutPin trueOutPin = new OutPin();

        [SerializeField, Title("False")]
        public OutPin falseOutPin = new OutPin();

        [SerializeField, Title("Boolean")]
        public InDataPin<bool> boolInput = new InDataPin<bool>();

        protected override void OnEnter()
        {
            OutputPin(boolInput.Data ? trueOutPin : falseOutPin);
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
