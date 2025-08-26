export interface ParticipantDetail {
  id: number;
  type: 'NaturalPerson' | 'LegalPerson';
  firstName?: string;
  lastName?: string;
  idCode?: string;
  companyName?: string;
  registerCode?: string;
  numberOfAttendees?: number;
  paymentMethodId: number;
  additionalInformation?: string;
}
