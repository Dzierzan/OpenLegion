using System.IO;

namespace OpenRA.Mods.Example.Games.SeventhLegion.FileFormats
{
    public class Dat
    {
        public readonly uint[][] Colors;

        public Dat(Stream stream)
        {
            Colors = new uint[stream.Length / 256 / 3][];

            for (var i = 0; i < Colors.Length; i++)
            {
                var palette = new uint[256];
                Colors[i] = palette;

                for (var j = 0; j < palette.Length; j++)
                    palette[j] = (uint)((0xff << 24) | (((stream.ReadUInt8() << 16) | (stream.ReadUInt8() << 8) | (stream.ReadUInt8() << 0)) << 2));
            }
        }
    }
}
