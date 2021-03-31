using System;
using System.IO;
using OpenRA.Mods.Example.Games.Kknd.FileSystem;
using OpenRA.Mods.Example.Games.Kknd.SpriteLoaders;

namespace OpenRA.Mods.Example.Games.Kknd
{
    public class KkndKrossfire : Game
    {
        public KkndKrossfire()
            : base("KKND2", "Krush Kill ‘N Destroy 2: Krossfire")
        {
        }

        protected override string FindInstallation()
        {
            return FindGoGInstallation(1207659196);
        }

        protected override void UpdateModData(string installPath)
        {
            RegisterPackageLoader<LvlLoader>();

            RegisterSpriteLoader<MobdLoader>();
            RegisterSpriteLoader<MapdLoader>();
            RegisterSpriteLoader<BlitLoader>();

            /* SoundFormats: Wav */
            /* VideoFormats: Vbc */
            /* TODO: palettes */

            foreach (var file in Directory.GetFiles(installPath, "*", SearchOption.AllDirectories))
            {
                if (file.EndsWith(".lpk", StringComparison.OrdinalIgnoreCase))
                    Mount(file);
                else if (file.EndsWith(".bpk", StringComparison.OrdinalIgnoreCase))
                    Mount(file);
                else if (file.EndsWith(".spk", StringComparison.OrdinalIgnoreCase))
                    Mount(file);
                else if (file.EndsWith(".mpk", StringComparison.OrdinalIgnoreCase))
                    Mount(file);
                else if (file.EndsWith(".lps", StringComparison.OrdinalIgnoreCase))
                    Mount(file);
                else if (file.EndsWith(".lpm", StringComparison.OrdinalIgnoreCase))
                    Mount(file);
            }
        }
    }
}
