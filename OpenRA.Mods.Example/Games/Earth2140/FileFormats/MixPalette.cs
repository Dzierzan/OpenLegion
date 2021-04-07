using System.IO;

namespace OpenRA.Mods.Example.Games.Earth2140.FileFormats
{
    public class MixPalette
    {
        public readonly uint[] Colors = new uint[256];

        public MixPalette(Stream stream)
        {
            for (var i = 0; i < Colors.Length; i++)
                Colors[i] = (uint)((0xff << 24) | (stream.ReadUInt8() << 16) | (stream.ReadUInt8() << 8) | (stream.ReadUInt8() << 0));
        }
    }
}
