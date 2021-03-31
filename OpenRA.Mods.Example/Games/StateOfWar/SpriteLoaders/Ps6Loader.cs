using System;
using System.IO;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Example.Games.StateOfWar.FileFormats;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.StateOfWar.SpriteLoaders
{
    public class Ps6SpriteFrame : ISpriteFrame
    {
        public SpriteFrameType Type { get; }
        public Size Size { get; }
        public Size FrameSize { get; }
        public float2 Offset { get; }
        public byte[] Data { get; }
        public bool DisableExportPadding => true;

        public Ps6SpriteFrame(Ps6Frame frame)
        {
            Type = SpriteFrameType.Rgba32;
            Size = new Size(frame.Width, frame.Height);
            FrameSize = new Size(frame.Width, frame.Height);
            Offset = new int2(frame.Width / 2 - frame.OriginX, frame.Height / 2 - frame.OriginY);
            Data = frame.Pixels;
        }
    }

    public class Ps6Loader : ISpriteLoader
    {
        public bool TryParseSprite(Stream s, string filename, out ISpriteFrame[] frames, out TypeDictionary metadata)
        {
            if (!filename.EndsWith(".ps6", StringComparison.OrdinalIgnoreCase))
            {
                metadata = null;
                frames = null;
                return false;
            }

            metadata = null;
            frames = new Ps6(s).Frames.Select(frame => new Ps6SpriteFrame(frame)).ToArray();

            return true;
        }
    }
}
