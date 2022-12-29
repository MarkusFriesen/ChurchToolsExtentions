import * as React from 'react';
import {LocalizationProvider} from '@mui/x-date-pickers/LocalizationProvider';
import {AdapterDateFns} from '@mui/x-date-pickers/AdapterDateFns';
import {StaticDatePicker} from '@mui/x-date-pickers/StaticDatePicker';
import {PickersDay} from '@mui/x-date-pickers/PickersDay';
import {useSearchParams} from 'react-router-dom';
import {GetParametersAsObject} from './SearchParamsExtentions';
import TextField from '@mui/material/TextField';
import useEvents from '../hooks/useEvents';

export default function Calendar() {

  let [searchParams, setSearchParams] = useSearchParams();
  const [value, setValue] = React.useState(new Date());

  React.useEffect(() => {
    const day = searchParams.get("day");

    if (!day) return;
    const newDay = new Date(day);

    if (newDay.toDateString() === value.toDateString()) return;
    setValue(newDay);
  }, [searchParams, value])

  const { daysWithEvents } = useEvents(value);

  const renderDay = React.useCallback((day, _value, DayComponentProps) => {
    const isSelected = daysWithEvents.indexOf(day.getDay()) > 0;
    return (
      <PickersDay {...DayComponentProps} disabled={!isSelected} />
    );
  }, [daysWithEvents]);

  const onChange = React.useCallback((newValue) => {
    console.info("changeing")
    const newParams = GetParametersAsObject(searchParams)
    newParams.day = newValue.toISOString();
    setSearchParams(newParams);
  }, [searchParams, setSearchParams])

  const renderInput = React.useCallback((params) => <TextField {...params} />, [] );

  return (
    <LocalizationProvider dateAdapter={AdapterDateFns}>
      <StaticDatePicker
        displayStaticWrapperAs="desktop"
        value={value}
        onChange={onChange}
        renderDay={renderDay}
        renderInput={renderInput}
      />

    </LocalizationProvider>
  );
}