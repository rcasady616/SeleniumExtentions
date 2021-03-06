#tool "nuget:?package=NUnit.ConsoleRunner"
#tool "nuget:?package=OpenCover"
//#tool coveralls.net
//#tool coveralls.io

//#addin Cake.Coveralls

var configuration="Release";
var solution="./Selenium.WebDriver.Equip.sln";
var testPackageDir="./TestPackage";
var testProjectDir="./Selenium.WebDriver.Equip.Tests/bin/" + configuration;
var projProjectDir="./Selenium.WebDriver.Equip/bin/" + configuration;
var dirNugetPackage="./nuget";
var dirTestResults="./TestResults";
var dirReleaseTesting = "./ReleaseTesting";
var envVars = EnvironmentVariables();
var WhereNotLocalTests = "cat != LocalOnly and cat != HeadLess";

var target = Argument("target", "Default");

Task("Default")
  .IsDependentOn("Build");

Task("Build")
  .Does(() =>
{
  NuGetRestore(solution);
  MSBuild(solution , new MSBuildSettings {
    Verbosity = Verbosity.Minimal,
    ToolVersion = MSBuildToolVersion.VS2015,
    Configuration = configuration,
    PlatformTarget = PlatformTarget.MSIL
    });
    var file = File($"{testProjectDir}/SeleniumSettings.config");
    XmlPoke(file, "/SeleniumSettings/DriverType", "SauceLabs");
    XmlPoke(file, "/SeleniumSettings/BrowserName", "Chrome");
    XmlPoke(file, "/SeleniumSettings/RemoteBrowserName", "Chrome");
    XmlPoke(file, "/SeleniumSettings/RemoteBrowserVersion", "71.0");
});

Task("Test_all")
.Does(()=>{
    if (!DirectoryExists(dirTestResults))
    {
        CreateDirectory(dirTestResults);
    }
    var resultFile = dirTestResults + "/Selenium.WebDriver.Equip.Tests.xml";    
    OpenCover(tool => {
  tool.NUnit3(testProjectDir + "/*.Tests.dll",
  new NUnit3Settings {
    Test = "Selenium.WebDriver.Equip.Tests",
    WorkingDirectory = testProjectDir,
     Results = new[] {new NUnit3Result { FileName = resultFile }}
    });
  },
  new FilePath("./OcResult.xml"),
  new OpenCoverSettings()
    .WithFilter("+[Selenium.WebDriver.Equip]*")
    .WithFilter("+[Selenium.WebDriver]*")
    .WithFilter("+[Equip]*"));
});

Task("Test_BuildServer")
.Does(()=>{
  if (!DirectoryExists(dirTestResults))
    {
        CreateDirectory(dirTestResults);
    }
var resultFile = dirTestResults + "/Selenium.WebDriver.Equip.Tests.xml";    
OpenCover(tool => {
  tool.NUnit3(testProjectDir + "/*.Tests.dll",
  new NUnit3Settings {
    Test = "Selenium.WebDriver.Equip.Tests.Tools,Selenium.WebDriver.Equip.Tests.Elements,Selenium.WebDriver.Equip.Tests.Extensions,Selenium.WebDriver.Equip.Tests.PageNotLoadedExceptionTests",
    Where = WhereNotLocalTests, // this removes all test catagories that cant run on build server
    WorkingDirectory = testProjectDir,
    //OutputFile = resultFile,
    Results = new[] {new NUnit3Result { FileName = resultFile }}
    });
  },
  new FilePath("./OcResult.xml"),
  new OpenCoverSettings()
    .WithFilter("+[Selenium.WebDriver.Equip]*")
    .WithFilter("+[Selenium.WebDriver]*")
    .WithFilter("+[Equip]*"));

if (AppVeyor.IsRunningOnAppVeyor) {
  AppVeyor.UploadTestResults(resultFile, AppVeyorTestResultsType.NUnit3);
}
});

