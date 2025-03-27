using System.Collections.Generic;
using System.Threading.Tasks;
using broker.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace broker.Data
{
    public class UserRepository : IRepository<User>
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public UserRepository()
        {
        }

        // public UserRepository()
        // {
        // }

        async Task<bool>  IRepository<User>.DeleteData(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        async Task<List<User>> IRepository<User>.GetData()
        {
            var data = await _context.Users
             .Include(e => e.Buys)
             .ToListAsync();
            return data;
        }

       async Task<User> IRepository<User>.GetDataById(int id)
        {
            return await _context.Users .Include(e => e.Buys).FirstOrDefaultAsync(x => x.UserId == id);
        }
        public async Task<User> GetByPhone(string phone)
        {
            return await _context.Users.Include(e => e.Buys).FirstOrDefaultAsync(u => u.Phone == phone);
        }

        public async Task<User> GetByEmail(string email)
        {
            return await _context.Users .Include(e => e.Buys).FirstOrDefaultAsync(x => x.Email == email);
        }

        async Task<User> IRepository<User>.InsertData(User user)
        {
             _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        async Task<User> IRepository<User>.UpdateData(User user)
        {
             _context.Update(user).Property(x => x.UserId).IsModified = false;
            await _context.SaveChangesAsync();
            return user;
        }
        

//    public async Task<int> GetTotalPage(int pageSize,string search){
//        return 0;
//    }
   public async Task<int> GetTotalPage(int pageSize, string search)
{
    // Calculate the total number of items that match the search criteria
    var totalItems = await _context.Users
                                    .Where(u => u.Phone.Contains(search))  // Assuming search filters by phone (you can modify this logic based on your needs)
                                    .CountAsync();

    // Calculate the total number of pages
    var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

    return totalPages;
}

        public Task<List<User>> GetPaginatedData(int pageNumber, int pageSize, string orderBy, string search)
        {
            throw new System.NotImplementedException();
        }
    }

}
