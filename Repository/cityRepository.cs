using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using broker.Models;
using Microsoft.EntityFrameworkCore;

namespace broker.Data
{
    public class CityRepository : IRepository<City>
    {
        private readonly DataContext _context;
        public CityRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteData(City city)
        {
              Console.WriteLine("Delete city method invoked");
            _context.Cities.Remove(city);
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

        public async Task<List<City>> GetData()
        {   
            Console.WriteLine("Get  city method invoked");
             var model = await _context.Cities.ToListAsync();
            return model;
        }

        public async Task<City> GetDataById(int id)
        {
            return await _context.Cities.FirstOrDefaultAsync(x => x.CityId== id);
        }

        public Task<List<Category>> GetPaginatedData(int pageNumber, int pageSize, string orderBy, string search)
        {
            throw new System.NotImplementedException();
        }

        public Task<int> GetTotalPage(int pageSize, string search)
        {
            throw new System.NotImplementedException();
        }

        public async Task<City> InsertData(City city)
        {
            
            Console.WriteLine("Create city  data  method invoked");
            _context.Cities.Add(city);

            await _context.SaveChangesAsync();
            return city;
        }

        public async Task<City> UpdateData(City city)
        {
            _context.Update(city).Property(x => x.CityId).IsModified = false;
            await _context.SaveChangesAsync();
            return city;
        }

        Task<User> IRepository<City>.GetByEmail(string email)
        {
            throw new NotImplementedException();
        }

        Task<List<City>> IRepository<City>.GetPaginatedData(int pageNumber, int pageSize, string orderBy, string search)
        {
            throw new NotImplementedException();
        }
    }
}
