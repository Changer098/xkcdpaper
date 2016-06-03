# xkcdpaper
Sets the current XKCD comic as a windows wallpaper

## Thanks
Basically ripping off Neil N's [answer](http://stackoverflow.com/a/1061682) on StackOverflow. So thanks to Neil!

## Requirements
1. Newtonsoft Json.Net : [Nuget](http://www.nuget.org/packages/Newtonsoft.Json)
2. .NET 4 Framework (Probably works with older versions, but was tested with Framework 4)

## Build
There is no visual studio solution, nor is there a batch file for building the project (TODO), so you have to compile csc.
The basic command is `csc.exe /r:[Json.Net DLL Location] /out:xkcdpaper.exe xkcdpaper.cs`

## TODO
1. Create a build script ||
2. Create a visual studio project (CLI compile with msbuild.exe)
3. Allow store in Temp folder (currently stored where the executable is)
4. Wait for internet access before starting option
5. Picture formatting (Add title to picture and comic number)
6. Comment all the things
