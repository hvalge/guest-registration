import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getPaymentMethods, addParticipant, type CreateParticipantData } from '../services/participantService';
import type { PaymentMethod } from '../types/paymentMethod';
import logger from '../services/logger';
import { isAxiosError } from 'axios';
import PageBanner from '../components/PageBanner';

const AddParticipantPage: React.FC = () => {
  const { eventId } = useParams<{ eventId: string }>();
  const navigate = useNavigate();

  const [participantType, setParticipantType] = useState<'NaturalPerson' | 'LegalPerson'>('NaturalPerson');
  const [paymentMethods, setPaymentMethods] = useState<PaymentMethod[]>([]);
  const [formData, setFormData] = useState<Partial<CreateParticipantData>>({
    paymentMethodId: 0,
  });
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const loadPaymentMethods = async () => {
      try {
        const methods = await getPaymentMethods();
        setPaymentMethods(methods);
        if (methods.length > 0) {
          setFormData(prev => ({ ...prev, paymentMethodId: methods[0].id }));
        }
      } catch (err) {
        logger.error('Failed to load payment methods', err);
      }
    };
    loadPaymentMethods();
  }, []);

  const handleTypeChange = (type: 'NaturalPerson' | 'LegalPerson') => {
    setParticipantType(type);
    setFormData({ paymentMethodId: formData.paymentMethodId });
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!eventId) return;

    const data: CreateParticipantData = {
        ...formData,
        type: participantType,
        paymentMethodId: Number(formData.paymentMethodId),
        numberOfAttendees: formData.numberOfAttendees ? Number(formData.numberOfAttendees) : undefined,
    };

    try {
      await addParticipant(parseInt(eventId, 10), data);
      logger.info('Participant added successfully');
      navigate(`/events/${eventId}`);
    } catch (err: unknown) {
        if (isAxiosError(err) && err.response) {
            setError(err.response.data.title || 'An unexpected error occurred.');
        } else {
            setError('Failed to add participant. Please try again.');
        }
        logger.error('Failed to add participant', err);
    }
  };

  return (
    <div className="container-lg my-4">
      <PageBanner title="Osavõtja lisamine" />
      <div className="card p-4 border-top-0 mt-4">
        <form onSubmit={handleSubmit}>
          {error && <div className="alert alert-danger">{error}</div>}
          
          <div className="mb-3">
            <div className="form-check form-check-inline">
              <input className="form-check-input" type="radio" name="participantType" id="naturalPerson" value="NaturalPerson" checked={participantType === 'NaturalPerson'} onChange={() => handleTypeChange('NaturalPerson')} />
              <label className="form-check-label" htmlFor="naturalPerson">Eraisik</label>
            </div>
            <div className="form-check form-check-inline">
              <input className="form-check-input" type="radio" name="participantType" id="legalPerson" value="LegalPerson" checked={participantType === 'LegalPerson'} onChange={() => handleTypeChange('LegalPerson')} />
              <label className="form-check-label" htmlFor="legalPerson">Ettevõte</label>
            </div>
          </div>

          {participantType === 'NaturalPerson' ? (
            <>
              <div className="mb-3 row">
                <label className="col-sm-3 col-form-label">Eesnimi:</label>
                <div className="col-sm-9"><input type="text" name="firstName" className="form-control" onChange={handleChange} value={formData.firstName || ''} required /></div>
              </div>
              <div className="mb-3 row">
                <label className="col-sm-3 col-form-label">Perekonnanimi:</label>
                <div className="col-sm-9"><input type="text" name="lastName" className="form-control" onChange={handleChange} value={formData.lastName || ''} required /></div>
              </div>
              <div className="mb-3 row">
                <label className="col-sm-3 col-form-label">Isikukood:</label>
                <div className="col-sm-9"><input type="text" name="idCode" className="form-control" onChange={handleChange} value={formData.idCode || ''} required pattern="[1-6]\d{10}" /></div>
              </div>
            </>
          ) : (
            <>
              <div className="mb-3 row">
                <label className="col-sm-3 col-form-label">Juriidiline nimi:</label>
                <div className="col-sm-9"><input type="text" name="companyName" className="form-control" onChange={handleChange} value={formData.companyName || ''} required /></div>
              </div>
              <div className="mb-3 row">
                <label className="col-sm-3 col-form-label">Registrikood:</label>
                <div className="col-sm-9"><input type="text" name="registerCode" className="form-control" onChange={handleChange} value={formData.registerCode || ''} required pattern="\d{8}" /></div>
              </div>
              <div className="mb-3 row">
                <label className="col-sm-3 col-form-label">Osavõtjate arv:</label>
                <div className="col-sm-9"><input type="number" name="numberOfAttendees" className="form-control" onChange={handleChange} value={formData.numberOfAttendees || ''} min="1" required /></div>
              </div>
            </>
          )}

          <div className="mb-3 row">
            <label className="col-sm-3 col-form-label">Makseviis:</label>
            <div className="col-sm-9">
              <select name="paymentMethodId" className="form-select" onChange={handleChange} value={formData.paymentMethodId}>
                {paymentMethods.map(method => (
                  <option key={method.id} value={method.id}>{method.name}</option>
                ))}
              </select>
            </div>
          </div>
          
          <div className="mb-3 row">
            <label className="col-sm-3 col-form-label">Lisainfo:</label>
            <div className="col-sm-9">
              <textarea 
                name={participantType === 'NaturalPerson' ? 'additionalInformationNatural' : 'additionalInformationLegal'} 
                className="form-control" 
                rows={3} 
                onChange={handleChange}
                value={(participantType === 'NaturalPerson' ? formData.additionalInformationNatural : formData.additionalInformationLegal) || ''}
                maxLength={participantType === 'NaturalPerson' ? 1500 : 5000}
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
