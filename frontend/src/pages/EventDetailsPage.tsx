import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getEventDetails, deleteParticipant } from '../services/eventService';
import type { EventDetails as EventDetailsType, Participant } from '../types/event';
import logger from '../services/logger';

const EventDetailsPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [eventDetails, setEventDetails] = useState<EventDetailsType | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (id) {
      const loadEventDetails = async () => {
        try {
          setIsLoading(true);
          const details = await getEventDetails(parseInt(id, 10));
          setEventDetails(details);
          logger.info(`Loaded details for event ID: ${id}`);
        } catch (err) {
          setError('Failed to load event details.');
          logger.error(`Failed to load event details for ID: ${id}`, err);
        } finally {
          setIsLoading(false);
        }
      };
      loadEventDetails();
    }
  }, [id]);

  const handleParticipantDelete = async (participantId: number) => {
    if (id && window.confirm('Are you sure you want to remove this participant?')) {
      try {
        await deleteParticipant(parseInt(id, 10), participantId);
        logger.info(`Participant ${participantId} removed from event ${id}`);
        setEventDetails(prevDetails => {
            if (!prevDetails) return null;
            return {
                ...prevDetails,
                participants: prevDetails.participants.filter(p => p.id !== participantId)
            };
        });
      } catch (error) {
        logger.error(`Failed to remove participant ${participantId} from event ${id}`, error);
        alert('Failed to remove participant.');
      }
    }
  };

  if (isLoading) return <p>Loading...</p>;
  if (error) return <div className="alert alert-danger">{error}</div>;
  if (!eventDetails) return <p>Event not found.</p>;

  return (
    <div className="container my-4">
      <div className="p-4" style={{ 
        backgroundImage: `url('https://rik.ee/sites/default/files/2022-03/header_ylapilt_roheline.jpg')`,
        backgroundSize: 'cover',
        backgroundPosition: 'center'
      }}>
        <h2 className="text-white">{eventDetails.name}</h2>
      </div>
      <div className="card p-4">
        <h3>Osavõtjad</h3>
        <table className="table">
          <thead>
            <tr>
              <th>Nimi</th>
              <th>Kood</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {eventDetails.participants.map((participant: Participant) => (
              <tr key={participant.id}>
                <td>{participant.name}</td>
                <td>{participant.code}</td>
                <td className="text-end">
                  <button onClick={() => handleParticipantDelete(participant.id)} className="btn btn-danger btn-sm">Kustuta</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        <div className="mt-3">
          <button className="btn btn-secondary" onClick={() => navigate('/')}>Tagasi</button>
        </div>
      </div>
    </div>
  );
};

export default EventDetailsPage;
