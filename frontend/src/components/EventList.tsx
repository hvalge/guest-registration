import React from 'react';
import type { EventSummary } from '../types/event';

interface EventListProps {
  title: string;
  events: EventSummary[];
  showDelete?: boolean;
}

const EventList: React.FC<EventListProps> = ({ title, events, showDelete = false }) => {
  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('et-EE', { day: '2-digit', month: '2-digit', year: 'numeric' });
  };

  const handleDelete = (id: number) => {
    // TODO: /api/events/:id DELETE call
    alert(`Deleting event with ID: ${id}`);
  };

  return (
    <div className="card border-0">
      <div className="card-header event-list-header">
        <h5 className="mb-0">{title}</h5>
      </div>
      <ul className="list-group list-group-flush">
        {events.length > 0 ? (
          events.map(event => (
            <li key={event.id} className="list-group-item d-flex justify-content-between align-items-center">
              <span>{event.name}</span>
              <div className="d-flex align-items-center">
                <span className="me-4 text-muted">{formatDate(event.startTime)}</span>
                <a href="#" className="event-link me-2">osavõtjad</a>
                {showDelete && <button onClick={() => handleDelete(event.id)} className="delete-button">X</button>}
              </div>
            </li>
          ))
        ) : (
          <li className="list-group-item">Üritusi ei leitud.</li>
        )}
      </ul>
    </div>
  );
};

export default EventList;