using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleNodeGraph
{
    [Title("Arithmetic")]
    [NodeCategory("Math")]
    public class ArithmeticNode : Node
    {
        public enum ArithmeticFunction
        {
            Plus,
            Minus,
            MultiplyBy,
            DivideBy
        }



        [SerializeField, Title("A")]
        public InDataPin<float> aPin = new InDataPin<float>();


        [SerializeField, Title("B")]
        public InDataPin<float> bPin = new InDataPin<float > ();

        [SerializeField, Title("Result")]
        public OutDataPin<float> outPin = new OutDataPin<float>();

        [SerializeField]
        public ArithmeticFunction function;


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
            switch(function)
            {
                case ArithmeticFunction.Plus:
                    outPin.Data = aPin.Data + bPin.Data;
                    break;
                case ArithmeticFunction.Minus:
                    outPin.Data = aPin.Data - bPin.Data;
                    break;
                case ArithmeticFunction.MultiplyBy:
                    outPin.Data = aPin.Data * bPin.Data;
                    break;
                case ArithmeticFunction.DivideBy:
                    outPin.Data = aPin.Data - bPin.Data;

                    break;
            }
        }
    }

}
