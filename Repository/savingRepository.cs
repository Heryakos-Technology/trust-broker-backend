using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using broker.Models;
using Microsoft.EntityFrameworkCore;

namespace broker.Data
{
    public class SavingRepository : IRepository<Saving>
    {
        private readonly DataContext _context;
        public SavingRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteData(Saving saving)
        {
              Console.WriteLine("Delete city method invoked");
            _context.Savings.Remove(saving);
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<City> GetByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetByPhone(string phone)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Saving>> GetData()
        {   
            Console.WriteLine("Get  city method invoked");
             var model = await _context.Savings.ToListAsync();
            return model;
        }

        public async Task<Saving> GetDataById(int id)
        {
            return await _context.Savings.FirstOrDefaultAsync(x => x.SavingId == id);
        }

        public Task<List<Saving>> GetPaginatedData(int pageNumber, int pageSize, string orderBy, string search)
        {
            throw new System.NotImplementedException();
        }

        public Task<int> GetTotalPage(int pageSize, string search)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Saving> InsertData(Saving saving)
        {
            
            Console.WriteLine("Create city  data  method invoked");
            _context.Savings.Add(saving);

            await _context.SaveChangesAsync();
            return saving;
        }

        public async Task<Saving> UpdateData(Saving saving)
        {
            _context.Update(saving).Property(x => x.SavingId).IsModified = false;
            await _context.SaveChangesAsync();
            return saving;
        }

        Task<User> IRepository<Saving>.GetByEmail(string email)
        {
            throw new NotImplementedException();
        }

        // Task<List<City>> IRepository<Saving>.GetPaginatedData(int pageNumber, int pageSize, string orderBy, string search)
        // {
        //     throw new NotImplementedException();
        // }
    }
}
