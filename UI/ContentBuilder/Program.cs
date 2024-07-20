// See https://aka.ms/new-console-template for more information

using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Common;
using Macabresoft.Macabre2D.UI.Common;
using Unity;

try {
    Console.WriteLine(@"Initializing services...");
    var assemblyService = new AssemblyService(new UnityContainer(), false);
    var fileSystem = new FileSystemService();
    var pathService = new PathService();
    var processService = new ProcessService();
    var buildService = new BuildService(assemblyService, fileSystem, pathService, processService, new Serializer());
    Console.WriteLine(@"Getting content directory...");
    var rootContentDirectory = new RootContentDirectory(fileSystem, pathService);
    Console.WriteLine(@"Creating MGCB file and building content...");
    buildService.BuildContentFromScratch(rootContentDirectory, true);
    Console.WriteLine(@"Done!");
}
catch (Exception e) {
    Console.WriteLine(e.ToString());
    Console.WriteLine(@"Failed!");
}


