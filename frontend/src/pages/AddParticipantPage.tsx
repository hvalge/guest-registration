import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { getPaymentMethods, addParticipant, type CreateParticipantData } from '../services/participantService';
import logger from '../services/logger';
import { isAxiosError } from 'axios';
import PageBanner from '../components/PageBanner';

type FormValues = Omit<CreateParticipantData, 'paymentMethodId' | 'numberOfAttendees'> & {
    paymentMethodId: string;
    numberOfAttendees?: string;
};


const AddParticipantPage: React.FC = () => {
  const { eventId } = useParams<{ eventId: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const { register, handleSubmit, formState: { errors }, watch } = useForm<FormValues>({
    defaultValues: {
        type: 'NaturalPerson'
    }
  });

  const participantType = watch('type');

  const { data: paymentMethods = [], error: paymentMethodsError } = useQuery({
    queryKey: ['paymentMethods'],
    queryFn: getPaymentMethods,
  });

  const mutation = useMutation({
    mutationFn: (data: CreateParticipantData) => addParticipant(parseInt(eventId!, 10), data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['event', eventId] });
      logger.info('Participant added successfully');
      navigate(`/events/${eventId}`);
    },
    onError: (err: unknown) => {
        if (isAxiosError(err) && err.response) {
            alert(err.response.data.title || 'An unexpected error occurred.');
        } else {
            alert('Failed to add participant. Please try again.');
        }
        logger.error('Failed to add participant', err);
    }
  });

  const onSubmit = (data: FormValues) => {
    if (!eventId) return;

    const submissionData: CreateParticipantData = {
        ...data,
        paymentMethodId: Number(data.paymentMethodId),
        numberOfAttendees: data.numberOfAttendees ? Number(data.numberOfAttendees) : undefined,
    };

    mutation.mutate(submissionData);
  };

  return (
    <div className="container-lg my-4">
      <PageBanner title="Osavõtja lisamine" />
      <div className="card p-4 border-top-0 mt-4">
        <form onSubmit={handleSubmit(onSubmit)}>
          {paymentMethodsError && <div className="alert alert-danger">Failed to load payment methods</div>}
          
          <div className="mb-3">
            <div className="form-check form-check-inline">
              <input className="form-check-input" type="radio" id="naturalPerson" {...register('type')} value="NaturalPerson" />
              <label className="form-check-label" htmlFor="naturalPerson">Eraisik</label>
            </div>
            <div className="form-check form-check-inline">
              <input className="form-check-input" type="radio" id="legalPerson" {...register('type')} value="LegalPerson" />
              <label className="form-check-label" htmlFor="legalPerson">Ettevõte</label>
            </div>
          </div>

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
              <select {...register('paymentMethodId', { required: 'Makseviis on kohustuslik' })} className={`form-select ${errors.paymentMethodId ? 'is-invalid' : ''}`}>
                {paymentMethods.map(method => (
                  <option key={method.id} value={method.id}>{method.name}</option>
                ))}
              </select>
              {errors.paymentMethodId && <div className="invalid-feedback">{errors.paymentMethodId.message}</div>}
            </div>
          </div>
          
          <div className="mb-3 row">
            <label className="col-sm-3 col-form-label">Lisainfo:</label>
            <div className="col-sm-9">
              <textarea 
                {...register(participantType === 'NaturalPerson' ? 'additionalInformationNatural' : 'additionalInformationLegal', { maxLength: { value: participantType === 'NaturalPerson' ? 1500 : 5000, message: 'Liiga pikk tekst' } })}
                className="form-control" 
                rows={3} 
              ></textarea>
            </div>
          </div>

          <div className="d-flex justify-content-start">
            <button type="button" className="btn btn-secondary me-2" onClick={() => navigate(eventId ? `/events/${eventId}` : '/')}>Tagasi</button>
            <button type="submit" className="btn btn-primary">Salvesta</button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddParticipantPage;