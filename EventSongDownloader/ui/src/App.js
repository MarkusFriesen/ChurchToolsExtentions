import Divider from '@mui/material/Divider';
import Calendar from "./components/Calendar";
import EventSelector from "./components/EventSelector";
import SongSelector from "./components/SongSelector";
import Grid from "@mui/material/Grid";

function App() {
  return (
    <Grid container spacing={2}>
      <Grid item xs={12} md={4}>
        <Calendar />
      </Grid>
      <Grid item xs={12} md={8} >
        <EventSelector />
      </Grid>
      <Grid item xs={12}>
        <Divider />
        <SongSelector />
      </Grid>
    </Grid>
  );
}

export default App;
