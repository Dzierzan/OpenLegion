using System;
using System.IO;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.OpenLegion.Games.SeventhLegion.FileFormats;
using OpenRA.Primitives;

namespace OpenRA.Mods.OpenLegion.Games.SeventhLegion.SpriteLoaders
{
	public class BimLoader : ISpriteLoader
	{
		private class BimSpriteFrame : ISpriteFrame
		{
			public SpriteFrameType Type => SpriteFrameType.Indexed8;
			public Size Size { get; }
			public Size FrameSize { get; }
			public float2 Offset { get; }
			public byte[] Data { get; }

			public bool DisableExportPadding => true;

			public BimSpriteFrame(BimFrame bimFrame)
			{
				var width = bimFrame.Width;
				var height = bimFrame.Height;

				Size = new Size(width, height);
				FrameSize = new Size(width, height);
				Offset = new float2(width / 2f, height / 2f);
				Data = bimFrame.Pixels;
			}
		}

		public bool TryParseSprite(Stream stream, string filename, out ISpriteFrame[] frames, out TypeDictionary metadata)
		{
			if (!filename.EndsWith(".bim", StringComparison.OrdinalIgnoreCase))
			{
				metadata = null;
				frames = null;

				return false;
			}

			frames = new Bim(stream, filename.ToLower()).Frames.Select(frame => new BimSpriteFrame(frame)).ToArray();
			metadata = null;

			return true;
		}
	}
}
