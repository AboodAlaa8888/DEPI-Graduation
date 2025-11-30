using Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.RepositoryInterfaces
{
    public interface INurseRepository
    {
        // Existing Synchronous methods
        IEnumerable<Nurse> GetAll(string searchString, string filterGender, int pageNumber, int pageSize, out int totalRecords);
        IEnumerable<Nurse> GetAllForUser(string genderString);
        Nurse GetById(int id);
        Nurse GetByUserName(string userName);
        void Add(Nurse nurse);
        void Update(Nurse nurse);
        void Delete(Nurse nurse);

        // --- NEW ASYNC METHODS (Required for Controllers) ---
        Task<IEnumerable<Nurse>> GetAllAsync();
        Task<Nurse> GetByIdAsync(int id);
    }
}