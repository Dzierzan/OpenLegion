using System;
using System.IO;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Example.Games.WarWind.FileFormats;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.WarWind.SpriteLoaders
{
    public class D3grSpriteFrame : ISpriteFrame
    {
        public SpriteFrameType Type { get; }
        public Size Size { get; }
        public Size FrameSize { get; }
        public float2 Offset { get; }
        public byte[] Data { get; }
        public bool DisableExportPadding => true;

        public D3grSpriteFrame(D3grFrame frame)
        {
            Type = SpriteFrameType.Indexed8;
            Size = new Size(frame.Width, frame.Height);
            FrameSize = new Size(frame.Width, frame.Height);
            Offset = new int2(frame.Width / 2 + frame.OffsetX, frame.Height / 2 + frame.OffsetY);
            Data = frame.Pixels;
        }
    }

    public class D3grLoader : ISpriteLoader
    {
        public bool TryParseSprite(Stream s, string filename, out ISpriteFrame[] frames, out TypeDictionary metadata)
        {
            if (!filename.EndsWith(".D3GR", StringComparison.OrdinalIgnoreCase))
            {
                metadata = null;
                frames = null;
                return false;
            }

            var d3gr = new D3gr(s);

            if (d3gr.Frames.Length == 0)
            {
                metadata = null;
                frames = null;
                return false;
            }

            metadata = null;
            frames = d3gr.Frames.Select(frame => new D3grSpriteFrame(frame)).ToArray();

            return true;
        }
    }
}
