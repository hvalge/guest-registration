import axios from 'axios';
import type { PaymentMethod } from '../types/paymentMethod';
import type { ParticipantDetail } from '../types/participant';

const API_URL = '/api';

export interface CreateParticipantData {
  type: 'NaturalPerson' | 'LegalPerson';
  firstName?: string;
  lastName?: string;
  idCode?: string;
  companyName?: string;
  registerCode?: string;
  numberOfAttendees?: number;
  paymentMethodId: number;
  additionalInformationNatural?: string;
  additionalInformationLegal?: string;
}

export const getPaymentMethods = async (): Promise<PaymentMethod[]> => {
  const response = await axios.get<PaymentMethod[]>(`${API_URL}/payment-methods`);
  return response.data;
};

export const addParticipant = async (eventId: number, data: CreateParticipantData): Promise<void> => {
  await axios.post(`${API_URL}/events/${eventId}/participants`, data);
};

export const getParticipantDetails = async (eventId: number, participantId: number): Promise<ParticipantDetail> => {
    const response = await axios.get<ParticipantDetail>(`${API_URL}/events/${eventId}/participants/${participantId}`);
    return response.data;
};

export const updateParticipant = async (eventId: number, participantId: number, data: Partial<ParticipantDetail>): Promise<void> => {
    await axios.put(`${API_URL}/events/${eventId}/participants/${participantId}`, data);
};