Task("Package")
  .Does(()=>{

    if (!DirectoryExists(dirNugetPackage))
    {
        CreateDirectory(dirNugetPackage);
    }
    
    var assemblyInfo = ParseAssemblyInfo("./Selenium.WebDriver.Equip/Properties/AssemblyInfo.cs");
    var nuGetPackSettings   = new NuGetPackSettings {                              
                                Version                 = assemblyInfo.AssemblyFileVersion,
                                Copyright               = "EQUIP 2016",
                                // ReleaseNotes            = new [] {"Bug fixes", "Issue fixes", "Typos"},
                                OutputDirectory         = "./nuget"
                                };
            
    NuGetPack("./Selenium.WebDriver.Equip/Selenium.WebDriver.Equip.Package.nuspec", nuGetPackSettings);
    NuGetPack("./Equip/Equip.Package.nuspec", nuGetPackSettings);
  });

  Task("LoadCoverage")
  .Does(()=>{
    string key;
    envVars.TryGetValue("COVERALLS_ACCESSKEY", out key);
    //CoverallsIo("./OcResult.xml", new CoverallsIoSettings()
   // {
    //    RepoToken = key
    // });
  });

Task("TestRelease")
  .IsDependentOn("Build")
  .Does(()=>{
    //copy testProjectDir
    if (!DirectoryExists(dirReleaseTesting))
    {
        CreateDirectory(dirReleaseTesting);
    }
    CleanDirectories(dirReleaseTesting);

    var testList = "Selenium.WebDriver.Equip.Tests.Elements,Selenium.WebDriver.Equip.Tests.Extensions,Selenium.WebDriver.Equip.Tests.PageNotLoadedExceptionTests";
    CopyFiles(testProjectDir+"/*",  dirReleaseTesting);
    DeleteFile($"{dirReleaseTesting}/Selenium.WebDriver.Equip.dll");
    DeleteFile($"{dirReleaseTesting}/Selenium.WebDriver.Equip.pdb");
    DeleteFile($"{dirReleaseTesting}/Equip.dll");
    DeleteFile($"{dirReleaseTesting}/Equip.pdb");
    // replace the binaries
    CopyFiles($"{testPackageDir}/*",  dirReleaseTesting);

    var browser = "chrome";
    var temTestDir = $"{dirTestResults}/{browser}";
    if (DirectoryExists(temTestDir))
    {
      CreateDirectory(temTestDir);
    }
    CleanDirectories(temTestDir);
    CopyFiles(dirReleaseTesting+"/*", temTestDir);

     // run the tests chrome
    var file = File($"{temTestDir}/SeleniumSettings.config");
    XmlPoke(file, "/SeleniumSettings/DriverType", "SauceLabs");
    XmlPoke(file, "/SeleniumSettings/BrowserName", browser);
    XmlPoke(file, "/SeleniumSettings/RemoteBrowserName", browser);
    XmlPoke(file, "/SeleniumSettings/RemoteBrowserVersion", "71.0");
   /* NUnit3(temTestDir + "/*.Tests.dll",
      new NUnit3Settings {
      Test = testList,
      WorkingDirectory = temTestDir,
      OutputFile = $"{temTestDir}/Selenium.WebDriver.Equip.Tests.{browser}.54.xml",
      StopOnError = false
    }); */

    browser = "MicrosoftEdge";
    temTestDir = $"{dirTestResults}/{browser}";
    if (!DirectoryExists(temTestDir))
    {
      CreateDirectory(temTestDir);
    }
    CleanDirectories(temTestDir);
    CopyFiles(dirReleaseTesting+"/*", temTestDir);
    file = File(temTestDir +"/Selenium.WebDriver.Equip.Tests.dll.config");
    XmlPoke(file, "/configuration/appSettings/add[@key = 'RemoteBrowserName']/@value", browser);
    XmlPoke(file, "/configuration/appSettings/add[@key = 'RemoteBrowserVersion']/@value", "14.14393");


    browser = "internet explorer";
    temTestDir = $"{dirTestResults}/{browser}";
    if (!DirectoryExists(temTestDir))
    {
      CreateDirectory(temTestDir);
    }
    CleanDirectories(temTestDir);
    CopyFiles(dirReleaseTesting+"/*", temTestDir);
    file = File(temTestDir +"/Selenium.WebDriver.Equip.Tests.dll.config");
    XmlPoke(file, "/configuration/appSettings/add[@key = 'RemoteBrowserName']/@value", browser);
    XmlPoke(file, "/configuration/appSettings/add[@key = 'RemoteBrowserVersion']/@value", "11.103");

    // firefox
    browser = "firefox";
    temTestDir = $"{dirTestResults}/{browser}";
    if (!DirectoryExists(temTestDir))
    {
      CreateDirectory(temTestDir);
    }
    CleanDirectories(temTestDir);
    CopyFiles(dirReleaseTesting+"/*", temTestDir);
    file = File(temTestDir +"/Selenium.WebDriver.Equip.Tests.dll.config");
    XmlPoke(file, "/configuration/appSettings/add[@key = 'RemoteBrowserName']/@value", "Firefox");
    XmlPoke(file, "/configuration/appSettings/add[@key = 'RemoteBrowserVersion']/@value", "64.0");


    // run
    var testAssemblies = GetFiles(dirTestResults + "/*/*.Tests.dll");
    NUnit3(testAssemblies,
      new NUnit3Settings {
      Test = testList,
      //WorkingDirectory = temTestDir,
      OutputFile = $"{dirReleaseTesting}/Selenium.WebDriver.Equip.Tests.xml",
      StopOnError = false
    });
/*
    browser = "internet explorer";
    temTestDir = $"{dirTestResults}/{browser}";
    if (DirectoryExists(temTestDir))
    {
      CreateDirectory(temTestDir);
    }
    CleanDirectories(temTestDir);
    CopyFiles(dirReleaseTesting+"/*", temTestDir);
    // run the tests ie
    file = File(dirReleaseTesting +"/Selenium.WebDriver.Equip.Tests.dll.config");
    XmlPoke(file, "/configuration/appSettings/add[@key = 'RemoteBrowserName']/@value", browser);
    XmlPoke(file, "/configuration/appSettings/add[@key = 'RemoteBrowserVersion']/@value", "11.103");

     // run the tests edge
    file = File(dirReleaseTesting +"/Selenium.WebDriver.Equip.Tests.dll.config");
    XmlPoke(file, "/configuration/appSettings/add[@key = 'RemoteBrowserName']/@value", "MicrosoftEdge");
    XmlPoke(file, "/configuration/appSettings/add[@key = 'RemoteBrowserVersion']/@value", "14.14393");
    NUnit3(dirReleaseTesting + "/*.Tests.dll",
      new NUnit3Settings {
      Test = testList,
      WorkingDirectory = dirReleaseTesting,
      OutputFile = dirTestResults + "/Selenium.WebDriver.Equip.Tests.Edge.14.xml",
      StopOnError = false      
    });

        // run the tests safari
    file = File(dirReleaseTesting +"/Selenium.WebDriver.Equip.Tests.dll.config");
    XmlPoke(file, "/configuration/appSettings/add[@key = 'RemoteBrowserName']/@value", "Safari");
    XmlPoke(file, "/configuration/appSettings/add[@key = 'RemoteBrowserVersion']/@value", "10.0");
    XmlPoke(file, "/configuration/appSettings/add[@key = 'RemoteOsPlatform']/@value", "OS X 10.11");
    NUnit3(dirReleaseTesting + "/*.Tests.dll",
      new NUnit3Settings {
      Test = testList,
      WorkingDirectory = dirReleaseTesting,
      OutputFile = dirTestResults + "/Selenium.WebDriver.Equip.Tests.Mac.Safari.10.xml",
      StopOnError = false      
    });

        // run the tests safari
    file = File(dirReleaseTesting +"/Selenium.WebDriver.Equip.Tests.dll.config");
    XmlPoke(file, "/configuration/appSettings/add[@key = 'RemoteBrowserName']/@value", "Firefox");
    XmlPoke(file, "/configuration/appSettings/add[@key = 'RemoteBrowserVersion']/@value", "45.0");
    XmlPoke(file, "/configuration/appSettings/add[@key = 'RemoteOsPlatform']/@value", "Linux");
    NUnit3(dirReleaseTesting + "/*.Tests.dll",
      new NUnit3Settings {
      Test = testList,
      WorkingDirectory = dirReleaseTesting,
      OutputFile = dirTestResults + "/Selenium.WebDriver.Equip.Tests.Mac.Safari.10.xml",
      StopOnError = false      
    });

     */
});

Task("Clean")
  .Does(()=>{
    CleanDirectories(dirNugetPackage);
    CleanDirectories(dirTestResults);
    CleanDirectories(dirReleaseTesting);    
  });

RunTarget(target);
