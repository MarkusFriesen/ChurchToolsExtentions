import React from "react";
import axios from "axios";
import savePdf from "../utils/saveFile";

const client = axios.create();

export default function useSongs(eventId, plannedSongs) {

  const [songs, setSongs] = React.useState([]);
  const [loading, setLoading] = React.useState(false);
  const [files, setFiles] = React.useState([]);
  const [loadingEventId, setLoadingEventId] = React.useState("");

  React.useEffect(() => {
    if (!eventId) return
    if (eventId === loadingEventId) return;
    setLoadingEventId(eventId);
    async function getData() {
      setSongs([])
      setLoading(true)
      var result = await client.get(`api/events/${eventId}/songs`);
      setSongs(result.data)
      setLoading(false)
    }

    getData();
  }, [eventId, loadingEventId, plannedSongs])

  React.useEffect(() => {
    const newFiels = []
    plannedSongs.forEach(s => {
      var song = songs.find(f => f.id === s.song?.songId)
      if (!song) return;
      var arrangement = song?.arrangements?.find(a => a.id === s.song?.arrangementId)
      if (!arrangement) return;
      newFiels.push(
        ...arrangement?.files?.filter(f => f.name.endsWith('.pdf'))
          .map(f => ({...f, songName: song.name, songId: song.id})))
    });
    setFiles(newFiels)
  }, [songs, plannedSongs])

  async function mergeAndDownloadFiles(fileNames, downloadedFileName){
    const fileUrls = files.filter(f => fileNames.includes(f.filename)).map(f => f.fileUrl)
    if (fileUrls.length < 1) return "";

    const result = await client.post("api/songs/merge", {
      fileUrls
    })


    if (result.status !== 200) return

    const file = await fetch(`api/songs/download/${result.data}`)

    if (file.status !== 200) return
    const blob = await file.blob();
    savePdf(blob, downloadedFileName)

    await client.delete(`api/songs/download/${result.data}`)
  }

  return {
    songs, files, mergeAndDownloadFiles, loading
  }
}