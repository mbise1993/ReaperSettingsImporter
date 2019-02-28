# ReaperSettingsImporter
Automated import of REAPER project settings (track pan, volume, and FX) from a source project to n destination projects.

## Description
This is a simple command line application written in C#. Its primary use is copying track settings (FX chain, volume, pan, and mute/solo) from one source project to multiple destination projects,
such as from an album template project to all the projects for individual tracks on the album. The tracks in the source and destination
projects must be named EXACTLY the same (it is case sensitive) and there should be no duplicate track names. 
You can provide a list of track names for the tracks you would like to import or you can import all the tracks by simply not providing 
any track names. Use "MASTER" to import the master track FX chain. 

## Usage:
Import settings for specific tracks:
```
ReaperSettingsImporterCLI.exe "C:\Path\To\SourceProject.RPP" -d "C:\Path\To\DestProject1.RPP" "C:\Path\To\DestProject2.RPP" -t "TrackName1" "TrackName2"
```

Import settings for all tracks:
```
ReaperSettingsImporterCLI.exe "C:\Recording\Project1.RPP" -d "C:\User\DestProject.RPP" "C:\User\Project2.RPP"
```

Import settings for MASTER, Gtrs, and Bass:
```
ReaperSettingsImporterCLI.exe "C:\Recording\Project1.RPP" -d "C:\User\DestProject.RPP" "C:\User\Project2.RPP" -t "MASTER" "Gtrs" "Bass"
```

There is no limit on the number of destination projects. I highly recommend making a backup of any project that you plan to use as a destination project before running this. Although it has worked
fine for me, it hasn't been extensively tested. All the 
testing I have performed has been with RPP files created with REAPER version 5.28.
