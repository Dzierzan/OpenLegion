using System;
using System.IO;
using System.Linq;

namespace OpenRA.Mods.OpenLegion.Games.SeventhLegion.FileFormats
{
    public class BimFrame
    {
        public readonly int Width;
        public readonly int Height;
        public readonly byte[] Pixels;

        public BimFrame(Stream s)
        {
            var pixelsOffset = s.ReadInt16();
            var dataHeight = s.ReadInt16();
            Height = dataHeight % 2 == 0 ? dataHeight : dataHeight + 1;

            if (s.Length == s.Position + pixelsOffset * dataHeight)
            {
                var dataWidth = pixelsOffset;
                Width = dataWidth % 2 == 0 ? dataWidth : dataWidth + 1;
                Pixels = new byte[Width * Height];

                for (var i = 0; i < dataHeight; i++)
                    Array.Copy(s.ReadBytes(dataWidth), 0, Pixels, i * Width, dataWidth);
            }
            else
            {
                s.Position = pixelsOffset;
                var compressed = s.ReadBytes((int)(s.Length - s.Position));
                var readOffset = 0;
                s.Position = 4;

                var rows = new byte[dataHeight][];

                for (var i = 0; i < dataHeight; i++)
                {
                    var numChunks = s.ReadInt16();
                    var row = Array.Empty<byte>();

                    for (var j = 0; j < numChunks; j++)
                    {
                        var offset = s.ReadInt16();
                        var copy = s.ReadInt16();

                        Array.Resize(ref row, Math.Max(row.Length, offset + copy));
                        Array.Copy(compressed, readOffset, row, offset, copy);

                        readOffset += copy;
                    }

                    rows[i] = row;
                }

                var dataWidth = rows.Max(row => row.Length);
                Width = dataWidth % 2 == 0 ? dataWidth : dataWidth + 1;
                Pixels = new byte[Width * Height];

                for (var i = 0; i < rows.Length; i++)
                    Array.Copy(rows[i], 0, Pixels, i * Width, rows[i].Length);
            }
        }
    }
}