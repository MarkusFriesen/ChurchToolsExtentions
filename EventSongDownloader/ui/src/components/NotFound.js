import * as React from 'react';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';

export default function NotFound() {
  return (
    <Box
      sx={{
        width: '100%',
        height: 300,
      }}
    >
      <Typography variant="h5" gutterBottom>This page doesn't exist.</Typography>
      <Button href="/" variant='outlined'>Go home</Button>
    </Box>
  );
}