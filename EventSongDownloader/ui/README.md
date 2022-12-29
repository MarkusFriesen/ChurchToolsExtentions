The UI project is made with react

# Getting started
1. `npm i`
2. `npm run build`
3. Start EventSongDownloader backend via: `dotnet run` in the parent directory. See parent Readme.
3. `npm run start`

## Deploy 
### On Windows
1. `npm i`
2. `npm run build`
3. `npm run deployW`

### On Linux
1. `npm i`
2. `npm run build`
3. `npm run deploy`

## Description
The UI consists of 3 Components: The calendar picker, the event selector, and the song selector. Each component works independently and gets their state from the query parameters. Once i have more time, i will replace it with a redux store. 