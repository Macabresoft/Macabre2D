namespace Macabre2D.UI.Models {

    using Macabre2D.Framework;
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
        private const string ContentFolderName = @"Content";

        public BuildConfiguration(BuildPlatform platform) {
            this.Platform = platform;
        }

        public BuildConfiguration() {
        }

        [DataMember]
        public BuildPlatform Platform { get; } = BuildPlatform.DesktopGL;

        public void CreateContentFile(string projectDirectoryPath, IEnumerable<Asset> assets, bool isTemp, params string[] referencePaths) {
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

            foreach (var referencePath in referencePaths) {
                stringBuilder.AppendLine($@"/reference:{referencePath}");
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine(@"#---------------------------------- Content ---------------------------------#");
            stringBuilder.AppendLine();

            var gameSettingsPath = $@"{GameSettings.ContentFileName}{FileHelper.GameSettingsExtension}";
            stringBuilder.AppendLine($"#begin {gameSettingsPath}");
            stringBuilder.AppendLine($@"/importer:{nameof(GameSettingsImporter)}");
            stringBuilder.AppendLine($@"/processor:{nameof(GameSettingsProcessor)}");
            stringBuilder.AppendLine($@"/build:{gameSettingsPath}");
            stringBuilder.AppendLine();

            var assetManagerPath = $@"{AssetManager.ContentFileName}{FileHelper.AssetManagerExtension}";
            stringBuilder.AppendLine($"#begin {assetManagerPath}");
            stringBuilder.AppendLine($@"/importer:{nameof(AssetManagerImporter)}");
            stringBuilder.AppendLine($@"/processor:{nameof(AssetManagerProcessor)}");
            stringBuilder.AppendLine($@"/build:{assetManagerPath}");
            stringBuilder.AppendLine();

            foreach (var asset in assets) {
                asset.BuildProcessorCommands(stringBuilder, projectDirectoryPath);
                stringBuilder.AppendLine();
            }

            var contentFile = Path.Combine(projectDirectoryPath, isTemp ? $"{FileHelper.TempName}{FileHelper.ContentExtension}" : $"{this.Platform.ToString()}{FileHelper.ContentExtension}");
            File.WriteAllText(contentFile, stringBuilder.ToString());
        }

        public void GenerateContent(string projectDirectoryPath, IEnumerable<Asset> assets, AssetManager assetManager, GameSettings gameSettings, Serializer serializer, params string[] referencePaths) {
            this.CreateContentFile(projectDirectoryPath, assets, false, referencePaths);
        }

        public string GetBinaryFolderPath(string projectDirectoryPath, BuildMode mode) {
            return Path.Combine(projectDirectoryPath, "bin", this.Platform.ToString(), mode.ToString());
        }

        public string GetCompiledContentPath(string projectDirectoryPath, BuildMode mode) {
            return Path.Combine(this.GetBinaryFolderPath(projectDirectoryPath, mode), "Content");
        }

        public string GetContentPath(string projectDirectoryPath) {
            return Path.Combine(projectDirectoryPath, this.Platform.ToString(), ContentFolderName);
        }
    }
}