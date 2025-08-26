import React from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getEventDetails, deleteParticipant } from '../services/eventService';
import type { Participant } from '../types/event';
import logger from '../services/logger';
import PageBanner from '../components/PageBanner';

const EventDetailsPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const { data: eventDetails, isLoading, error } = useQuery({
    queryKey: ['event', id],
    queryFn: () => getEventDetails(parseInt(id!, 10)),
    enabled: !!id,
  });

  const mutation = useMutation({
    mutationFn: (participantId: number) => deleteParticipant(parseInt(id!, 10), participantId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['event', id] });
    },
    onError: (error) => {
        logger.error(`Failed to remove participant`, error);
        alert('Failed to remove participant.');
    }
  });

  const handleParticipantDelete = (participantId: number) => {
    if (window.confirm('Are you sure you want to remove this participant?')) {
      mutation.mutate(participantId);
    }
  };


  if (isLoading) return <p>Loading...</p>;
  if (error) return <div className="alert alert-danger">Failed to load event details.</div>;
  if (!eventDetails) return <p>Event not found.</p>;

  return (
    <div className="container my-4">
      <PageBanner title="Osavõtjate info" />
      <div className="card p-4 border-top-0 mt-4">
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
                <td>
                  <Link to={`/events/${id}/participants/${participant.id}`}>{participant.name}</Link>
                </td>
                <td>{participant.code}</td>
                <td className="text-end">
                  <button onClick={() => handleParticipantDelete(participant.id)} className="btn btn-danger btn-sm">Kustuta</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        <div className="mt-3 d-flex justify-content-between">
          <button className="btn btn-secondary" onClick={() => navigate('/')}>Tagasi</button>
          <Link to={`/events/${id}/add-participant`} className="btn btn-primary">Lisa osavõtja</Link>
        </div>
      </div>
    </div>
  );
};

export default EventDetailsPage;