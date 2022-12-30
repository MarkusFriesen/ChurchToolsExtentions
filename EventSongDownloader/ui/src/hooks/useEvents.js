import React from "react";
import axios from "axios";

const client = axios.create();

function toQueryFormat(date){
  return date.getFullYear()+ '-' + (date.getMonth() + 1);
}

export default function useEvents(date) {

  const [events, setEvents] = React.useState([]);
  const [loading, setLoading] = React.useState(false);
  const [daysWithEvents, setDaysWithEvents] = React.useState([]);
  const [loadingDate, setLoadingDate] = React.useState("");

  React.useEffect(() => {
    if (!date) return
    const queryDate = toQueryFormat(date);
    if (queryDate === loadingDate) return;
    setLoadingDate(queryDate);
    async function getData() {
      setLoading(true)
      var result = await client.get(`api/events/month?month=${queryDate}`);
      
      setEvents(result.data)
      const allDays = new Set(result.data.map(d => new Date(d.startDate).getDate()))
      setDaysWithEvents([...allDays])
      setLoading(false)
    }

    getData();
  }, [date, loadingDate])

  return {events, daysWithEvents, loading }
}