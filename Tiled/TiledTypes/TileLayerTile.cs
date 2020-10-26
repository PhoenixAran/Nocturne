using Microsoft.Xna.Framework;

namespace Nocturne.Tiled
{
    /// <summary>
    /// Wrapper around TilesetTile. Do not reuse this class, as each MapData instance has unique tile Id mappings
    /// </summary>
    public class TileLayerTile
    {
        public TileLayerTileset TileLayerTileset;
        public int Gid;
        public int X;
        public int Y;
        public Vector2 Position => new Vector2( X, Y );

        /// <summary>
        /// get the actual tile represented by this TileLayerTile
        /// </summary>
        public TilesetTile TilesetTile
        {
            get
            {
                // No tile exists for Gid of 0
                if ( Gid < 1 )
                {
                    return null;
                }
                return TileLayerTileset.GetTile( Gid );
            }
        }

        public bool EmptyTile => Gid < 1;
    }
}
