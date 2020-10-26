using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Nocturne.Tiled
{
    /// <summary>
    /// This class will load the MapData from a tmx file for you.
    /// </summary>
    public static class MapLoader
    {
        private static Dictionary<string, MapData> mapDataCache = new Dictionary<string, MapData>( StringComparer.OrdinalIgnoreCase );
        private static Dictionary<string, Tileset> tilesetCache = new Dictionary<string, Tileset>( StringComparer.OrdinalIgnoreCase );
        // TODO use spritebank class to store tileset spritesheet?
        private static Dictionary<string, SpriteSheet> tilesetSpritesheetCache = new Dictionary<string, SpriteSheet>( StringComparer.OrdinalIgnoreCase );
       
        public static MapData LoadMapData( string filepath )
        {
            var mapName = Path.GetFileNameWithoutExtension( filepath );

            if ( mapDataCache.ContainsKey( mapName ) )
            {
                return mapDataCache[mapName];
            }

            MapData mapData = new MapData();
            using ( Stream stream = TitleContainer.OpenStream( filepath ) )
            {
                XDocument xDoc = XDocument.Load( stream );
                mapData.TmxDirectory = Path.GetDirectoryName( filepath );

                XElement xMap = xDoc.Element( "map" );
                Insist.IsTrue( (string)xMap.Attribute( "orientation" ) == "orthogonal", "Only orthogonal maps are supported" );
                Insist.IsTrue( (string)xMap.Attribute( "renderorder" ) == "right-down", "Only right-down rendering order is supported" );


                mapData.Version = xMap.Attribute( "version" ).Value;
                mapData.TiledVersion = xMap.Attribute( "tiledversion" ).Value;

                mapData.Width = (int)xMap.Attribute( "width" );
                mapData.Height = (int)xMap.Attribute( "height" );
                mapData.TileWidth = (int)xMap.Attribute( "tilewidth" );
                mapData.TileHeight = (int)xMap.Attribute( "tileheight" );

                mapData.NextObjectId = (int?)xMap.Attribute( "nextobjectid" );
                mapData.BackgroundColor = ParseColor( xMap.Attribute( "backgroundcolor" ) );
                mapData.Properties = ParsePropertyDict( xMap.Element( "properties" ) );

                
                mapData.MaxTileWidth = mapData.TileWidth;
                mapData.MaxTileHeight = mapData.TileHeight;

                mapData.Tilesets = new List<TileLayerTileset>();

                foreach ( XElement e in xMap.Elements( "tileset" ) )
                {
                    Tileset tileset = ParseTileset( mapData, e, mapData.TmxDirectory );

                    //place tileset into wrapper in order for multiple tile support to work
                    TileLayerTileset tileLayerTileset = new TileLayerTileset();
                    tileLayerTileset.FirstGid = (int)e.Attribute( "firstgid" );
                    tileLayerTileset.Tileset = tileset;
                    mapData.Tilesets.Add( tileLayerTileset );
                }

                mapData.Layers = new TmxList<ITmxLayer>();
                mapData.TileLayers = new TmxList<TileLayer>();
                mapData.ObjectLayers = new TmxList<ObjectLayer>();
                mapData.ImageLayers = new TmxList<ImageLayer>();

                foreach ( XElement xLayer in xMap.Elements().Where(x => x.Name == "layer" || x.Name == "objectgroup" || x.Name == "imagelayer" || x.Name == "group" ) )
                {
                    ITmxLayer layer = null;
                    switch ( xLayer.Name.LocalName )
                    {
                        case "layer":
                            #region TileLayer
                            TileLayer tileLayer = new TileLayer();
                            tileLayer.Name = xLayer.Attribute( "name" ).Value;
                            tileLayer.Width = (int)xLayer.Attribute( "width" );
                            tileLayer.Height = (int)xLayer.Attribute( "height" );

                            XElement xData = xLayer.Element( "data" );
                            string encoding = xData.Attribute( "encoding" ).Value;

                            tileLayer.Tiles = new TileLayerTile[tileLayer.Width * tileLayer.Height];

                            if ( encoding == "base64" )
                            {
                                TmxBase64Data decodedStream = new TmxBase64Data( xData );
                                Stream dataStream = decodedStream.Data;

                                int index = 0;
                                using ( BinaryReader binaryReader = new BinaryReader( dataStream ) )
                                {
                                    for ( int j = 0; j < tileLayer.Height; ++j )
                                    {
                                        for ( int i = 0; i < tileLayer.Width; ++i )
                                        {
                                            uint gid = binaryReader.ReadUInt32();

                                            TileLayerTile layerTile = new TileLayerTile();
                                            layerTile.Gid = (int)gid;
                                            if ( !layerTile.EmptyTile )
                                            {
                                                layerTile.TileLayerTileset = mapData.GetTileSetForTileLayerGid( layerTile.Gid );
                                                layerTile.X = i;
                                                layerTile.Y = j;
                                            }
                                            tileLayer.Tiles[index] = layerTile;
                                            index += 1;
                                        }
                                    }
                                }
                                layer = tileLayer;
                            }
                            else if ( encoding == "csv" )
                            {
                                string csvData = xData.Value;
                                int index = 0;

                                foreach ( string s in csvData.Split( ',' ) )
                                {
                                    uint gid = uint.Parse( s.Trim() );
                                    int x = index % mapData.Width;
                                    int y = index % mapData.Height;

                                    TileLayerTile layerTile = new TileLayerTile();
                                    layerTile.Gid = (int)gid;
                                    if ( !layerTile.EmptyTile )
                                    {
                                        layerTile.TileLayerTileset = mapData.GetTileSetForTileLayerGid( layerTile.Gid );
                                        layerTile.X = x;
                                        layerTile.Y = y;
                                    }
                                    tileLayer.Tiles[index] = layerTile;
                                    index += 1;
                                }
                            }
                            mapData.TileLayers.Add( tileLayer );
                            layer = tileLayer;
                            #endregion
                            break;
                        case "objectgroup":
                            #region Object Layer
                            ObjectLayer objLayer = new ObjectLayer();
                            objLayer.Name = (string)xLayer.Attribute( "name" ) ?? string.Empty;
                            objLayer.Objects = new TmxList<TmxObject>();
                            foreach ( var xElement in xLayer.Elements( "object" ) )
                            {
                                objLayer.Objects.Add( ParseObject( xElement ) );
                            }
                            objLayer.Properties = ParsePropertyDict( xLayer.Element( "properties" ) );
                            mapData.ObjectLayers.Add( objLayer );
                            layer = objLayer;
                            #endregion
                            break;
                        case "imagelayer":
                            #region Image Layer
                            ImageLayer imageLayer = new ImageLayer();
                            imageLayer.Name = (string)xLayer.Attribute( "name" );
                            imageLayer.Width = (int?) xLayer.Attribute( "height" );
                            imageLayer.Visible = (bool?)xLayer.Attribute( "visible" ) ?? true;
                            imageLayer.Opacity = (float?)xLayer.Attribute( "opacity" ) ?? 1.0f;
                            imageLayer.OffsetX = (float?)xLayer.Attribute( "offsetx" ) ?? 0.0f;
                            imageLayer.OffsetY =  (float?)xLayer.Attribute( "offsety" ) ?? 0.0f;

                            XElement image = xLayer.Element( "image" );
                            if ( image != null )
                            {
                                imageLayer.Image = ParseTextureFromXImage( image, mapData.TmxDirectory );
                            }

                            imageLayer.Properties = ParsePropertyDict( xLayer.Element( "properties" ) );
                            mapData.ImageLayers.Add( imageLayer );
                            layer = imageLayer;
                            #endregion
                            break;
                        default:
                            throw new ArgumentOutOfRangeException( $"MapLoader does not support layer type {xLayer.Name.LocalName}" );
                    }

                    if ( layer != null )
                    {

                        mapData.Layers.Add( layer );
                    }
                }
            }

            mapDataCache.Add( mapName, mapData );
            return mapData;
        }


        public static Tileset LoadTileset( string tsxPath )
        {
            var tilesetName = Path.GetFileNameWithoutExtension( tsxPath );

            if ( tilesetCache.ContainsKey( tilesetName ) )
            {
                return tilesetCache[tilesetName];
            }


            using ( Stream stream = TitleContainer.OpenStream( tsxPath ) )
            {
                XDocument xDoc = XDocument.Load( stream );
                XElement xTileset = xDoc.Element( "tileset" );

                /*
                if ( Engine.ContentManager.HasResource( (string)xTileset.Attribute( "name" ) + "_tileset" ) )
                {
                    return Engine.ContentManager.GetResource<Tileset>( (string)xTileset.Attribute( "name" ) + "_tileset" );
                }
                */

                Tileset tileset = new Tileset();

                tileset.Name = (string)xTileset.Attribute( "name" );
                tileset.TileWidth = (int)xTileset.Attribute( "tilewidth" );
                tileset.TileHeight = (int)xTileset.Attribute( "tileheight" );

                int? spacing = (int?)xTileset.Attribute( "spacing" ) ?? 0;
                int? margin = (int?)xTileset.Attribute( "margin" ) ?? 0;

                XElement xImage = xTileset.Element( "image" );

                // not sure when a tileset will have it's spritesheet loaded without the tileset loaded but just in case
                if ( tilesetSpritesheetCache.ContainsKey( tileset.Name ) )
                {
                    tileset.TileSpriteSheet = tilesetSpritesheetCache[tileset.Name];
                }
                else
                {
                    tileset.TileSpriteSheet = new SpriteSheet(
                        ParseTextureFromXImage( xImage, Path.GetDirectoryName( tsxPath ) ), tileset.TileWidth, tileset.TileHeight, spacing.Value, margin.Value );
                    tilesetSpritesheetCache.Add( tileset.Name, tileset.TileSpriteSheet );

                    // load TilesetTiles
                    tileset.Tiles = new Dictionary<int, TilesetTile>();

                    // load the animated tiles and/or tiles with custom properties
                    foreach ( XElement xTile in xTileset.Elements( "tile" ) )
                    {
                        TilesetTile tilesetTile = ParseTilesetTile( tileset, xTile );
                        tileset.Tiles[tilesetTile.Id] = tilesetTile;
                    }

                    // load the basic tiles
                    for ( int i = 0; i < tileset.TileSpriteSheet.Count; ++i )
                    {
                        if ( tileset.Tiles.ContainsKey( i ) )
                            continue;

                        TilesetTile tilesetTile = new TilesetTile();
                        tilesetTile.Id = i;
                        tilesetTile.Texture = tileset.TileSpriteSheet.GetTexture( tilesetTile.Id );
                        tilesetTile.Properties = new Dictionary<string, string>();
                        tilesetTile.Type = "default";

                        tileset.Tiles[tilesetTile.Id] = tilesetTile;

                    }

                    // add tileset to cache
                    tilesetCache.Add( tileset.Name, tileset );
                }
                return tileset;
            }
        }

        #region Parsers
        public static Color ParseColor( XAttribute xColor )
        {
            if ( xColor == null )
                return Color.White;

            string colorStr = ( (string)xColor ).TrimStart( "#".ToCharArray() );
            return ColorExt.HexToColor( colorStr );
        }

        public static Dictionary<string, string> ParsePropertyDict( XContainer xmlProp )
        {
            if ( xmlProp == null )
                return null;

            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach ( XElement p in xmlProp.Elements( "property" ) )
            {
                string pname = p.Attribute( "name" ).Value;

                // Fallback to element value if no "value"
                XAttribute valueAttr = p.Attribute( "value" );
                string pval = valueAttr?.Value ?? p.Value;

                dict.Add( pname, pval );
            }
            return dict;
        }

        public static Tileset ParseTileset( MapData mapData, XElement xTilesetSource, string tmxDir )
        {
            XAttribute xFirstGid = xTilesetSource.Attribute( "firstgid" );
            int firstGid = (int)xFirstGid;
            string source = xTilesetSource.Attribute( "source" ).Value;

            Insist.IsNotNull( source, "Embedded tilesets are not supported" );
            source = Path.Combine( tmxDir, source );
            return LoadTileset( source );
        }

        public static Texture2D ParseTextureFromXImage( XElement xImage, string tmxDir )
        {
            var xSource = xImage.Attribute( "source" );

            if ( xSource == null )
            {
                throw new NotImplementedException( "Stream Image Data loading is not supported" );
            }

            string source = Path.Combine( tmxDir, (string)xSource );

            // dumb hack below because I'm too lazy to do content loading without the pipeline
            int index = source.LastIndexOf( '.' );

            source = index == -1 ? source : source.Substring( 0, index );

            // the path to the image extension since our image is a built in xnb folder
            string rootDirectory = Engine.ContentManager.RootDirectory;
            Engine.ContentManager.RootDirectory = "";
            // get the image
            Texture2D texture = Engine.ContentManager.Load<Texture2D>( source );
            // set the root directory to what it was
            Engine.ContentManager.RootDirectory = rootDirectory;

            return texture;
        }

        public static TilesetTile ParseTilesetTile( Tileset tileset, XElement xTile )
        {
            TilesetTile tile = new TilesetTile();

            tile.Id = (int)xTile.Attribute( "id" );
            tile.Type = (string)xTile.Attribute( "type" );
            tile.Texture = tileset.TileSpriteSheet.GetTexture( tile.Id );
            tile.AnimatedTextures = new List<Subtexture>();
            tile.Durations = new List<float>();

            if ( xTile.Element("animation") != null )
            {
                foreach ( XElement e in xTile.Element("animation").Elements("frame") )
                {
                    tile.AnimatedTextures.Add( tileset.TileSpriteSheet.GetTexture( (int)e.Attribute( "id" ) ) );
                    tile.Durations.Add( (float)e.Attribute( "duration" ) );
                }
            }

            tile.Properties = ParsePropertyDict( xTile );
            return tile;
        }

        public static TmxObject ParseObject( XElement xObject )
        {
            TmxObject obj = new TmxObject();
            obj.Id = (int?)xObject.Attribute( "id" ) ?? 0;
            obj.Name = (string)xObject.Attribute( "name" ) ?? string.Empty;
            obj.X = (float)xObject.Attribute( "x" );
            obj.Y = (float)xObject.Attribute( "y" );
            obj.Width = (float?)xObject.Attribute( "width" ) ?? 0.0f;
            obj.Height = (float?)xObject.Attribute( "height" ) ?? 0.0f;
            obj.Type = (string)xObject.Attribute( "type" ) ?? string.Empty;
            obj.Rotation = (float?)xObject.Attribute( "rotation" ) ?? 0.0f;

            XElement xEllipse = xObject.Element( "ellipse" );
            XElement xPolygon = xObject.Element( "polygon" );
            XElement xPolyline = xObject.Element( "polyline" );
            XElement xPoint = xObject.Element( "point" );

            XAttribute xGid = xObject.Attribute( "gid" );
            XElement xText = xObject.Element( "text" );

            if ( xGid != null )
            {
                throw new ArgumentOutOfRangeException( "MapLoader does not support tmx Tile object type. Consider using a MapLayer instead" );
            }

            if ( xText != null )
            {
                throw new ArgumentOutOfRangeException( " MapLoader does not support tmx text object type" );
            }

            if ( xEllipse != null )
            {
                obj.ObjectType = TmxObjectType.Ellipse;
            } 
            else if ( xPolygon != null )
            {
                obj.ObjectType = TmxObjectType.Polygon;
                obj.Points = ParsePoints( xPolygon );
            }
            else if ( xPolyline != null )
            {
                obj.ObjectType = TmxObjectType.Polyline;
            }
            else if ( xPoint != null )
            {
                obj.ObjectType = TmxObjectType.Point;
            }
            else
            {
                obj.ObjectType = TmxObjectType.Basic;
            }

            obj.Properties = ParsePropertyDict( xObject.Element( "properties" ) );

            return obj;
        }

        public static Vector2 ParsePoint( string s )
        {
            string[] pt = s.Split( ',' );
            float x = float.Parse( pt[0], NumberStyles.Float, CultureInfo.InvariantCulture );
            float y = float.Parse( pt[1], NumberStyles.Float, CultureInfo.InvariantCulture );
            return new Vector2( x, y );
        }

        public static Vector2[] ParsePoints( XElement xPoints )
        {
            string pointString = (string)xPoints.Attribute( "points" );
            string[] pointstringPair = pointString.Split( ' ' );
            Vector2[] points = new Vector2[pointstringPair.Length];

            int index = 0;
            foreach ( var elem in pointstringPair )
            {
                points[index] = ParsePoint( elem );
                index += 1;
            }

            return points;
        }
        #endregion

        /// <summary>
        /// Beware, does not dispose of Texture2D instances.
        /// </summary>
        public static void ClearCache()
        {
            mapDataCache.Clear();
            tilesetCache.Clear();
            tilesetSpritesheetCache.Clear();
        }
    }
}
