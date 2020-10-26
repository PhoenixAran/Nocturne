using System;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;

namespace Nocturne.Tiled
{
    public class TmxBase64Data
    {
        public Stream Data { get; private set; }

        public TmxBase64Data(XElement xData )
        {
            if ( xData.Attribute("string").Value != "base64" )
            {
                throw new Exception( "TmxBase64Data: Only Base64-encoded data is supported" );
            }

            byte[] rawData = Convert.FromBase64String( xData.Value );
            Data = new MemoryStream( rawData, false );

            string compression = xData.Attribute( "compression" ).Value;
            
            switch ( compression )
            {
                case "gzip":
                    Data = new GZipStream( Data, CompressionMode.Decompress );
                    break;
                case "zlib":
                    // Strip 2-byte header and 4-byte checksum
                    int bodyLength = rawData.Length - 6;
                    byte[] bodyData = new byte[bodyLength];

                    Array.Copy( rawData, 2, bodyData, 0, bodyLength );

                    MemoryStream bodyStream = new MemoryStream( bodyData, false );
                    Data = new DeflateStream( bodyStream, CompressionMode.Decompress );
                    break;
                default:
                    throw new Exception( $"TmxBase64Data: Unknown compression format  '{compression}'" );
            }
        }
    }
}
