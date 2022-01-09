using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleNodeGraph
{
    public class NodeGraphRunnerBehaviour : MonoBehaviour
    {
        public NodeGraph graph;
        public bool runOnStart;

        private bool graphIsRunning = false;
        // Start is called before the first frame update
        void Start()
        {
            if (runOnStart)
                Run(); 
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Run()
        {
            if(!graphIsRunning)
            {
                graphIsRunning = true;
                graph.Run();
            }
        }
    }

}
