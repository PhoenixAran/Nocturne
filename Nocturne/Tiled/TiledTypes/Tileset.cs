using System.Collections.Generic;

namespace Nocturne.Tiled
{
    /// <summary>
    /// raw tileset data directly translated from the tsx file. This requires the wrapper class TileLayerTileset for
    /// tile id mappings. This class can be reused since it does not contain MapData specific values like 'FirstGid'
    /// </summary>
    public class Tileset : TmxDocument, ITmxElement
    {
        public string Name { get; set; }

        public int Width;
        public int Height;
        public int TileHeight;
        public int TileWidth;
        public int FirstGid;

        public Dictionary<int, TilesetTile> Tiles;
        public SpriteSheet TileSpriteSheet;

        public TilesetTile GetTile( int gid )
        {
            return Tiles[gid];
        }
        
        public bool TryGetTile( int gid, out TilesetTile tile )
        {
            return Tiles.TryGetValue( gid, out tile );
        }
    }
}
