using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nocturne.Tiled
{
    public class ObjectLayer : ITmxLayer
    {
        public string Name { get; set; }
        public TmxList<TmxObject> Objects;
        public Dictionary<string, string> Properties { get; set; }
    }

    public class TmxObject : ITmxElement
    {
        public int Id;
        public string Name { get; set; }
        public TmxObjectType ObjectType;
        public string Type;
        public float X;
        public float Y;
        public float Width;
        public float Height;
        public float Rotation;
        public Vector2[] Points;

        public Dictionary<string, string> Properties;
    }

    public enum TmxObjectType
    {
        //supported
        Basic, 
        Point, 
        Ellipse,
        Polygon,
        Polyline,

        //unsupported for now...
        Tile,
        Text
    }
}
