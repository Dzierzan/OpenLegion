using System.Collections.Generic;
using System.IO;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.StateOfWar.FileFormats
{
    public class Ps6
    {
	    public readonly Ps6Frame[] Frames;

	    public Ps6(Stream stream)
        {
			var frames = new List<Ps6Frame>();

			while (stream.Position < stream.Length)
			{
				var size = stream.ReadInt32();

				if (size == 0)
					break;

				frames.Add(new Ps6Frame(new SegmentStream(stream, stream.Position, size)));
			}

			Frames = frames.ToArray();
        }
    }
}
