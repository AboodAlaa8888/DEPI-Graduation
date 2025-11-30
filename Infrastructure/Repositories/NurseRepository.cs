using Infrastructure.Data.DbContexts;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Core.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Added this

namespace Infrastructure.Repositories
{
    public class NurseRepository : INurseRepository
    {
        private readonly NursingServicesDbContext _context;

        public NurseRepository(NursingServicesDbContext context)
        {
            _context = context;
        }

        // --- Existing Synchronous Methods ---

        public IEnumerable<Nurse> GetAll(string searchString, string filterGender, int pageNumber, int pageSize, out int totalRecords)
        {
            var query = _context.Nurses.Include(n => n.Orders).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(n => n.FullName.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(filterGender))
            {
                query = query.Where(p => p.Gender == filterGender);
            }

            totalRecords = query.Count();

            var nurses = query
                .OrderBy(n => n.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return nurses;
        }

        public IEnumerable<Nurse> GetAllForUser(string genderString)
        {
            var nurses = _context.Nurses.Include(n => n.Orders).AsQueryable();

            if (!string.IsNullOrEmpty(genderString) && genderString != "All")
            {
                nurses = nurses.Where(p => p.Gender == genderString);
            }

            return nurses;
        }

        public Nurse GetById(int id)
        {
            return _context.Nurses.Include(n => n.Orders).FirstOrDefault(c => c.Id == id);
        }

        public Nurse GetByUserName(string userName)
        {
            return _context.Nurses.Include(n => n.Orders).FirstOrDefault(n => n.UserName == userName);
        }

        public void Add(Nurse nurse)
        {
            _context.Nurses.Add(nurse);
            _context.SaveChanges();
        }

        public void Update(Nurse nurse)
        {
            _context.Nurses.Update(nurse);
            _context.SaveChanges();
        }

        public void Delete(Nurse nurse)
        {
            _context.Nurses.Remove(nurse);
            _context.SaveChanges();
        }

        // --- NEW ASYNC METHODS (Implemented to fix errors) ---

        public async Task<IEnumerable<Nurse>> GetAllAsync()
        {
            return await _context.Nurses.Include(n => n.Orders).ToListAsync();
        }

        public async Task<Nurse> GetByIdAsync(int id)
        {
            return await _context.Nurses.Include(n => n.Orders).FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}