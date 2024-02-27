using System.Diagnostics;

Stopwatch sw = new();
sw.Start();

// JACK 1.1.1064 related settings
string pathCFG = "VDKGameCfg.ini";
string mapSourceFileType = "map";

string pathJACKDir = args[0];
string pathMapsSourceDir = args[1];
string pathMapsOutputDir = args[2];
string argsCSG = args[3];
string argsBSP = args[4];
string argsVIS = args[5];
string argsRAD = args[6];

string[] cfgLines = File.ReadAllLines($"{args[0]}\\{pathCFG}");

string pathCSG = "";
string pathBSP = "";
string pathVIS = "";
string pathRAD = "";
foreach (string line in cfgLines)
{
    string substring = line[..8];
    if (substring == "MapTool0")
    {
        pathCSG = line[9..];
    }
    else if (substring == "MapTool1")
    {
        pathBSP = line[9..];
    }
    else if (substring == "MapTool2")
    {
        pathVIS = line[9..];
    }
    else if (substring == "MapTool3")
    {
        pathRAD = line[9..];
    }
}

List<Task> compileTasks = [];

IEnumerable<string> pathMapSources = Directory.GetFiles(pathMapsSourceDir)
    .Where(line => line.EndsWith($".{mapSourceFileType}"));
foreach (string pathMapSource in pathMapSources)
{
    string fileNameWithExtension = pathMapSource[(pathMapsSourceDir.Length + 1)..];
    string pathMapOutput = $"{pathMapsOutputDir}\\{fileNameWithExtension}";

    compileTasks.Add(Task.Run(() =>
    {
        File.Copy(pathMapSource, pathMapOutput, true);
        Process.Start(pathCSG, $"{pathMapOutput} {argsCSG}").WaitForExit();
        Process.Start(pathBSP, $"{pathMapOutput} {argsBSP}").WaitForExit();
        Process.Start(pathVIS, $"{pathMapOutput} {argsVIS}").WaitForExit();
        Process.Start(pathRAD, $"{pathMapOutput} {argsRAD}").WaitForExit();
    }));
}

Task.WaitAll([.. compileTasks]);

Console.WriteLine($"\nTime elapsed: {sw.Elapsed}");