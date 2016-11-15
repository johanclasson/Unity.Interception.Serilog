#tool "nuget:?package=GitVersion.CommandLine"
#tool "nuget:?package=xunit.runner.console"

var target = Argument("target", "Test");
string version = "";

const string outputDir = "out";
const string buildPlatform = "AnyCPU";
const string configuration = "Release";

public static void EnsureDirExists(string path){
    if (!System.IO.Directory.Exists(path)) {
        System.IO.Directory.CreateDirectory(path);
    }
}

Task("Paket Restore")
    .Does(() => 
{
    string command = @"if (-not (Test-Path .\.paket\paket.exe)) { .\.paket\paket.bootstrapper.exe }; " +                     @".\.paket\paket.exe restore";
    Information("Running command: {0}", command);
    var settings = new ProcessSettings
    {
        Arguments = string.Format("-NoProfile -Command \"{0}\"", command)
    };
    var exitCodeWithArgument = StartProcess("powershell", settings);
    if (exitCodeWithArgument != 0) {
        throw new Exception("Something bad happened. Exit code: " + exitCodeWithArgument);
    }
});


Task("Paket Outdated")
    .IsDependentOn("Paket Restore")
    .Does(() => 
{
    string command = "$ok = $true; " +
                     "./.paket/paket.exe outdated | %{ " +
                     @"$ok = $ok -and !($_ -match 'Outdated packages found:') -or ($_ -match '^\d+ '); "+
                     "if ($ok) { Write-Host $_ } else { Write-Warning $_ } }";
    Information("Running command: {0}", command);
    var settings = new ProcessSettings
    {
        Arguments = string.Format("-NoProfile -Command \"{0}\"", command)
    };
    var exitCodeWithArgument = StartProcess("powershell", settings);
    if (exitCodeWithArgument != 0) {
        throw new Exception("Something bad happened. Exit code: " + exitCodeWithArgument);
    }
});

Task("Version")
    .Does(() =>
{
    var settings = new GitVersionSettings {
        UpdateAssemblyInfo = true
    };
    var result = GitVersion(settings);
    version = result.LegacySemVerPadded;
    Information("LegacySemVerPadded: {0}", version);
});

Task("Build")
    .IsDependentOn("Version")
    .IsDependentOn("Paket Restore")
    .Does(() =>
{
    var settings = new MSBuildSettings {
        Verbosity = Verbosity.Minimal,
        Configuration = configuration
    };
    MSBuild("Unity.Interception.Serilog.sln", settings);
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() => 
{
    EnsureDirExists(outputDir);
    var file = "**/bin/*/*.Tests.dll";
    var settings = new XUnit2Settings {
        Parallelism = ParallelismOption.All,
        HtmlReport = true,
        NoAppDomain = true,
        XmlReport = true,
        OutputDirectory = outputDir
    };
    XUnit2(file, settings);
});

Task("Paket Pack")
    .IsDependentOn("Build")
    .Does(() =>
{
    var command = string.Format("./.paket/paket.exe " +
                  "pack " +
                  "include-referenced-projects " +
                  "minimum-from-lock-file " +
                  "output {0} " +
                  "buildplatform {1} " +
                  "version {2} " + 
                  "buildconfig {3}", 
                  outputDir, buildPlatform, version, configuration);
    Information("Running command: {0}", command);
    var settings = new ProcessSettings
    {
        Arguments = string.Format("-NoProfile -Command \"{0}\"", command)
    };
    var exitCodeWithArgument = StartProcess("powershell", settings);
    if (exitCodeWithArgument != 0) {
        throw new Exception("Something bad happened. Exit code: " + exitCodeWithArgument);
    }
});

Task("Default")
    .IsDependentOn("Paket Pack")
    .IsDependentOn("Test")
    .IsDependentOn("Paket Outdated");
RunTarget(target);