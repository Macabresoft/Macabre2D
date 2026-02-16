namespace Macabre2D.UI.Tests;

using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Macabresoft.AvaloniaEx;
using Macabre2D.UI.Common;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class SaveServiceTests {
    [Test]
    [Category("Unit Tests")]
    public void HasChanges_IsFalse_AfterSaving() {
        var saveService = this.CreateSaveService(out var undoService);

        undoService.Do(() => { }, () => { });
        saveService.Save();

        using (new AssertionScope()) {
            saveService.HasChanges.Should().BeFalse();
        }
    }

    [Test]
    [Category("Unit Tests")]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(5)]
    [TestCase(8)]
    [TestCase(13)]
    [TestCase(50)]
    public void HasChanges_IsFalse_AfterUndoingChanges(int numberOfChanges) {
        var saveService = this.CreateSaveService(out var undoService);

        for (var i = 0; i < numberOfChanges; i++) {
            undoService.Do(() => { }, () => { });
        }

        for (var i = 0; i < numberOfChanges; i++) {
            undoService.Undo();
        }

        using (new AssertionScope()) {
            saveService.HasChanges.Should().BeFalse();
        }
    }

    [Test]
    [Category("Unit Tests")]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(5)]
    [TestCase(8)]
    [TestCase(13)]
    [TestCase(50)]
    public void HasChanges_IsFalse_AfterUndoingChangesSinceSave(int numberOfChanges) {
        var saveService = this.CreateSaveService(out var undoService);

        undoService.Do(() => { }, () => { });
        saveService.Save();

        for (var i = 0; i < numberOfChanges; i++) {
            undoService.Do(() => { }, () => { });
        }

        for (var i = 0; i < numberOfChanges; i++) {
            undoService.Undo();
        }

        using (new AssertionScope()) {
            saveService.HasChanges.Should().BeFalse();
        }
    }

    [Test]
    [Category("Unit Tests")]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(5)]
    [TestCase(8)]
    [TestCase(13)]
    [TestCase(50)]
    public void HasChanges_IsTrue_AfterChanges(int numberOfChanges) {
        var saveService = this.CreateSaveService(out var undoService);

        for (var i = 0; i < numberOfChanges; i++) {
            undoService.Do(() => { }, () => { });
        }

        using (new AssertionScope()) {
            saveService.HasChanges.Should().BeTrue();
        }
    }

    [Test]
    [Category("Unit Tests")]
    public void HasChanges_IsTrue_WhenUndoServiceHasNewChange() {
        var saveService = this.CreateSaveService(out var undoService);

        undoService.Do(() => { }, () => { });

        using (new AssertionScope()) {
            saveService.HasChanges.Should().BeTrue();
        }
    }

    private SaveService CreateSaveService(out UndoService undoService) {
        undoService = new UndoService();
        var saveService = new SaveService(
            Substitute.For<IContentService>(),
            Substitute.For<ICommonDialogService>(),
            Substitute.For<IEditorSettingsService>(),
            Substitute.For<IProjectService>(),
            Substitute.For<ISceneService>(),
            undoService);

        return saveService;
    }
}