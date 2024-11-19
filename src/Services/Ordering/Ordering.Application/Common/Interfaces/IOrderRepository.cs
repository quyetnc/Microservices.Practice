using Contracts.Common.Interfaces;
using Contracts.Domains.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Common.Interfaces
{
    public interface IOrderRepository : IRepositoryBase<Order, long>
    {
        Task<IEnumerable<Order>> GetOrdersByUserNameAsync ( string userName );
        Task<Order> GetOrderByDocumentNo ( string documentNo );
        void CreateOrder ( Order order );
        Task<Order> UpdateOrderAsync ( Order order );
        void DeleteOrder ( Order order );
    }
}
