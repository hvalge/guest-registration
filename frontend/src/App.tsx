import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import './App.css';

import Header from './components/Header';
import Footer from './components/Footer';
import HomePage from './pages/HomePage';
import AddEventPage from './pages/AddEventPage';
import EventDetailsPage from './pages/EventDetailsPage';
import AddParticipantPage from './pages/AddParticipantPage';

function App() {
  return (
    <Router>
      <div className="d-flex flex-column min-vh-100">
        <Header />
        <main>
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/add-event" element={<AddEventPage />} />
            <Route path="/events/:id" element={<EventDetailsPage />} />
            <Route path="/events/:eventId/add-participant" element={<AddParticipantPage />} />
          </Routes>
        </main>
        <Footer />
      </div>
    </Router>
  );
}

export default App;
