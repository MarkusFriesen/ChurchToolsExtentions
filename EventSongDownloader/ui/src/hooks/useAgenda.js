import React from "react";
import axios from "axios";

const client = axios.create();

export default function useAgenda(eventId) {

  const [agenda, setAgenda] = React.useState([]);
  const [loading, setLoading] = React.useState(false);
  const [plannedSongs, setPlannedSongs] = React.useState([]);
  const [loadingEventId, setLoadingEventId] = React.useState("");

  React.useEffect(() => {
    if (!eventId) return
    if (eventId === loadingEventId) return;
    setLoadingEventId(eventId);
    async function getData() {
      setLoading(true)
      setAgenda([])
      setPlannedSongs([])
      var result = await client.get(`api/events/${eventId}/agenda`);
      setAgenda(result.data)
      setPlannedSongs(result?.data?.items?.filter(i => i.type === "song"))
      setLoading(false)
    }

    getData();
  }, [eventId, loadingEventId])

  return {agenda, plannedSongs, loading}
}