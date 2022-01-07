using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleNodeGraph
{
    [ExecuteAlways]
    public class Updater : MonoBehaviour
    {
        private static Updater _instance = null;
        public static System.Action onUpdate;

        public static Updater Instance()
        {
            if (_instance != null)
            {
                return _instance;
            }
            else
            {
                _instance = FindObjectOfType<Updater>();
                if(!_instance)
                {

                    GameObject go = new GameObject("SimpleNodeGraph.Updater", typeof(Updater));
                    _instance = go.GetComponent<Updater>();
                    if (Application.isPlaying)
                        DontDestroyOnLoad(go);

                }

                return _instance;
            }
        }

        public void Update()
        {
            onUpdate?.Invoke();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                UnityEditor.SceneView.RepaintAll();
            }
        }

#endif
    }
}
