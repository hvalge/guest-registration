import React from 'react';
import { Link } from 'react-router-dom';
import type { EventSummary } from '../types/event';
import { deleteEvent } from '../services/eventService';
import logger from '../services/logger';

interface EventListProps {
  title: string;
  events: EventSummary[];
  showDelete?: boolean;
  onEventDeleted?: (id: number) => void;
}

const EventList: React.FC<EventListProps> = ({ title, events, showDelete = false, onEventDeleted }) => {
  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('et-EE', { day: '2-digit', month: '2-digit', year: 'numeric' });
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('Are you sure you want to delete this event?')) {
      try {
        await deleteEvent(id);
        logger.info(`Event with ID: ${id} deleted.`);
        if (onEventDeleted) {
          onEventDeleted(id);
        }
      } catch (error) {
        logger.error(`Failed to delete event with ID: ${id}`, error);
        alert('Failed to delete event.');
      }
    }
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
      {showDelete && (
        <div className="card-footer bg-white border-0 text-start">
          <Link to="/add-event" className="btn btn-link">LISA ÜRITUS</Link>
        </div>
      )}
    </div>
  );
};

export default EventList;
