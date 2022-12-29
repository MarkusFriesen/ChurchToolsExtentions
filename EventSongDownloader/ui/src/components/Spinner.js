import * as React from 'react';
import LinearProgress from '@mui/material/LinearProgress';
import Box from '@mui/material/Box';

export default function Spinner() {
  return (
    <Box sx={{width: '100%', margin: 'auto'}}>
      <LinearProgress />
    </Box>
  );
}