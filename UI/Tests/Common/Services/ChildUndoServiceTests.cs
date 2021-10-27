namespace Macabresoft.Macabre2D.UI.Tests {
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Macabresoft.Macabre2D.UI.Common;
    using NUnit.Framework;

    [TestFixture]
    public sealed class ChildUndoServiceTests {
        [Test]
        [Category("Unit Tests")]
        public void GetChanges_Should_GetAllChanges() {
            var childUndoService = new ChildUndoService();
            var max = 10;
            var count = 0;

            for (var i = 0; i < max; i++) {
                childUndoService.Do(
                    () => count++,
                    () => count--);
            }

            var changes = childUndoService.GetChanges();

            using (new AssertionScope()) {
                childUndoService.CanUndo.Should().BeTrue();
                count.Should().Be(max);

                changes.Undo();
                count.Should().Be(0);

                changes.Do();
                count.Should().Be(max);
            }
        }
    }
}