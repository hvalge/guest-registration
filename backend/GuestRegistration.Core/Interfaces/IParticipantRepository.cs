using GuestRegistration.Core.Entities;

namespace GuestRegistration.Core.Interfaces
{
    public interface IParticipantRepository
    {
        Task AddAsync(Participant participant);
        Task<List<PaymentMethod>> GetPaymentMethodsAsync();
    }
}