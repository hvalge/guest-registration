using GuestRegistration.Core.Entities;
using GuestRegistration.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GuestRegistration.Infrastructure.Persistence.Repositories;

public class PaymentMethodsRepository : IPaymentMethodsRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentMethodsRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<PaymentMethod>> GetPaymentMethodsAsync()
    {
        return await _context.PaymentMethods.ToListAsync();
    }
}