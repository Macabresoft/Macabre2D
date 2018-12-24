namespace Macabre2D.Tests.UI.Services {

    using Macabre2D.Framework;
    using Macabre2D.UI.Services;
    using NUnit.Framework;
    using System.IO;

    [TestFixture]
    public static class AssemblyServiceTests {

        [Test]
        [Category("Unit Test")]
        public static void AssemblyService_LoadBaseComponentTypesTest() {
            var assemblyService = new AssemblyService();
            assemblyService.LoadAssemblies(Path.GetDirectoryName(Path.GetDirectoryName(Path.Combine(TestContext.CurrentContext.WorkDirectory, "Bin", "Debug")))).Wait();
            var types = assemblyService.LoadTypes(typeof(BaseComponent)).Result;
            Assert.IsNotEmpty(types);
            Assert.False(types.Contains(typeof(BaseComponent)));
        }
    }
}