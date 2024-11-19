using Contracts.Common.Interfaces;
using Customer.API.Persistence;
using Customer.API.Repositories.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace Customer.API.Repositories
{
    public class CustomerRepository : RepositoryQueryBase<Entities.Customer, int, CustomerContext>, ICustomerRepository
    {
        public CustomerRepository ( CustomerContext dbContext ) : base(dbContext)
        {
        }

        public async Task<Entities.Customer> GetCustomerByUserNameAsync ( string username ) => await FindByCondition(x => x.UserName.Equals(username))
            .SingleOrDefaultAsync();

        public async Task<IEnumerable<Entities.Customer>> GetCustomers ( ) => await FindAll().ToListAsync();
    }
}
