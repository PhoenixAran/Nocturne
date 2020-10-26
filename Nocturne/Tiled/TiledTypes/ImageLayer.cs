using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Nocturne.Tiled
{
    public class ImageLayer : ITmxLayer
    {
        public string Name { get; set; }
        public bool Visible { get; set; }
        public float Opacity { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }

        public int? Width;
        public int? Height;

        public Texture2D Image;

        public Dictionary<string, string> Properties { get; set; }
    }
}
