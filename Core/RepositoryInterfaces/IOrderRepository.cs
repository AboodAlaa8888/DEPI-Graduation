using Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.RepositoryInterfaces
{
    public interface IOrderRepository
    {
        // Existing Synchronous methods
        List<Order> GetAll();
        List<Order> GetOrdersByNurseId(int nurseId);
        List<Order> GetOrdersByPatientId(string patientId);
        Order GetById(int id);
        void Add(Order order);
        void Update(Order order);
        void Delete(Order order);
        void AcceptOrder(Order order);
        void CancelOrder(Order order);
        Order? IsConflict(Order order);

        // --- NEW ASYNC METHODS (Required for Controllers) ---
        Task<Order> GetByIdAsync(int id);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
    }
}