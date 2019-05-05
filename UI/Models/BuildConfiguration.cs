namespace Macabre2D.UI.Models {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Content;
    using Macabre2D.Framework.Serialization;
    using Macabre2D.UI.Common;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;

    public enum BuildMode {
        Debug,
        Release
    }

    public enum BuildPlatform {
        DesktopGL = 0
    }

    [DataContract]
    public sealed class BuildConfiguration {
        private const string ContentFileName = @"Content.mgcb";
        private const string ContentFolderName = @"Content";

        public BuildConfiguration(BuildPlatform platform) {
            this.Platform = platform;
        }

        public BuildConfiguration() {
        }

        [DataMember]
        public BuildPlatform Platform { get; } = BuildPlatform.DesktopGL;

        public void CopyMonoGameFrameworkDLL(string destination) {
            var source = Path.Combine("Configurations", this.Platform.ToString());
            var target = Path.Combine(destination, this.Platform.ToString());
            FileHelper.CopyDirectory(source, target);
        }

        public void GenerateContent(string sourcePath, IEnumerable<Asset> assets, AssetManager assetManager, GameSettings gameSettings, Serializer serializer, params string[] referencePaths) {
            var contentPath = this.GetContentPath(sourcePath);
            this.EraseContent(contentPath);
            serializer.Serialize(gameSettings, Path.Combine(contentPath, $"{AssetManager.ContentFileName}{FileHelper.AssetManagerExtension}"));
            serializer.Serialize(gameSettings, Path.Combine(contentPath, $"{GameSettings.ContentFileName}{FileHelper.GameSettingsExtension}"));
            this.CopyContent(contentPath, assets);
            this.CreateContentFile(contentPath, assets, referencePaths);
        }

        public string GetBinaryFolderPath(string sourcePath, BuildMode mode) {
            return Path.Combine(sourcePath, this.Platform.ToString(), "bin", mode.ToString());
        }

        public string GetCompiledContentPath(string sourcePath, BuildMode mode) {
            return Path.Combine(this.GetBinaryFolderPath(sourcePath, mode), "Content");
        }

        public string GetContentPath(string sourcePath) {
            return Path.Combine(sourcePath, this.Platform.ToString(), ContentFolderName);
        }

        private void CopyContent(string contentPath, IEnumerable<Asset> assets) {
            foreach (var asset in assets) {
                var path = Path.Combine(contentPath, asset.GetContentPath());
                var fileInfo = new FileInfo(path);
                fileInfo.Directory.Create();
                File.Copy(asset.GetPath(), Path.Combine(contentPath, path));
            }
        }

        private void CreateContentFile(string contentPath, IEnumerable<Asset> assets, params string[] referencePaths) {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("#----------------------------- Global Properties ----------------------------#");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(@"/outputDir:bin/$(Platform)");
            stringBuilder.AppendLine(@"/intermediateDir:obj/$(Platform)");
            stringBuilder.AppendLine($@"/platform:{this.Platform.ToString()}");
            stringBuilder.AppendLine(@"/config:");
            stringBuilder.AppendLine(@"/profile:Reach");
            stringBuilder.AppendLine(@"/compress:False");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(@"#-------------------------------- References --------------------------------#");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($@"/reference:..\..\..\Dependencies\References\Newtonsoft.Json.dll");
            stringBuilder.AppendLine($@"/reference:..\..\..\Dependencies\References\Macabre2D.Framework.dll");

            foreach (var referencePath in referencePaths) {
                stringBuilder.AppendLine($@"/reference:{referencePath}");
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine(@"#---------------------------------- Content ---------------------------------#");
            stringBuilder.AppendLine();

            var gameSettingsPath = $@"{contentPath}\{GameSettings.ContentFileName}{FileHelper.GameSettingsExtension}";
            stringBuilder.AppendLine($"#begin {gameSettingsPath}");
            stringBuilder.AppendLine($@"/importer:{nameof(GameSettingsImporter)}");
            stringBuilder.AppendLine($@"/processor:{nameof(GameSettingsProcessor)}");
            stringBuilder.AppendLine($@"/build:{gameSettingsPath}");
            stringBuilder.AppendLine();

            var assetManagerPath = $@"{contentPath}\{AssetManager.ContentFileName}{FileHelper.AssetManagerExtension}";
            stringBuilder.AppendLine($"#begin {assetManagerPath}");
            stringBuilder.AppendLine($@"/importer:{nameof(AssetManagerImporter)}");
            stringBuilder.AppendLine($@"/processor:{nameof(AssetManagerProcessor)}");
            stringBuilder.AppendLine($@"/build:{assetManagerPath}");
            stringBuilder.AppendLine();

            foreach (var asset in assets) {
                asset.BuildProcessorCommands(stringBuilder, contentPath);
                stringBuilder.AppendLine();
            }

            var contentFile = Path.Combine(contentPath, ContentFileName);
            File.WriteAllText(contentFile, stringBuilder.ToString());
        }

        private void EraseContent(string contentPath) {
            if (Directory.Exists(contentPath)) {
                Directory.Delete(contentPath, true);
            }

            Directory.CreateDirectory(contentPath);
        }
    }
}