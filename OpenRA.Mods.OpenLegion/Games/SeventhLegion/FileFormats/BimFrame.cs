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
            Height = s.ReadInt16();

            if (s.Length == s.Position + pixelsOffset * Height)
            {
                Width = pixelsOffset;
                Pixels = s.ReadBytes(Width * Height);
                return;
            }

            s.Position = pixelsOffset;
            var compressed = s.ReadBytes((int)(s.Length - s.Position));
            var readOffset = 0;
            s.Position = 4;

            var rows = new byte[Height][];

            for (var i = 0; i < Height; i++)
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

            Width = rows.Max(row => row.Length);
            Pixels = new byte[Width * Height];

            for (var i = 0; i < rows.Length; i++)
                Array.Copy(rows[i], 0, Pixels, i * Width, rows[i].Length);
        }
    }
}
