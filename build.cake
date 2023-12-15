//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define solutions.
var solutions = new Dictionary<string, string> {
     { "./ObjectListView.sln", "Any" },
};

// Define directories.
var buildDir = Directory("./build") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
	CleanDirectories("./**/bin");
    CleanDirectories("./**/obj");
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetRestore("./ObjectListView.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    foreach (var solution in solutions)
    {
        DotNetBuild(solution.Key, new DotNetBuildSettings
        {
            Configuration = configuration,
            NoRestore = false,
        });
    }

});

Task("Pack")
    .IsDependentOn("Build")
    .Does(() =>
{
    foreach (var solution in solutions)
    {
        DotNetPack(solution.Key, new DotNetPackSettings
        {
            Configuration = configuration,
        });
    }
});

Task("CopyPackages")
    .IsDependentOn("Pack")
    .Does(() =>
{
    CopyFiles("./src/**/*.nupkg", buildDir);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("CopyPackages");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
