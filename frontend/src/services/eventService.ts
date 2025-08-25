import axios from 'axios';
import type { EventDetails, EventSummary } from '../types/event';

const API_URL = '/api/events';

interface CreateEventData {
  name: string;
  startTime: string;
  location: string;
  additionalInformation?: string;
}

export const getEvents = async (view: 'future' | 'past'): Promise<EventSummary[]> => {
  const response = await axios.get<EventSummary[]>(API_URL, {
    params: { view },
  });
  return response.data;
};

export const getEventDetails = async (id: number): Promise<EventDetails> => {
  const response = await axios.get<EventDetails>(`${API_URL}/${id}`);
  return response.data;
};

export const deleteParticipant = async (eventId: number, participantId: number): Promise<void> => {
  await axios.delete(`${API_URL}/${eventId}/participants/${participantId}`);
};

export const deleteEvent = async (id: number): Promise<void> => {
  await axios.delete(`${API_URL}/${id}`);
};

export const createEvent = async (eventData: CreateEventData): Promise<void> => {
  await axios.post(API_URL, eventData);
};
