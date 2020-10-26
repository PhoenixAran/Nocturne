
namespace Nocturne.Tiled
{
    // Maps in Tiled can have more than one tileset. To account for duplicate tile IDs, each tileset loaded
    // will have an attribute called "firstgid." (always defaults to 1) This firstgid is an id offset to keep tile ids unique.
    // Example, map has Tileset A and Tileset B. Tileset A has the default firstgid of 1 and Tileset B has first gid of 7
    // Tile 1 will point to A[1 - A.firstgid]
    // Tile 7 will point to B[7 - B.firstgid]

    /// <summary>
    /// Wrapper around Tileset. Do not reuse this class, as each MapData instance has unique TileLayerTilesets
    /// </summary>
    public class TileLayerTileset
    {
        public int FirstGid;

        public Tileset Tileset;

        public TilesetTile GetTile( int index ) 
        {
            return Tileset.GetTile( index - FirstGid );
        }
    }
}
