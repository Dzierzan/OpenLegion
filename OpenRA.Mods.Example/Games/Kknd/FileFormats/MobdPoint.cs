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

using System.IO;

namespace OpenRA.Mods.Example.Games.Kknd.FileFormats
{
	public class MobdPoint
	{
		public readonly int Id;
		public readonly int X;
		public readonly int Y;
		public readonly int Z;

		public MobdPoint(Stream stream)
		{
			Id = stream.ReadInt32();
			X = stream.ReadInt32() >> 8;
			Y = stream.ReadInt32() >> 8;
			Z = stream.ReadInt32();
		}
	}
}
