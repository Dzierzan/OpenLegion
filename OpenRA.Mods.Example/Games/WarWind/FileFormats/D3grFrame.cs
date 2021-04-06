using System.IO;

namespace OpenRA.Mods.Example.Games.WarWind.FileFormats
{
    public class D3grFrame
    {
        public readonly int Width;
        public readonly int Height;
        public readonly byte[] Pixels;

        public D3grFrame(Stream stream)
        {
            stream.ReadInt32(); // frameSize
            stream.ReadInt32(); // TODO paletteId ?
            stream.ReadInt16(); // TODO origin x?
            stream.ReadInt16(); // TODO origin y?
            Height = stream.ReadInt16();
            Width = stream.ReadInt16();
            Pixels = stream.ReadBytes(Width * Height);
        }
    }
}
