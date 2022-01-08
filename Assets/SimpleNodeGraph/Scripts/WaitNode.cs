using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleNodeGraph
{
    [System.Serializable, Title("Wait for Seconds")]
    [NodeCategory("Action")]

    public class WaitNode : Node
    {

        [SerializeField, Title("In")]
        public InPin inPin = new InPin();

        [SerializeField, Title("Out")]
        public OutPin outPin = new OutPin();

        [SerializeField, Title("Time")]
        public InDataPin<float> waitTimeInput = new InDataPin<float>();


        float timeOut;

        protected override void OnEnter()
        {
            //throw new System.NotImplementedException();
            timeOut = Time.realtimeSinceStartup + waitTimeInput.Data;
            Debug.Log("entered wait node for time " + waitTimeInput.Data);
        }

        public override void OnTick()
        {
            if (Time.realtimeSinceStartup > timeOut)
            {
                OutputPin(outPin);
            }
            else
                Debug.Log("tick!");
        }

        protected override void OnExit()
        {
            throw new System.NotImplementedException();
        }
    }

}
