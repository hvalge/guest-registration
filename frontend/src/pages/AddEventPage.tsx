import React, { useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { createEvent } from '../services/eventService';
import logger from '../services/logger';
import PageBanner from '../components/PageBanner';

interface EventFormData {
  name: string;
  startTime: string;
  location: string;
  additionalInformation?: string;
}

const AddEventPage: React.FC = () => {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { register, handleSubmit, formState: { errors } } = useForm<EventFormData>();

  const getLocalDateTime = () => {
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    return now.toISOString().slice(0, 16);
  };

  const minDateTime = useMemo(() => getLocalDateTime(), []);

  const mutation = useMutation({
    mutationFn: createEvent,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['events', 'future'] });
      logger.info('Event created successfully');
      navigate('/');
    },
    onError: (err) => {
        logger.error('Failed to create event', err);
        alert('Failed to create event. Please try again.');
    }
  });

  const onSubmit = (data: EventFormData) => {
    mutation.mutate(data);
  };

  return (
    <div className="container my-4">
      <PageBanner title="Ürituse lisamine" />
      <div className="card p-4">
        <form onSubmit={handleSubmit(onSubmit)}>
          <div className="mb-3 row">
            <label htmlFor="name" className="col-sm-3 col-form-label">Ürituse nimi:</label>
            <div className="col-sm-9">
              <input
                type="text"
                className={`form-control ${errors.name ? 'is-invalid' : ''}`}
                id="name"
                {...register('name', { required: 'Ürituse nimi on kohustuslik', maxLength: { value: 100, message: 'Nimi ei tohi olla pikem kui 100 tähemärki' } })}
              />
              {errors.name && <div className="invalid-feedback">{errors.name.message}</div>}
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="startTime" className="col-sm-3 col-form-label">Toimumisaeg:</label>
            <div className="col-sm-9">
              <input
                type="datetime-local"
                className={`form-control ${errors.startTime ? 'is-invalid' : ''}`}
                id="startTime"
                min={minDateTime}
                {...register('startTime', { required: 'Toimumisaeg on kohustuslik' })}
              />
              {errors.startTime && <div className="invalid-feedback">{errors.startTime.message}</div>}
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="location" className="col-sm-3 col-form-label">Koht:</label>
            <div className="col-sm-9">
              <input
                type="text"
                className={`form-control ${errors.location ? 'is-invalid' : ''}`}
                id="location"
                {...register('location', { required: 'Koht on kohustuslik', maxLength: { value: 100, message: 'Asukoht ei tohi olla pikem kui 100 tähemärki' } })}
              />
              {errors.location && <div className="invalid-feedback">{errors.location.message}</div>}
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="additionalInformation" className="col-sm-3 col-form-label">Lisainfo:</label>
            <div className="col-sm-9">
              <textarea
                className="form-control"
                id="additionalInformation"
                rows={3}
                {...register('additionalInformation', { maxLength: { value: 1000, message: 'Lisainfo ei tohi olla pikem kui 1000 tähemärki' } })}
              ></textarea>
               {errors.additionalInformation && <div className="invalid-feedback">{errors.additionalInformation.message}</div>}
            </div>
          </div>
          <div className="d-flex justify-content-start">
            <button type="button" className="btn btn-secondary me-2" onClick={() => navigate('/')}>Tagasi</button>
            <button type="submit" className="btn btn-primary">Lisa</button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddEventPage;