using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenRA.Mods.Example.Games.SeventhLegion.Utils;
using OpenRA.Primitives;
using FS = OpenRA.FileSystem.FileSystem;

namespace OpenRA.Mods.Example.Games.SeventhLegion.FileFormats
{
    public class Bim
    {
        public readonly BimFrame[] Frames;

        public Bim(Stream s, string filename)
        {
            var firstValue = s.ReadInt32();

            if (firstValue == 0x5a4c4356)
            {
                s = Decompressor.Decompress(s);
                firstValue = s.ReadInt32();
            }

            var firstFrame = firstValue;
            s.Position -= 4;

            var frames = new List<int[]>();

            while (s.Position < firstFrame)
            {
                var offset = s.ReadInt32();
                var end = offset;
                var position = s.Position;

                while (end == offset)
                {
                    if (s.Position == firstFrame)
                    {
                        end = (int)s.Length;
                        break;
                    }

                    end = s.ReadInt32();
                }

                s.Position = position;

                // Skip last entry if its 4 bytes (numFrames) (this entry does not always exist!)
                if (offset != s.Length - 4)
                    frames.Add(new[] { offset, end - offset });
            }

            Frames = frames.Select(e =>
            {
                if (filename == "font16.bim" && e[0] == 0xf74)
                {
                    // This is numFrames but with 172 bytes garbage before
                    return null;
                }

                if (filename == "tiles2.bim" && e[0] == 0x29f61c)
                {
                    // This is simply broken
                    return null;
                }

                return new BimFrame(new SegmentStream(s, e[0], e[1]));
            }).Where(e => e != null).ToArray();
        }
    }
}
