using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
using ILogger = Serilog.ILogger;
namespace Ordering.Infrastructure.Persistence;

public class OrderContextSeed
{
    private readonly ILogger _logger;
    private readonly OrderContext _context;

    public OrderContextSeed ( OrderContext context, ILogger logger )
    {
        _context = context;
        _logger = logger;
    }
    public async Task InitialiseAsync ( )
    {
        try
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync ( )
    {
        try
        {
            await TrySeedAsync();
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error occurred while initialising the database.");
            throw;
        }
    }
    private async Task TrySeedAsync()
    {
        if (!_context.Orders.Any())
        {
            await _context.Orders.AddRangeAsync(
                new Order
                {
                    UserName = "customer1",
                    FirstName = "Quyet",
                    LastName = "Nguyen",
                    EmailAddress = "quyetnc@local.com",
                    ShippingAddress = "UK",
                    InvoiceAddress = "Australia",
                    TotalPrice = 250
                });
        }
    }
}
