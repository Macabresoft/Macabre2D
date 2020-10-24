namespace Macabresoft.Macabre2D.Tests.Editor.Framework.Services {

    using FluentAssertions;
    using FluentAssertions.Execution;
    using Macabresoft.Macabre2D.Editor.Framework.Services;
    using NUnit.Framework;

    [TestFixture]
    public sealed class ContentServiceTests {

        [Test]
        [Category("Integration Test")]
        public void Build_ShouldRunMGCB() {
            var service = new ContentService();

            using (new AssertionScope()) {
                service.Build(false).Should().Be(0);
            }
        }
    }
}