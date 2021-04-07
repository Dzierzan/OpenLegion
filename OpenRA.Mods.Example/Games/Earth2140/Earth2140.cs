using System;
using System.IO;
using OpenRA.Mods.Example.Games.Earth2140.FileFormats;
using OpenRA.Mods.Example.Games.Earth2140.FileSystem;
using OpenRA.Mods.Example.Games.Earth2140.SpriteLoaders;
using OpenRA.Mods.Example.Games.Earth2140.Traits;

namespace OpenRA.Mods.Example.Games.Earth2140
{
    public class Earth2140 : Game
    {
        public Earth2140()
            : base("EARTH2140", "Earth 2140")
        {
        }

        protected override string FindInstallation()
        {
            return FindGoGInstallation(1207658738) ?? FindSteamInstallation(253860);
        }

        protected override void UpdateModData(string installPath)
        {
            RegisterPackageLoader<WdLoader>();
            RegisterSpriteLoader<MixLoader>();

            foreach (var file in Directory.GetFiles(installPath, "*", SearchOption.AllDirectories))
            {
                if (!file.EndsWith(".WD", StringComparison.OrdinalIgnoreCase))
                    continue;

                var package = Mount(file, out var prefix);

                foreach (var content in package.Contents)
                {
                    if (!content.EndsWith(".MIX") || content.StartsWith("MIXMAX"))
                        continue;

                    var mix = new Mix(package.GetStream(content));

                    foreach (var index in mix.Palettes.Keys)
                    {
                        RegisterPalette(() =>
                        {
                            var key = $"{prefix}|{content}";
                            var paletteFromMix = new PaletteFromMixInfo();
                            FieldLoader.LoadField(paletteFromMix, nameof(paletteFromMix.Name), $"{key}:{index}");
                            FieldLoader.LoadField(paletteFromMix, nameof(paletteFromMix.Filename), key);
                            FieldLoader.LoadField(paletteFromMix, nameof(paletteFromMix.Index), $"{index}");

                            return paletteFromMix;
                        });
                    }
                }
            }
        }
    }
}
