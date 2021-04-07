using System.IO;

namespace OpenRA.Mods.Example.Games.WarWind.FileFormats
{
    public class D3gr
    {
        public readonly D3grFrame[] Frames;
        public readonly D3grPalette Palette;

        public D3gr(Stream stream)
        {
            stream.ReadASCII(4); // D3GR
            stream.ReadInt32(); // fileSize
            var framesStart = stream.ReadInt32();
            var paletteStart = stream.ReadInt32();
            stream.ReadInt32(); // 0
            stream.ReadInt32(); // 0
            Frames = new D3grFrame[stream.ReadInt16()];
            stream.ReadInt16(); // TODO paletteId?

            var offsets = new int[Frames.Length];

            for (var i = 0; i < offsets.Length; i++)
                offsets[i] = stream.ReadInt32();

            for (var i = 0; i < offsets.Length; i++)
            {
                stream.Position = framesStart + offsets[i];
                Frames[i] = new D3grFrame(stream);
            }

            if (paletteStart == 0)
                return;

            stream.Position = paletteStart;
            Palette = new D3grPalette(stream);
        }
    }
}
