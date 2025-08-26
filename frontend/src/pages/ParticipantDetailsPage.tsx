import React, { useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { getParticipantDetails, updateParticipant, getPaymentMethods } from '../services/participantService';
import type { ParticipantDetail } from '../types/participant';
import logger from '../services/logger';
import PageBanner from '../components/PageBanner';

const ParticipantDetailsPage: React.FC = () => {
    const { eventId, participantId } = useParams<{ eventId: string, participantId: string }>();
    const navigate = useNavigate();
    const queryClient = useQueryClient();

    const { register, handleSubmit, formState: { errors }, reset, watch } = useForm<ParticipantDetail>();
    const participantType = watch('type');

    const { data: participantDetails, isLoading: isLoadingDetails, error: errorDetails } = useQuery({
        queryKey: ['participant', eventId, participantId],
        queryFn: () => getParticipantDetails(parseInt(eventId!), parseInt(participantId!)),
        enabled: !!eventId && !!participantId,
    });

    const { data: paymentMethods = [], isLoading: isLoadingMethods, error: errorMethods } = useQuery({
        queryKey: ['paymentMethods'],
        queryFn: getPaymentMethods,
    });
    
    useEffect(() => {
        if (participantDetails) {
            reset(participantDetails);
        }
    }, [participantDetails, reset]);

    const mutation = useMutation({
        mutationFn: (updatedParticipant: ParticipantDetail) => updateParticipant(parseInt(eventId!), updatedParticipant.id, updatedParticipant),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['participant', eventId, participantId] });
            queryClient.invalidateQueries({ queryKey: ['event', eventId] });
            navigate(`/events/${eventId}`);
        },
        onError: (error) => {
            logger.error('Update failed', error);
            alert('Failed to update participant.');
        }
    });

    const onSubmit = (data: ParticipantDetail) => {
        mutation.mutate(data);
    };

    const isLoading = isLoadingDetails || isLoadingMethods;
    const error = errorDetails || errorMethods;

    if (isLoading) return <p>Loading...</p>;
    if (error) return <div className="alert alert-danger">Failed to load participant details.</div>;
    if (!participantDetails) return <p>Participant not found.</p>;

    return (
        <div className="container-lg my-4">
            <PageBanner title="Osavõtja info" />
            <div className="card p-4 border-top-0 mt-4">
                <form onSubmit={handleSubmit(onSubmit)}>
                    {participantType === 'NaturalPerson' ? (
                        <>
                            <div className="mb-3 row">
                                <label className="col-sm-3 col-form-label">Eesnimi:</label>
                                <div className="col-sm-9">
                                    <input type="text" {...register('firstName', { required: 'Eesnimi on kohustuslik' })} className={`form-control ${errors.firstName ? 'is-invalid' : ''}`} />
                                    {errors.firstName && <div className="invalid-feedback">{errors.firstName.message}</div>}
                                </div>
                            </div>
                            <div className="mb-3 row">
                                <label className="col-sm-3 col-form-label">Perekonnanimi:</label>
                                <div className="col-sm-9">
                                    <input type="text" {...register('lastName', { required: 'Perekonnanimi on kohustuslik' })} className={`form-control ${errors.lastName ? 'is-invalid' : ''}`} />
                                    {errors.lastName && <div className="invalid-feedback">{errors.lastName.message}</div>}
                                </div>
                            </div>
                            <div className="mb-3 row">
                                <label className="col-sm-3 col-form-label">Isikukood:</label>
                                <div className="col-sm-9">
                                    <input type="text" {...register('idCode', { required: 'Isikukood on kohustuslik', pattern: { value: /^[1-6]\d{10}$/, message: 'Vigane isikukood' } })} className={`form-control ${errors.idCode ? 'is-invalid' : ''}`} />
                                    {errors.idCode && <div className="invalid-feedback">{errors.idCode.message}</div>}
                                </div>
                            </div>
                        </>
                    ) : (
                        <>
                            <div className="mb-3 row">
                                <label className="col-sm-3 col-form-label">Juriidiline nimi:</label>
                                <div className="col-sm-9">
                                    <input type="text" {...register('companyName', { required: 'Juriidiline nimi on kohustuslik' })} className={`form-control ${errors.companyName ? 'is-invalid' : ''}`} />
                                    {errors.companyName && <div className="invalid-feedback">{errors.companyName.message}</div>}
                                </div>
                            </div>
                            <div className="mb-3 row">
                                <label className="col-sm-3 col-form-label">Registrikood:</label>
                                <div className="col-sm-9">
                                    <input type="text" {...register('registerCode', { required: 'Registrikood on kohustuslik', pattern: { value: /^\d{8}$/, message: 'Registrikood peab olema 8 numbrit' } })} className={`form-control ${errors.registerCode ? 'is-invalid' : ''}`} />
                                    {errors.registerCode && <div className="invalid-feedback">{errors.registerCode.message}</div>}
                                </div>
                            </div>
                            <div className="mb-3 row">
                                <label className="col-sm-3 col-form-label">Osavõtjate arv:</label>
                                <div className="col-sm-9">
                                    <input type="number" {...register('numberOfAttendees', { required: 'Osavõtjate arv on kohustuslik', min: { value: 1, message: 'Vähemalt 1 osavõtja' } })} className={`form-control ${errors.numberOfAttendees ? 'is-invalid' : ''}`} />
                                    {errors.numberOfAttendees && <div className="invalid-feedback">{errors.numberOfAttendees.message}</div>}
                                </div>
                            </div>
                        </>
                    )}
                    <div className="mb-3 row">
                        <label className="col-sm-3 col-form-label">Makseviis:</label>
                        <div className="col-sm-9">
                            <select {...register('paymentMethodId')} className="form-select">
                                {paymentMethods.map(m => <option key={m.id} value={m.id}>{m.name}</option>)}
                            </select>
                        </div>
                    </div>
                    <div className="mb-3 row">
                        <label className="col-sm-3 col-form-label">Lisainfo:</label>
                        <div className="col-sm-9">
                            <textarea {...register('additionalInformation')} className="form-control" rows={3}></textarea>
                        </div>
                    </div>
                    <div className="d-flex justify-content-start">
                        <button type="button" className="btn btn-secondary me-2" onClick={() => navigate(`/events/${eventId}`)}>Tagasi</button>
                        <button type="submit" className="btn btn-primary">Salvesta</button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default ParticipantDetailsPage;