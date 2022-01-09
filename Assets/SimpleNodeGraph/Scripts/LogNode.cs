using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleNodeGraph
{
    [System.Serializable, Title("Debug Log")]
    [NodeCategory("Action")]

    public class LogNode : Node
    {
        [SerializeField, Title("In")]
        public InPin inPin = new InPin();

        [SerializeField, Title("Out")]
        public OutPin outPin = new OutPin();

        [SerializeField, Title("String")]
        public InDataPin<string> inputString = new InDataPin<string>();

        protected override void OnEnter()
        {
            Debug.Log(inputString.Data);
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
