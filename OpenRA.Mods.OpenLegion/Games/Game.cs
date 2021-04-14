using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using OpenRA.FileSystem;
using OpenRA.Graphics;
using OpenRA.Primitives;
using OpenRA.Traits;

namespace OpenRA.Mods.OpenLegion.Games
{
    public abstract class Game
    {
        protected string Id { get; }
        protected string Name { get; }

        private ModData modData;
        private string installPath;

        protected Game(string id, string name)
        {
            Id = id;
            Name = name;
        }

        protected abstract string FindInstallation();

        protected abstract void UpdateModData(string installPath);

        public void TryAdd(ModData modData)
        {
            Log.Write("debug", $"Checking for game {Name}...");

            this.modData = modData;
            installPath = FindInstallation();

            if (installPath == null)
            {
                Log.Write("debug", "Not found!");

                return;
            }

            Log.Write("debug", $"Found: {installPath}");
            modData.ModFiles.Mount(installPath, Id);
            UpdateModData(installPath);
        }

        protected void Mount(string file)
        {
            Mount(file, out _);
        }

        protected IReadOnlyPackage Mount(string file, out string path)
        {
            path = Path.GetRelativePath(installPath, file);
            modData.ModFiles.Mount(Id + "|" + path, path);

            var field = modData.ModFiles.GetType().GetField("explicitMounts", BindingFlags.NonPublic | BindingFlags.Instance);
            var dict = (Dictionary<string, IReadOnlyPackage>)field.GetValue(modData.ModFiles);

            return dict[path];
        }

        protected static string FindSteamInstallation(int appIdSteam)
        {
            foreach (var steamDirectory in SteamDirectory())
            {
                var manifestPath = Path.Combine(steamDirectory, "steamapps", $"appmanifest_{appIdSteam}.acf");

                if (!File.Exists(manifestPath))
                    continue;

                var data = ParseKeyValuesManifest(manifestPath);

                if (!data.TryGetValue("StateFlags", out var stateFlags) || stateFlags != "4")
                    continue;

                if (!data.TryGetValue("installdir", out var installDir))
                    continue;

                return Path.Combine(steamDirectory, "steamapps", "common", installDir);
            }

            return null;
        }

        private static IEnumerable<string> SteamDirectory()
        {
            var candidatePaths = new List<string>();

            if (Platform.CurrentPlatform == PlatformType.Windows)
            {
                var prefixes = new[] { "HKEY_LOCAL_MACHINE\\Software\\", "HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\" };

                foreach (var prefix in prefixes)
                    if (Registry.GetValue($"{prefix}Valve\\Steam", "InstallPath", null) is string path)
                        candidatePaths.Add(path);
            }
            else if (Platform.CurrentPlatform == PlatformType.OSX)
                candidatePaths.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", "Steam"));
            else
                candidatePaths.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".steam", "root"));

            foreach (var libraryPath in candidatePaths.Where(Directory.Exists))
            {
                yield return libraryPath;

                var libraryFoldersPath = Path.Combine(libraryPath, "steamapps", "libraryfolders.vdf");

                if (!File.Exists(libraryFoldersPath))
                    continue;

                var data = ParseKeyValuesManifest(libraryFoldersPath);

                for (var i = 1; ; i++)
                {
                    if (!data.TryGetValue(i.ToString(), out var path))
                        break;

                    yield return path;
                }
            }
        }

        private static Dictionary<string, string> ParseKeyValuesManifest(string path)
        {
            var regex = new Regex("^\\s*\"(?<key>[^\"]*)\"\\s*\"(?<value>[^\"]*)\"\\s*$");
            var result = new Dictionary<string, string>();

            using (var s = new FileStream(path, FileMode.Open))
            {
                foreach (var line in s.ReadAllLines())
                {
                    var match = regex.Match(line);

                    if (match.Success)
                        result[match.Groups["key"].Value] = match.Groups["value"].Value;
                }
            }

            return result;
        }

        protected static string FindGoGInstallation(int appIdGog)
        {
            if (Platform.CurrentPlatform != PlatformType.Windows)
                return null;

            return new[] { "HKEY_LOCAL_MACHINE\\Software\\", "HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\" }
                .Select(prefix => Registry.GetValue($"{prefix}GOG.com\\Games\\{appIdGog}", "path", null) as string)
                .FirstOrDefault(installDir => installDir != null && Directory.Exists(installDir));
        }

        protected void RegisterPackageLoader<T>() where T : IPackageLoader
        {
            // Make sure packages can be loaded.
            {
                var scope = modData;
                var field = scope.GetType().GetField(nameof(scope.PackageLoaders));

                if (field == null)
                    return;

                var value = (IPackageLoader[])field.GetValue(scope);
                var newEntry = new[] { (IPackageLoader)Activator.CreateInstance(typeof(T)) };

                if (value == null)
                    field.SetValue(scope, newEntry);
                else if (value.All(loader => loader.GetType() != typeof(T)))
                    field.SetValue(scope, value.Concat(newEntry).ToArray());
            }

            // Make sure the package loaders are known to the filesystem.
            {
                var scope = modData.ModFiles;
                var field = scope.GetType().GetField("packageLoaders", BindingFlags.NonPublic | BindingFlags.Instance);

                if (field == null)
                    return;

                var value = (IPackageLoader[])field.GetValue(scope);
                var newEntry = new[] { (IPackageLoader)Activator.CreateInstance(typeof(T)) };

                if (value == null)
                    field.SetValue(scope, newEntry);
                else if (value.All(loader => loader.GetType() != typeof(T)))
                    field.SetValue(scope, value.Concat(newEntry).ToArray());
            }
        }

        protected void RegisterSpriteLoader<T>() where T : ISpriteLoader
        {
            // Make sure sprites can be loaded.
            {
                var scope = modData.Manifest;
                var field = scope.GetType().GetField(nameof(scope.SpriteFormats));

                if (field == null)
                    return;

                var value = (string[])field.GetValue(scope);
                var newEntry = new[] { typeof(T).Name.Substring(0, typeof(T).Name.Length - "Loader".Length) };

                if (value == null)
                    field.SetValue(scope, newEntry);
                else if (value.All(loader => loader.GetType() != typeof(T)))
                    field.SetValue(scope, value.Concat(newEntry).ToArray());
            }

            // Make sure the asset browser lists them.
            {
                var scope = modData.Manifest.Get<AssetBrowser>();
                var field = scope.GetType().GetField(nameof(scope.SupportedExtensions));

                if (field == null)
                    return;

                var value = (string[])field.GetValue(scope);
                var newEntry = new[] { "." + typeof(T).Name.Substring(0, typeof(T).Name.Length - "Loader".Length).ToLowerInvariant() };

                if (value == null)
                    field.SetValue(scope, newEntry);
                else if (value.All(extension => extension != newEntry[0]))
                    field.SetValue(scope, value.Concat(newEntry).ToArray());
            }
        }

        private readonly List<Func<TraitInfo>> palettes = new List<Func<TraitInfo>>();

        protected void RegisterPalette(Func<TraitInfo> paletteCreator)
        {
            palettes.Add(paletteCreator);
        }

        public void CreateRules()
        {
            var world = modData.DefaultRules.Actors["world"];
            var field = world.GetType().GetField("traits", BindingFlags.NonPublic | BindingFlags.Instance);

            if (field == null)
                return;

            var traits = (TypeDictionary)field.GetValue(world);

            if (traits == null)
                return;

            foreach (var paletteCreator in palettes)
                traits.Add(paletteCreator());
        }
    }
}
