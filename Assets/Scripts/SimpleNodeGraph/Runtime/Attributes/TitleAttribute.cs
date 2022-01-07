using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SimpleNodeGraph
{
    public class TitleAttribute : Attribute
    {
        public string title;

        public TitleAttribute(string title)
        {
            this.title = title;
        }
    }
}

