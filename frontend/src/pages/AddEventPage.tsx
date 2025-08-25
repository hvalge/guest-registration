import React, { useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createEvent } from '../services/eventService';
import logger from '../services/logger';

const AddEventPage: React.FC = () => {
  const [name, setName] = useState('');
  const [startTime, setStartTime] = useState('');
  const [location, setLocation] = useState('');
  const [additionalInformation, setAdditionalInformation] = useState('');
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

  const getLocalDateTime = () => {
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    return now.toISOString().slice(0, 16);
  };

  const minDateTime = useMemo(() => getLocalDateTime(), []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!name || !startTime || !location) {
      setError('All fields except additional information are required.');
      return;
    }

    try {
      await createEvent({ name, startTime, location, additionalInformation });
      logger.info('Event created successfully');
      navigate('/');
    } catch (err) {
      setError('Failed to create event. Please try again.');
      logger.error('Failed to create event', err);
    }
  };

  return (
    <div className="container my-4">
      <div className="p-4" style={{ 
        backgroundImage: `url('https://rik.ee/sites/default/files/2022-03/header_ylapilt_roheline.jpg')`,
        backgroundSize: 'cover',
        backgroundPosition: 'center'
      }}>
        <h2 className="text-white">Ürituse lisamine</h2>
      </div>
      <div className="card p-4">
        <form onSubmit={handleSubmit}>
          {error && <div className="alert alert-danger">{error}</div>}
          <div className="mb-3 row">
            <label htmlFor="name" className="col-sm-3 col-form-label">Ürituse nimi:</label>
            <div className="col-sm-9">
              <input
                type="text"
                className="form-control"
                id="name"
                value={name}
                onChange={(e) => setName(e.target.value)}
                maxLength={100}
                required
              />
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="startTime" className="col-sm-3 col-form-label">Toimumisaeg:</label>
            <div className="col-sm-9">
              <input
                type="datetime-local"
                className="form-control"
                id="startTime"
                value={startTime}
                onChange={(e) => setStartTime(e.target.value)}
                min={minDateTime}
                required
              />
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="location" className="col-sm-3 col-form-label">Koht:</label>
            <div className="col-sm-9">
              <input
                type="text"
                className="form-control"
                id="location"
                value={location}
                onChange={(e) => setLocation(e.target.value)}
                maxLength={100}
                required
              />
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="additionalInformation" className="col-sm-3 col-form-label">Lisainfo:</label>
            <div className="col-sm-9">
              <textarea
                className="form-control"
                id="additionalInformation"
                rows={3}
                value={additionalInformation}
                onChange={(e) => setAdditionalInformation(e.target.value)}
                maxLength={1000}
              ></textarea>
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
