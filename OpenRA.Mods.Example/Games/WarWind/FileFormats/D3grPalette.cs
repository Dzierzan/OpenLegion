using System.IO;

namespace OpenRA.Mods.Example.Games.WarWind.FileFormats
{
    public class D3grPalette
    {
        public readonly uint[] Colors = new uint[256];

        public D3grPalette(Stream stream)
        {
            stream.ReadInt16(); // 256
            stream.ReadInt16(); // 256

            for (var i = 0; i < Colors.Length; i++)
                Colors[i] = (uint)((0xff << 24) | (((stream.ReadUInt8() << 16) | (stream.ReadUInt8() << 8) | (stream.ReadUInt8() << 0)) << 2));
        }
    }
}
