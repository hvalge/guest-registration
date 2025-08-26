import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getParticipantDetails, updateParticipant } from '../services/participantService';
import { getPaymentMethods } from '../services/participantService';
import type { ParticipantDetail } from '../types/participant';
import type { PaymentMethod } from '../types/paymentMethod';
import logger from '../services/logger';
import PageBanner from '../components/PageBanner';

const ParticipantDetailsPage: React.FC = () => {
    const { eventId, participantId } = useParams<{ eventId: string, participantId: string }>();
    const navigate = useNavigate();

    const [participant, setParticipant] = useState<ParticipantDetail | null>(null);
    const [paymentMethods, setPaymentMethods] = useState<PaymentMethod[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (eventId && participantId) {
            const loadDetails = async () => {
                try {
                    setIsLoading(true);
                    const [details, methods] = await Promise.all([
                        getParticipantDetails(parseInt(eventId), parseInt(participantId)),
                        getPaymentMethods()
                    ]);
                    setParticipant(details);
                    setPaymentMethods(methods);
                } catch (err) {
                    setError('Failed to load participant details.');
                    logger.error('Data loading failed', err);
                } finally {
                    setIsLoading(false);
                }
            };
            loadDetails();
        }
    }, [eventId, participantId]);
    
    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
        if (!participant) return;
        const { name, value } = e.target;
        setParticipant({ ...participant, [name]: value });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!eventId || !participant) return;

        try {
            await updateParticipant(parseInt(eventId), participant.id, participant);
            navigate(`/events/${eventId}`);
        } catch (err) {
            setError('Failed to update participant.');
            logger.error('Update failed', err);
        }
    };

    if (isLoading) return <p>Loading...</p>;
    if (error) return <div className="alert alert-danger">{error}</div>;
    if (!participant) return <p>Participant not found.</p>;

    return (
        <div className="container-lg my-4">
            <PageBanner title="Osavõtja info" />
            <div className="card p-4 border-top-0 mt-4">
                <form onSubmit={handleSubmit}>
                    {participant.type === 'NaturalPerson' ? (
                        <>
                            <div className="mb-3 row"><label className="col-sm-3 col-form-label">Eesnimi:</label><div className="col-sm-9"><input type="text" name="firstName" className="form-control" value={participant.firstName} onChange={handleChange} /></div></div>
                            <div className="mb-3 row"><label className="col-sm-3 col-form-label">Perekonnanimi:</label><div className="col-sm-9"><input type="text" name="lastName" className="form-control" value={participant.lastName} onChange={handleChange} /></div></div>
                            <div className="mb-3 row"><label className="col-sm-3 col-form-label">Isikukood:</label><div className="col-sm-9"><input type="text" name="idCode" className="form-control" value={participant.idCode} onChange={handleChange} /></div></div>
                        </>
                    ) : (
                        <>
                            <div className="mb-3 row"><label className="col-sm-3 col-form-label">Juriidiline nimi:</label><div className="col-sm-9"><input type="text" name="companyName" className="form-control" value={participant.companyName} onChange={handleChange} /></div></div>
                            <div className="mb-3 row"><label className="col-sm-3 col-form-label">Registrikood:</label><div className="col-sm-9"><input type="text" name="registerCode" className="form-control" value={participant.registerCode} onChange={handleChange} /></div></div>
                            <div className="mb-3 row"><label className="col-sm-3 col-form-label">Osavõtjate arv:</label><div className="col-sm-9"><input type="number" name="numberOfAttendees" className="form-control" value={participant.numberOfAttendees || ''} onChange={handleChange} /></div></div>
                        </>
                    )}
                    <div className="mb-3 row"><label className="col-sm-3 col-form-label">Makseviis:</label><div className="col-sm-9"><select name="paymentMethodId" className="form-select" value={participant.paymentMethodId} onChange={handleChange}>{paymentMethods.map(m => <option key={m.id} value={m.id}>{m.name}</option>)}</select></div></div>
                    <div className="mb-3 row"><label className="col-sm-3 col-form-label">Lisainfo:</label><div className="col-sm-9"><textarea name="additionalInformation" className="form-control" rows={3} value={participant.additionalInformation || ''} onChange={handleChange}></textarea></div></div>
                    <div className="d-flex justify-content-start"><button type="button" className="btn btn-secondary me-2" onClick={() => navigate(`/events/${eventId}`)}>Tagasi</button><button type="submit" className="btn btn-primary">Salvesta</button></div>
                </form>
            </div>
        </div>
    );
};

export default ParticipantDetailsPage;
