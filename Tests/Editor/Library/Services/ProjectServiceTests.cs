namespace Macabresoft.Macabre2D.Tests.Editor.Library.Services {
    using System;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Macabresoft.Macabre2D.Editor.Library.Models.Content;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class ProjectServiceTests {
        [Test]
        [Category("Unit Tests")]
        public void LoadProject_ShouldCreateProject_WhenFileDoesNotExist() {
            var fileSystem = Substitute.For<IFileSystemService>();
            var sceneService = Substitute.For<ISceneService>();
            var serializer = Substitute.For<ISerializer>();
            var undoService = Substitute.For<IUndoService>();
            var projectService = new ProjectService(
                Substitute.For<IBuildService>(),
                fileSystem,
                Substitute.For<ILoggingService>(),
                sceneService,
                serializer,
                undoService);

            var sceneAsset = new SceneAsset();
            sceneService.CreateNewScene(Arg.Any<string>(), Arg.Any<string>()).Returns(sceneAsset);

            var projectDirectoryPath = Path.Combine(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            fileSystem.DoesDirectoryExist(projectDirectoryPath).Returns(true);

            var projectFilePath = Path.Combine(projectDirectoryPath, ProjectService.ContentDirectory, GameProject.ProjectFileName);
            fileSystem.DoesFileExist(projectFilePath).Returns(false);

            serializer.When(x => x.Serialize(Arg.Any<GameProject>(), Arg.Any<string>()))
                .Do(x => {
                    if (x[1] is string path) {
                        fileSystem.DoesFileExist(path).Returns(true);
                        serializer.Deserialize<GameProject>(path).Returns(new GameProject());
                    }
                });


            var project = projectService.LoadProject(projectDirectoryPath);

            using (new AssertionScope()) {
                serializer.Received().Serialize(Arg.Any<GameProject>(), projectFilePath);
                project.Should().NotBeNull();
                projectService.CurrentProject.Should().NotBeNull();
                projectService.CurrentProject.Should().Be(project);
            }
        }

        [Test]
        [Category("Unit Tests")]
        public void LoadProject_Should_ArchiveMetadataWithMissingContent() {
            var metadataToArchive = new[] {
                new ContentMetadata(new SpriteSheet(), new[] { Folder1, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { Folder2, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { Folder1, Folder1A, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { Guid.NewGuid().ToString() }, ".jpg")
            };

            var container = new ContentContainer(Enumerable.Empty<ContentMetadata>(), metadataToArchive, Enumerable.Empty<string>());
            container.RunLoadProjectTest();
        }

        [Test]
        [Category("Unit Tests")]
        public void LoadProject_Should_CreateMetadataForNewContent() {
            var newContentFiles = new[] {
                Path.Combine(Folder1, $"{Guid.NewGuid()}.jpg"),
                Path.Combine(Folder2, $"{Guid.NewGuid()}.jpg"),
                Path.Combine(Folder1, Folder1A, $"{Guid.NewGuid()}.jpg"),
                $"{Guid.NewGuid()}.jpg"
            };

            var container = new ContentContainer(Enumerable.Empty<ContentMetadata>(), Enumerable.Empty<ContentMetadata>(), newContentFiles);
            container.RunLoadProjectTest();
        }

        [Test]
        [Category("Unit Tests")]
        public void LoadProject_Should_HandleAComplexSituation() {
            var existing = new[] {
                new ContentMetadata(new SpriteSheet(), new[] { Folder1, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { Folder2, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { Folder2, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { Folder1, Folder1A, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { Guid.NewGuid().ToString() }, ".jpg")
            };

            var metadataToArchive = new[] {
                new ContentMetadata(new SpriteSheet(), new[] { Folder1, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { Folder2, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { Folder1, Folder1A, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { Guid.NewGuid().ToString() }, ".jpg")
            };

            var newContentFiles = new[] {
                Path.Combine(Folder1, $"{Guid.NewGuid()}.jpg"),
                Path.Combine(Folder2, $"{Guid.NewGuid()}.jpg"),
                Path.Combine(Folder1, Folder1A, $"{Guid.NewGuid()}.jpg"),
                $"{Guid.NewGuid()}.jpg"
            };

            var container = new ContentContainer(existing, metadataToArchive, newContentFiles);
            container.RunLoadProjectTest();
        }

        [Test]
        [Category("Unit Tests")]
        public void LoadProject_Should_ResolveExistingMetadata() {
            var existing = new[] {
                new ContentMetadata(new SpriteSheet(), new[] { Folder1, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { Folder2, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { Folder1, Folder1A, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { Guid.NewGuid().ToString() }, ".jpg")
            };

            var container = new ContentContainer(existing, Enumerable.Empty<ContentMetadata>(), Enumerable.Empty<string>());
            container.RunLoadProjectTest();
        }

        [Test]
        [Category("Unit Tests")]
        public void LoadProject_ShouldLoad_WhenFileExists() {
            var fileSystem = Substitute.For<IFileSystemService>();
            var sceneService = Substitute.For<ISceneService>();
            var serializer = Substitute.For<ISerializer>();
            var undoService = Substitute.For<IUndoService>();
            var projectService = new ProjectService(
                Substitute.For<IBuildService>(),
                fileSystem,
                Substitute.For<ILoggingService>(),
                sceneService,
                serializer,
                undoService);

            var projectDirectoryPath = Path.Combine(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            fileSystem.DoesDirectoryExist(projectDirectoryPath).Returns(true);

            var projectFilePath = Path.Combine(projectDirectoryPath, ProjectService.ContentDirectory, GameProject.ProjectFileName);
            fileSystem.DoesFileExist(projectFilePath).Returns(true);

            var project = new GameProject();
            serializer.Deserialize<GameProject>(projectFilePath).Returns(project);

            var loadedProject = projectService.LoadProject(projectDirectoryPath);

            using (new AssertionScope()) {
                serializer.Received().Deserialize<GameProject>(projectFilePath);
                loadedProject.Should().Be(project);
                projectService.CurrentProject.Should().Be(project);
            }
        }

        [Test]
        [Category("Unit Tests")]
        public void LoadProject_ShouldNotLoad_WhenFileDoesNotExist() {
            var fileSystem = Substitute.For<IFileSystemService>();
            var sceneService = Substitute.For<ISceneService>();
            var serializer = Substitute.For<ISerializer>();
            var undoService = Substitute.For<IUndoService>();
            var projectService = new ProjectService(
                Substitute.For<IBuildService>(),
                fileSystem,
                Substitute.For<ILoggingService>(),
                sceneService,
                serializer,
                undoService);

            var projectFilePath = Path.Combine(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), GameProject.ProjectFileExtension);
            fileSystem.DoesFileExist(projectFilePath).Returns(false);


            using (new AssertionScope()) {
                projectService.Invoking(x => x.LoadProject(projectFilePath)).Should().Throw<NotSupportedException>();
                serializer.DidNotReceive().Deserialize<GameProject>(projectFilePath);
                projectService.CurrentProject.Should().BeNull();
            }
        }

        [Test]
        [Category("Unit Tests")]
        public void MoveContent_ShouldMoveDirectory() {
            var metadata = new ContentMetadata(new SpriteSheet(), new[] { Folder1, Guid.NewGuid().ToString() }, ".jpg");
            var existing = new[] {
                metadata,
                new ContentMetadata(new SpriteSheet(), new[] { Folder2, Guid.NewGuid().ToString() }, ".jpg")
            };

            var container = new ContentContainer(existing, Enumerable.Empty<ContentMetadata>(), Enumerable.Empty<string>());
            container.RunLoadProjectTest();

            var directory1 = container.Instance.RootContentDirectory.FindNode(new[] { Folder1 });
            var originalPath = directory1.GetFullPath();
            var directory2 = container.Instance.RootContentDirectory.FindNode(new[] { Folder2 }) as IContentDirectory;
            container.Instance.MoveContent(directory1, directory2);
            var newPath = directory1.GetFullPath();

            using (new AssertionScope()) {
                newPath.Should().NotBe(originalPath);
                container.FileSystem.Received().MoveDirectory(originalPath, newPath);
                directory1.Parent.Should().Be(directory2);
                // ReSharper disable once PossibleNullReferenceException
                directory2.Children.Should().Contain(directory1);
            }
        }

        [Test]
        [Category("Unit Tests")]
        public void MoveContent_ShouldMoveFile() {
            var metadata = new ContentMetadata(new SpriteSheet(), new[] { Folder1, Guid.NewGuid().ToString() }, ".jpg");
            var existing = new[] {
                metadata,
                new ContentMetadata(new SpriteSheet(), new[] { Folder2, Guid.NewGuid().ToString() }, ".jpg")
            };

            var container = new ContentContainer(existing, Enumerable.Empty<ContentMetadata>(), Enumerable.Empty<string>());
            container.RunLoadProjectTest();

            var contentFile = container.Instance.RootContentDirectory.FindNode(metadata.SplitContentPath.ToArray());
            var originalPath = contentFile.GetFullPath();
            var secondFolder = container.Instance.RootContentDirectory.FindNode(new[] { Folder2 }) as IContentDirectory;
            container.Instance.MoveContent(contentFile, secondFolder);
            var newPath = contentFile.GetFullPath();

            using (new AssertionScope()) {
                newPath.Should().NotBe(originalPath);
                container.FileSystem.Received().MoveFile(originalPath, newPath);
                contentFile.Parent.Should().Be(secondFolder);
                // ReSharper disable once PossibleNullReferenceException
                secondFolder.Children.Should().Contain(contentFile);
            }
        }

        [Test]
        [Category("Unit Tests")]
        public void SaveProject_ShouldNotSave_WhenProjectDoesNotExist() {
            var fileSystem = Substitute.For<IFileSystemService>();
            var sceneService = Substitute.For<ISceneService>();
            var serializer = Substitute.For<ISerializer>();
            var undoService = Substitute.For<IUndoService>();
            var projectService = new ProjectService(
                Substitute.For<IBuildService>(),
                fileSystem,
                Substitute.For<ILoggingService>(),
                sceneService,
                serializer,
                undoService);

            var projectDirectoryPath = Path.Combine(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var projectFilePath = Path.Combine(projectDirectoryPath, ProjectService.ContentDirectory, GameProject.ProjectFileName);
            projectService.SaveProject();

            using (new AssertionScope()) {
                serializer.DidNotReceive().Serialize(Arg.Any<IGameProject>(), projectFilePath);
            }
        }

        [Test]
        [Category("Unit Tests")]
        public void SaveProject_ShouldSave_WhenProjectExists() {
            var fileSystem = Substitute.For<IFileSystemService>();
            var sceneService = Substitute.For<ISceneService>();
            var serializer = Substitute.For<ISerializer>();
            var undoService = Substitute.For<IUndoService>();
            var projectService = new ProjectService(
                Substitute.For<IBuildService>(),
                fileSystem,
                Substitute.For<ILoggingService>(),
                sceneService,
                serializer,
                undoService);

            var projectDirectoryPath = Path.Combine(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var projectFilePath = Path.Combine(projectDirectoryPath, ProjectService.ContentDirectory, GameProject.ProjectFileName);
            fileSystem.DoesDirectoryExist(projectDirectoryPath).Returns(true);
            fileSystem.DoesFileExist(projectFilePath).Returns(true);
            var project = new GameProject();
            serializer.Deserialize<GameProject>(projectFilePath).Returns(project);
            projectService.LoadProject(projectDirectoryPath);
            projectService.SaveProject();

            using (new AssertionScope()) {
                serializer.Received().Serialize(project, projectFilePath);
            }
        }

        private const string Folder1 = "Folder1";
        private const string Folder2 = "Folder2";
        private const string Folder1A = "Folder1A";
    }
}