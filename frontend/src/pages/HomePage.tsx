import { useState, useEffect } from 'react';
import { getEvents } from '../services/eventService';
import type { EventSummary } from '../types/event';
import logger from '../services/logger';
import HeroSection from '../components/HeroSection';
import EventList from '../components/EventList';

const HomePage = () => {
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
        logger.info('Successfully loaded events.', { counts: { future: future.length, past: past.length } });
      } catch (err) {
        setError('Failed to load events. Please try again later.');
        logger.error('Failed to load events.', err);
      } finally {
        setIsLoading(false);
      }
    };
    loadEvents();
  }, []);

  const handleEventDeleted = (id: number) => {
    setFutureEvents(futureEvents.filter(event => event.id !== id));
  };

  return (
    <main className="container my-4">
      <HeroSection />

      {isLoading && <p>Loading events...</p>}
      {error && <div className="alert alert-danger">{error}</div>}
      
      {!isLoading && !error && (
        <div className="row">
          <div className="col-lg-6 mb-4">
            <EventList 
              title="Tulevased üritused" 
              events={futureEvents} 
              showDelete={true} 
              onEventDeleted={handleEventDeleted} 
            />
          </div>
          <div className="col-lg-6 mb-4">
            <EventList title="Toimunud üritused" events={pastEvents} />
          </div>
        </div>
      )}
    </main>
  );
};

export default HomePage;
