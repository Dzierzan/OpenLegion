using System.IO;

namespace OpenRA.Mods.Example.Games.WarWind.FileFormats
{
    public class D3grFrame
    {
        public readonly int OffsetX;
        public readonly int OffsetY;
        public readonly int Width;
        public readonly int Height;
        public readonly byte[] Pixels;

        public D3grFrame(Stream stream)
        {
            stream.ReadInt32(); // frameSize
            stream.ReadInt32(); // TODO paletteId? Transparency? Flags? Bitmask?
            OffsetX = stream.ReadInt16();
            OffsetY = stream.ReadInt16();
            Height = stream.ReadInt16();
            Width = stream.ReadInt16();
            Pixels = stream.ReadBytes(Width * Height);
        }
    }
}
