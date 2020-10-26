
using System;
using System.Collections.Generic;

// ripped from https://bitbucket.org/MattThorson/monocle-engine

namespace Nocturne
{
    public class BitTag
    {
        internal static int TotalTags = 0;
        internal static BitTag[] byID = new BitTag[32];
        private static Dictionary<string, BitTag> byName = new Dictionary<string, BitTag>( StringComparer.OrdinalIgnoreCase );

        public static BitTag Get( string name )
        {
            return byName[name];
        }

        public int ID;
        public int Value;
        public string Name;

        public BitTag( string name )
        {
            ID = TotalTags;
            Value = 1 << TotalTags;
            Name = name;

            byID[ID] = this;
            byName[name] = this;

            TotalTags += 1;
        }
    }
}
