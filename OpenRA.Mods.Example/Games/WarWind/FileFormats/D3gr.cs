using System.IO;

namespace OpenRA.Mods.Example.Games.WarWind.FileFormats
{
    public class D3gr
    {
        public readonly D3grFrame[] Frames;

        public D3gr(Stream stream)
        {
            stream.ReadASCII(4); // D3GR
            stream.ReadInt32(); // TODO paletteOffset?
            var framesStart = stream.ReadInt32();
            stream.ReadInt32(); // TODO paletteOffset?
            stream.ReadInt32(); // TODO
            stream.ReadInt32(); // TODO
            Frames = new D3grFrame[stream.ReadInt16()];
            stream.ReadInt16(); // TODO

            var offsets = new int[Frames.Length];

            for (var i = 0; i < offsets.Length; i++)
                offsets[i] = stream.ReadInt32();

            for (var i = 0; i < offsets.Length; i++)
            {
                stream.Position = framesStart + offsets[i];
                Frames[i] = new D3grFrame(stream);
            }
        }
    }
}
