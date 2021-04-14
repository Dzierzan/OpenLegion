using System;
using System.IO;
using OpenRA.Mods.OpenLegion.Games.SeventhLegion.FileFormats;
using OpenRA.Mods.OpenLegion.Games.SeventhLegion.SpriteLoaders;
using OpenRA.Mods.OpenLegion.Games.SeventhLegion.Traits;

namespace OpenRA.Mods.OpenLegion.Games.SeventhLegion
{
    public class SeventhLegion : Game
    {
        public SeventhLegion()
            : base("7LEGION", "7th Legion")
        {
        }

        protected override string FindInstallation()
        {
            return FindGoGInstallation(1207660773) ?? FindSteamInstallation(327910);
        }

        protected override void UpdateModData(string installPath)
        {
            RegisterSpriteLoader<BimLoader>();

            foreach (var directory in Directory.GetDirectories(installPath))
            {
                if (!Path.GetFileName(directory).Equals("gfx", StringComparison.OrdinalIgnoreCase))
                    continue;

                var package = Mount(directory, out var prefix);

                foreach (var content in package.Contents)
                {
                    if (content.EndsWith(".DAT", StringComparison.OrdinalIgnoreCase))
                    {
                        var dat = new Dat(package.GetStream(content));

                        for (var i = 0; i < dat.Colors.Length; i++)
                        {
                            var index = i;

                            RegisterPalette(() =>
                            {
                                var key = $"{prefix}|{content}";
                                var paletteFromDat = new PaletteFromDatInfo();
                                FieldLoader.LoadField(paletteFromDat, nameof(paletteFromDat.Name), $"{key}:{index}");
                                FieldLoader.LoadField(paletteFromDat, nameof(paletteFromDat.Filename), key);
                                FieldLoader.LoadField(paletteFromDat, nameof(paletteFromDat.Index), $"{index}");

                                return paletteFromDat;
                            });
                        }
                    }

                    if (content.EndsWith(".COL", StringComparison.OrdinalIgnoreCase))
                    {
                        RegisterPalette(() =>
                        {
                            var key = $"{prefix}|{content}";
                            var paletteFromCol = new PaletteFromColInfo();
                            FieldLoader.LoadField(paletteFromCol, nameof(paletteFromCol.Name), key);
                            FieldLoader.LoadField(paletteFromCol, nameof(paletteFromCol.Filename), key);

                            return paletteFromCol;
                        });
                    }
                }
            }
        }
    }
}
