import { useQuery } from '@tanstack/react-query';
import { getEvents } from '../services/eventService';
import HeroSection from '../components/HeroSection';
import EventList from '../components/EventList';

const HomePage = () => {
  const { data: futureEvents = [], isLoading: isLoadingFuture, error: errorFuture } = useQuery({
    queryKey: ['events', 'future'],
    queryFn: () => getEvents('future'),
  });

  const { data: pastEvents = [], isLoading: isLoadingPast, error: errorPast } = useQuery({
    queryKey: ['events', 'past'],
    queryFn: () => getEvents('past'),
  });


  const isLoading = isLoadingFuture || isLoadingPast;
  const error = errorFuture || errorPast;


  return (
    <main className="container my-4">
      <HeroSection />

      {isLoading && <p>Loading events...</p>}
      {error && <div className="alert alert-danger">Failed to load events. Please try again later.</div>}
      
      {!isLoading && !error && (
        <div className="row">
          <div className="col-lg-6 mb-4">
            <EventList 
              title="Tulevased üritused" 
              events={futureEvents} 
              showDelete={true} 
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