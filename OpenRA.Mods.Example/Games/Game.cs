using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;
using OpenRA.FileSystem;
using OpenRA.Graphics;

namespace OpenRA.Mods.Example.Games
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
            modData.ModFiles.Mount(Id + "|" + Path.GetRelativePath(installPath, file));
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
    }
}
