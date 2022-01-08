using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SimpleNodeGraph
{ 
    public static class NodeHelper
    {
        public static List<Pin> GetPins(Node target)
        {
            var result = new List<Pin>();
            var parentType = target.GetType();

            var fields = parentType.GetFields();
            {
                foreach (var fi in fields)
                {
                    System.Type ftype = fi.FieldType;
                    if (ftype.IsSubclassOf(typeof(Pin)))
                    {
                        var val = fi.GetValue(target);
                        result.Add(val as Pin);
                    }
                }
            }

            return result;
        }

        public static List<string> GetPinTitles(Node target)
        {
            var result = new List<string>();
            var parentType = target.GetType();

            var fields = parentType.GetFields();
            {
                foreach (var fi in fields)
                {
                    System.Type ftype = fi.FieldType;
                    if (ftype.IsSubclassOf(typeof(Pin)))
                    {
                        bool usedAttr = false;
                        var attrs = fi.GetCustomAttributes(typeof(TitleAttribute), false);
                        foreach(var attr in attrs)
                        {
                            if(attr is TitleAttribute)
                            {
                                result.Add((attr as TitleAttribute).title);
                                usedAttr = true;
                                break;
                            }
                        }

                        if(!usedAttr)
                            result.Add(fi.Name);
                    }


                }
            }

            return result;
        }

    
    }


    
}
