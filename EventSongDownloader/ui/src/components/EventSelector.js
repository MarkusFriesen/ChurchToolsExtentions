import * as React from 'react';
import List from '@mui/material/List';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemText from '@mui/material/ListItemText';
import ListSubheader from '@mui/material/ListSubheader';
import {useSearchParams} from 'react-router-dom';
import {GetParametersAsObject} from './SearchParamsExtentions';
import useDayEvents from '../hooks/useDayEvents';
import Spinner from './Spinner';

export default function EventSelector() {

  let [searchParams, setSearchParams] = useSearchParams();
  const day = searchParams.get("day");
  const event = searchParams.get("event");
  const dayAsDate = day ? new Date(day) : new Date();

  const {events, loading} = useDayEvents(dayAsDate);

  const handleListItemClick = React.useCallback((event, index) => {
    const newParams = GetParametersAsObject(searchParams)
    newParams.event = index;
    setSearchParams(newParams);
  }, [searchParams, setSearchParams]);


  if (loading) return <Spinner />
  
  return (
    <List subheader={
      <ListSubheader component="div" id="nested-list-subheader">
        Events on {dayAsDate.toDateString()}
      </ListSubheader>
    }>
      {
        events.map(e =>
        (<ListItemButton
          key={e.id}
          id={e.id}
          selected={event === e.id?.toString()}
          onClick={(event) => handleListItemClick(event, e.id)}
        >
          <ListItemText primary={e.name} secondary={e.description} />
        </ListItemButton>)
        )
      }
    </List>
  );
}