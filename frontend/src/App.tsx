import { useState, useEffect } from 'react';
import { getEvents } from './services/eventService';
import type { EventSummary } from './types/event';
import logger from './services/logger';
import 'bootstrap/dist/css/bootstrap.min.css';
import './App.css';

import Header from './components/Header';
import HeroSection from './components/HeroSection';
import EventList from './components/EventList';
import Footer from './components/Footer';

function App() {
  const [futureEvents, setFutureEvents] = useState<EventSummary[]>([]);
  const [pastEvents, setPastEvents] = useState<EventSummary[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const loadEvents = async () => {
      logger.info('Attempting to load events...');
      try {
        setIsLoading(true);
        const [future, past] = await Promise.all([
          getEvents('future'),
          getEvents('past'),
        ]);
        setFutureEvents(future);
        setPastEvents(past);
        logger.info({ counts: { future: future.length, past: past.length } }, 'Successfully loaded events.');
      } catch (err) {
        setError('Failed to load events. Please try again later.');
        logger.error(err, 'Failed to load events.');
      } finally {
        setIsLoading(false);
      }
    };
    loadEvents();
  }, []);

  return (
    <>
      <Header />
      <main className="container my-4">
        <HeroSection />

        {isLoading && <p>Loading events...</p>}
        {error && <div className="alert alert-danger">{error}</div>}
        
        {!isLoading && !error && (
          <div className="row">
            <div className="col-lg-6 mb-4">
              <EventList title="Tulevased üritused" events={futureEvents} showDelete={true} />
              <a href="#" className="btn btn-link mt-2">LISA ÜRITUS</a>
            </div>
            <div className="col-lg-6 mb-4">
              <EventList title="Toimunud üritused" events={pastEvents} />
            </div>
          </div>
        )}
      </main>
      <Footer />
    </>
  );
}

export default App;