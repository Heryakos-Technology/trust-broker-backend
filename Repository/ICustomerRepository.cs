using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks;
using broker.Models;
namespace broker.Data
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer> GetCustomerByEmailAsync(string email); 
    }
}