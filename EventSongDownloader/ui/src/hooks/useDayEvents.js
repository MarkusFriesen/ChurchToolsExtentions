import React from "react";
import axios from "axios";

const client = axios.create();

function toQueryFormat(date){
  return date.getFullYear()+ '-' + (date.getMonth() + 1) + '-' + (date.getDate());
}

export default function useDayEvents(date) {

  const [events, setEvents] = React.useState([]);
  const [loading, setLoading] = React.useState(false);
  const [loadingDate, setLoadingDate] = React.useState("");

  React.useEffect(() => {
    if (!date) return
    const queryDate = toQueryFormat(date);
    if (queryDate === loadingDate) return;
    setLoadingDate(queryDate);
    async function getData() {
      setLoading(true)
      setEvents([])
      var result = await client.get(`api/events/day?day=${queryDate}`);
      setEvents(result.data)
      setLoading(false)
    }

    getData();
  }, [date, loadingDate])

  return {events, loading}
}