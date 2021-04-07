using System;
using System.Collections.Generic;
using OpenRA.FileSystem;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Example.Games.WarWind.FileFormats;
using OpenRA.Traits;

namespace OpenRA.Mods.Example.Games.WarWind.Traits
{
	class PaletteFromD3grInfo : TraitInfo, IProvidesCursorPaletteInfo
	{
		[PaletteDefinition]
		[FieldLoader.Require]
		[Desc("Internal palette name")]
		public readonly string Name = null;

		[FieldLoader.Require]
		[Desc("Filename to load")]
		public readonly string Filename = null;

		public readonly bool AllowModifiers = true;

		[Desc("Whether this palette is available for cursors.")]
		public readonly bool CursorPalette = false;

		public override object Create(ActorInitializer init) { return new PaletteFromD3gr(init.World, this); }

		string IProvidesCursorPaletteInfo.Palette => CursorPalette ? Name : null;

		ImmutablePalette IProvidesCursorPaletteInfo.ReadPalette(IReadOnlyFileSystem fileSystem)
		{
			var d3gr = new D3gr(fileSystem.Open(Filename));

			if (d3gr.Palette == null)
				throw new InvalidOperationException("Unable to load palette `{0}` from `{1}`".F(Name, Filename));

			return new ImmutablePalette(d3gr.Palette.Colors);
		}
	}

	class PaletteFromD3gr : ILoadsPalettes, IProvidesAssetBrowserPalettes
	{
		readonly World world;
		readonly PaletteFromD3grInfo info;

		public IEnumerable<string> PaletteNames { get { yield return info.Name; } }

		public PaletteFromD3gr(World world, PaletteFromD3grInfo info)
		{
			this.world = world;
			this.info = info;
		}

		public void LoadPalettes(WorldRenderer wr)
		{
			wr.AddPalette(info.Name, ((IProvidesCursorPaletteInfo)info).ReadPalette(world.Map), info.AllowModifiers);
		}
	}
}
