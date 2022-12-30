import * as React from 'react';
import List from '@mui/material/List';
import Button from '@mui/material/Button';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import ListSubheader from '@mui/material/ListSubheader';
import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import Checkbox from '@mui/material/Checkbox';
import useSongs from '../hooks/useSongs';
import {useSearchParams} from 'react-router-dom';
import useAgenda from '../hooks/useAgenda';
import Spinner from './Spinner'

export default function SongSelector() {
  const [checked, setChecked] = React.useState([0]);
  const [downloading, setLoading] = React.useState(false);

  let [searchParams] = useSearchParams();

  const event = searchParams.get("event")
  const {agenda, plannedSongs} = useAgenda(event);
  const {files, mergeAndDownloadFiles, loading} = useSongs(event, plannedSongs);

  const handleToggle = React.useCallback((value) => () => {
    const currentIndex = checked.indexOf(value);
    const newChecked = [...checked];

    if (currentIndex === -1) {
      newChecked.push(value);
    } else {
      newChecked.splice(currentIndex, 1);
    }

    setChecked(newChecked);
  }, [checked]);

  const download = React.useCallback(async () => {
    setLoading(true)
    await mergeAndDownloadFiles(checked, `${agenda.name}.pdf`);
    setLoading(false)
  }, [agenda.name, checked, mergeAndDownloadFiles])

  if (loading) return <Spinner />
  return (
    <>
      <List subheader={
        <ListSubheader component="div" id="nested-list-subheader">
          {agenda.name}
        </ListSubheader>
      }>
        {files?.map(file => {
          const labelId = `checkbox-list-label-${file.filename}`;

          return (
            <ListItem
              key={file?.filename}
              disablePadding
            >
              <ListItemButton role={undefined} onClick={handleToggle(file.filename)} dense>
                <ListItemIcon>
                  <Checkbox
                    edge="start"
                    checked={checked.indexOf(file.filename) !== -1}
                    tabIndex={-1}
                    disableRipple
                    inputProps={{'aria-labelledby': labelId}}
                  />
                </ListItemIcon>
                <ListItemText id={labelId} primary={file.name} secondary={file.songName} />
              </ListItemButton>
            </ListItem>
          );
        })}
      </List>

      <Box sx={{m: 1, position: 'relative', width: 'fit-content'}}>
        <Button disabled={downloading || files?.length < 1} variant='outlined' onClick={download}> Download</Button>
        {downloading && (
          <CircularProgress
            size={24}
            sx={{
              position: 'absolute',
              top: '50%',
              left: '50%',
              marginTop: '-12px',
              marginLeft: '-12px',
            }}
          />
        )}
      </Box>
    </>
  );
}