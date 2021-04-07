using System.IO;
using System.Text.RegularExpressions;
using OpenRA.Mods.Example.Games.WarWind.FileFormats;
using OpenRA.Mods.Example.Games.WarWind.FileSystem;
using OpenRA.Mods.Example.Games.WarWind.SpriteLoaders;
using OpenRA.Mods.Example.Games.WarWind.Traits;

namespace OpenRA.Mods.Example.Games.WarWind
{
    public class WarWind : Game
    {
        public WarWind()
            : base("WARWIND", "War Wind")
        {
        }

        protected override string FindInstallation()
        {
            return FindGoGInstallation(1455545518);
        }

        protected override void UpdateModData(string installPath)
        {
            RegisterPackageLoader<ResLoader>();
            RegisterSpriteLoader<D3grLoader>();

            foreach (var file in Directory.GetFiles(installPath, "*", SearchOption.AllDirectories))
            {
                if (!Regex.IsMatch(Path.GetFileName(file), "^RES.\\d{3}$", RegexOptions.IgnoreCase))
                    continue;

                var package = Mount(file, out var prefix);

                foreach (var content in package.Contents)
                {
                    if (!content.EndsWith(".D3GR"))
                        continue;

                    var d3gr = new D3gr(package.GetStream(content));

                    if (d3gr.Palette == null)
                        continue;

                    RegisterPalette(() =>
                    {
                        var key = $"{prefix}|{content}";
                        var paletteFromD3gr = new PaletteFromD3grInfo();
                        FieldLoader.LoadField(paletteFromD3gr, nameof(paletteFromD3gr.Name), key);
                        FieldLoader.LoadField(paletteFromD3gr, nameof(paletteFromD3gr.Filename), key);

                        return paletteFromD3gr;
                    });
                }
            }
        }
    }
}
