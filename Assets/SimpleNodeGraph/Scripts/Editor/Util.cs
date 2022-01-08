using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;

namespace SimpleNodeGraph
{

    //taken from https://github.com/alelievr/NodeGraphProcessor
    public static class NodeProvider
    {
        public static Dictionary<Type, MonoScript> nodeScripts = new Dictionary<Type, MonoScript>();

        static NodeProvider()
        {
            BuildScriptCache();
        }

        static void BuildScriptCache()
        {
            foreach (var nodeType in TypeCache.GetTypesDerivedFrom<Node>())
            {
                if (!IsNodeAccessibleFromMenu(nodeType))
                    continue;

                AddNodeScriptAsset(nodeType);
            }

        }

        static void AddNodeScriptAsset(Type type)
        {
            var nodeScriptAsset = FindScriptFromClassName(type.Name);

            // Try find the class name with Node name at the end
            if (nodeScriptAsset == null)
                nodeScriptAsset = FindScriptFromClassName(type.Name + "Node");
            if (nodeScriptAsset != null)
                nodeScripts[type] = nodeScriptAsset;
        }

        static MonoScript FindScriptFromClassName(string className)
        {
            var scriptGUIDs = AssetDatabase.FindAssets($"t:script {className}");

            if (scriptGUIDs.Length == 0)
                return null;

            foreach (var scriptGUID in scriptGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(scriptGUID);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

                if (script != null && String.Equals(className, Path.GetFileNameWithoutExtension(assetPath), StringComparison.OrdinalIgnoreCase))
                    return script;
            }

            return null;
        }

        static bool IsNodeAccessibleFromMenu(Type nodeType)
        {
            if (nodeType.IsAbstract)
                return false;

            // return nodeType.GetCustomAttributes<NodeMenuItemAttribute>().Count() > 0;
            return true;
        }


        public static List<Type> GetNodesOfBaseClass<T>()
        {
            var result = new List<Type>();
            foreach (var nodeType in nodeScripts.Keys)
            {
               if(nodeType == typeof(T) || nodeType.IsSubclassOf(typeof(T)))
                {
                    result.Add(nodeType);
                }
            }

            return result;
        }
        

    }

}
