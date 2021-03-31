using System.IO;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Example.Games.Earth2140.FileFormats;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.Earth2140.SpriteLoaders
{
    public class MixSpriteFrame : ISpriteFrame
    {
        public SpriteFrameType Type { get; }
        public Size Size { get; }
        public Size FrameSize { get; }
        public float2 Offset { get; }
        public byte[] Data { get; }
        public bool DisableExportPadding => true;

        public MixSpriteFrame(MixFrame frame)
        {
            Type = frame.Is32bpp ? SpriteFrameType.Rgba32 : SpriteFrameType.Indexed8;
            Size = new Size(frame.Width, frame.Height);
            FrameSize = new Size(frame.Width, frame.Height);
            Offset = new int2(frame.Width / 2, frame.Height / 2);
            Data = frame.Pixels;
        }
    }

    public class MixLoader : ISpriteLoader
    {
        public bool TryParseSprite(Stream s, string filename, out ISpriteFrame[] frames, out TypeDictionary metadata)
        {
            var start = s.Position;
            var identifier = s.ReadASCII(10);
            s.Position = start;

            if (identifier != "MIX FILE  ")
            {
                metadata = null;
                frames = null;
                return false;
            }

            metadata = null;
            frames = new Mix(s).Frames.Select(frame => new MixSpriteFrame(frame)).ToArray();

            return true;
        }
    }
}
