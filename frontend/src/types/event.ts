export interface EventSummary {
  id: number;
  name: string;
  startTime: string;
  location: string;
  participantCount: number;
}

export interface Participant {
  id: number;
  name: string;
  code: string;
}

export interface EventDetails {
  id: number;
  name: string;
  participants: Participant[];
}
