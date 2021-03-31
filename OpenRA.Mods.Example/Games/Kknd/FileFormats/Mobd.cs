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

using System;
using System.Collections.Generic;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.Kknd.FileFormats
{
	public class Mobd
	{
		public readonly MobdAnimation[] RotationalAnimations;
		public readonly MobdAnimation[] SimpleAnimations;

		public Mobd(SegmentStream stream)
		{
			// This is damn ugly, but it seems MOBD uses offsets from lvl start.
			var fileOffset = (uint)stream.BaseOffset;
			var firstFrameStart = stream.Length;

			var animationOffsets = new List<uint>();
			var rotationalAnimations = new List<MobdAnimation>();
			var simpleAnimations = new List<MobdAnimation>();

			while (stream.Position < firstFrameStart)
			{
				var value = stream.ReadInt32();

				if (value == 0 || (value - fileOffset < stream.Position && value >= fileOffset))
				{
					stream.Position -= 4;

					break;
				}

				animationOffsets.Add((uint)(stream.Position - 4));

				while (true)
				{
					value = stream.ReadInt32();

					if (value == -1 || value == 0)
						break;

					firstFrameStart = Math.Min(firstFrameStart, value - fileOffset);
				}
			}

			while (stream.Position < firstFrameStart)
			{
				var value = stream.ReadUInt32();

				if (value == 0)
					continue;

				animationOffsets.Remove(value - fileOffset);
				var returnPosition = stream.Position;
				stream.Position = value - fileOffset;
				rotationalAnimations.Add(new MobdAnimation(stream));
				stream.Position = returnPosition;
			}

			foreach (var animationOffset in animationOffsets)
			{
				stream.Position = animationOffset;
				simpleAnimations.Add(new MobdAnimation(stream));
			}

			RotationalAnimations = rotationalAnimations.ToArray();
			SimpleAnimations = simpleAnimations.ToArray();
		}
	}
}
