import axios from 'axios';
import type { EventSummary } from '../types/event';

const API_URL = '/api/events';

export const getEvents = async (view: 'future' | 'past'): Promise<EventSummary[]> => {
  const response = await axios.get<EventSummary[]>(API_URL, {
    params: { view },
  });
  return response.data;
};