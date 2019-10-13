namespace Macabre2D.UI.Tests.Services {
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Services;
    using NUnit.Framework;

    [TestFixture]
    public static class UndoServiceTests {
        [Test]
        [Category("Unit Test")]
        public static void UndoService_DoUndoRedoTest() {
            var undoService = new UndoService();
            var value = -1;
            for (var i = 0; i < 10; i++) {
                var command = new UndoCommand(() => value++, () => value--);
                undoService.Do(command);
                Assert.AreEqual(i, value);
            }

            Assert.True(undoService.CanUndo);
            Assert.False(undoService.CanRedo);

            var undoValue = value;
            while (undoService.CanUndo) {
                undoService.Undo();
                Assert.AreEqual(--undoValue, value);
            }

            Assert.False(undoService.CanUndo);
            Assert.True(undoService.CanRedo);

            var redoValue = value;
            while (undoService.CanRedo) {
                undoService.Redo();
                Assert.AreEqual(++redoValue, value);
            }

            Assert.True(undoService.CanUndo);
            Assert.False(undoService.CanRedo);
        }
    }
}