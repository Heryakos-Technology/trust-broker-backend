using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using broker.Models;
using Microsoft.EntityFrameworkCore;

namespace broker.Data
{
    public interface IBrokerRepository : IRepository<Broker>
    {
        Task<Broker> GetBrokerByEmailAsync(string email); 
        Task<bool> UpdateApprovedStatusAsync(int brokerId, bool approved);
    }
}