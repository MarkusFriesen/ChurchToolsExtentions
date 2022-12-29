# SongExtention
This repository contains a few projects that simplify the handeling of churchtools songs.
It solves the following Problems: 

## Prerequisites
The project is build with dotnet and react.

1. ### DownloadAllSongBeamerFiles: 
  A dotnet cli tool that downloads all songs from your churchtools instance, and saves the last modified date of each file. If the Program is run again, it will only downlod the files that are new or modified since the last run.
  
  Start application
```bash
cd ./DownloadAllSongBeamerFiles

dotnet run . --outDir /temp/test --instance my-instance --username username --password password
```

2. ### EventSongDownloader
  An asp.net webserver, with a react frontend, that allows the user to get a single merged pdf of all songs that are selected for an event. This pdf cen then be imported into ownsong, or a simple pdf viewer for simple seamless naviagation during the worship session.

  Start application
  1. Build and deploy the ui at `EventSongDownloader/ui`. See the [Readme](./EventSongDownloader/ui/README.md).
  2. Edit the `ChurchToolsConnection` properties at [appsettings.Development.json](./EventSongDownloader/appsettings.Development.json)
  4. Set the env variable to `ASPNETCORE_ENVIRONMENT=Development`
  3. Start the application `dotnet run .`  
