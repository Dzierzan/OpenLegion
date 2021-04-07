#region Copyright & License Information

/*
 * Copyright 2007-2021 The OpenKrush Developers (see AUTHORS)
 * This file is part of OpenKrush, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */

#endregion

using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Graphics;
using OpenRA.Mods.Example.Games.Kknd.FileFormats;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.Kknd.SpriteLoaders
{
	public class MobdLoader : ISpriteLoader
	{
		private class MobdSpriteFrame : ISpriteFrame
		{
			public SpriteFrameType Type => SpriteFrameType.Indexed8;
			public Size Size { get; }
			public Size FrameSize { get; }
			public float2 Offset { get; }
			public byte[] Data { get; }
			public readonly uint[] Palette;
			public readonly MobdPoint[] Points;

			public bool DisableExportPadding => true;

			public MobdSpriteFrame(MobdFrame mobdFrame)
			{
				var width = mobdFrame.RenderFlags.Image.Width;
				var height = mobdFrame.RenderFlags.Image.Height;

				Size = new Size(width, height);
				FrameSize = new Size(width, height);
				Offset = new int2(width / 2 - mobdFrame.OffsetX, height / 2 - mobdFrame.OffsetY);
				Data = mobdFrame.RenderFlags.Image.Pixels;
				Palette = mobdFrame.RenderFlags.Palette;
				Points = mobdFrame.Points;
			}
		}

		public bool TryParseSprite(Stream stream, string filename, out ISpriteFrame[] frames, out TypeDictionary metadata)
		{
			if (!filename.EndsWith(".mobd"))
			{
				metadata = null;
				frames = null;

				return false;
			}

			var mobd = new Mobd(stream as SegmentStream);
			var tmp = new List<MobdSpriteFrame>();

			tmp.AddRange(from mobdAnimation in mobd.RotationalAnimations from mobdFrame in mobdAnimation.Frames select new MobdSpriteFrame(mobdFrame));
			tmp.AddRange(from mobdAnimation in mobd.SimpleAnimations from mobdFrame in mobdAnimation.Frames select new MobdSpriteFrame(mobdFrame));

			uint[] palette = null;
			/*var points = new Dictionary<int, Offset[]>();*/

			for (var i = 0; i < tmp.Count; i++)
			{
				/*if (tmp[i].Points != null)
					points.Add(i, tmp[i].Points.Select(point => new Offset(point.Id, point.X, point.Y)).ToArray());*/

				if (tmp[i].Palette != null)
					palette = tmp[i].Palette;
			}

			frames = tmp.ToArray();

			metadata = new TypeDictionary { new EmbeddedSpritePalette(palette) /*, new EmbeddedSpriteOffsets(points)*/ };

			return true;
		}
	}
}
