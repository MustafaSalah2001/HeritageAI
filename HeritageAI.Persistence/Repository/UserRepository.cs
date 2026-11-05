using HeritageAI.Application.Interfaces;
using HeritageAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeritageAI.Persistence.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetByIdAsync(int id) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        public async Task<List<User>> GetAllAsync() =>
            await _context.Users.ToListAsync();

        public async Task AddAsync(User user) =>
            await _context.Users.AddAsync(user);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}
