using Infrastructure.Data.DbContexts;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Core.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Added this

namespace Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly NursingServicesDbContext _context;

        public OrderRepository(NursingServicesDbContext context)
        {
            _context = context;
        }

        // --- Existing Synchronous Methods ---

        public List<Order> GetAll()
        {
            return _context.Orders.Include(n => n.Nurse).ToList();
        }

        public List<Order> GetOrdersByNurseId(int nurseId)
        {
            return _context.Orders
                  .Include(o => o.Nurse)
                  .Where(o => o.NurseId == nurseId)
                  .OrderByDescending(o => o.OrderDate)
                  .ToList();
        }

        public List<Order> GetOrdersByPatientId(string patientId)
        {
            return _context.Orders
                  .Include(o => o.Nurse)
                  .Where(o => o.PatientId == patientId)
                  .OrderByDescending(o => o.OrderDate)
                  .ToList();
        }

        public Order GetById(int id)
        {
            return _context.Orders.Include(n => n.Nurse).FirstOrDefault(c => c.Id == id);
        }

        public void Add(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
        }

        public void Delete(Order order)
        {
            _context.Orders.Remove(order);
            _context.SaveChanges();
        }

        public void AcceptOrder(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
        }

        public void CancelOrder(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
        }

        public Order? IsConflict(Order order)
        {
            DateTime newStart = order.OrderDate;
            DateTime newEnd = newStart.AddHours(order.Duration);
            Order? flag = null;

            var conflictOrder = _context.Orders
                .FirstOrDefault(o =>
                    (newStart < o.OrderDate.AddHours(o.Duration)) &&
                    (o.OrderDate < newEnd)
                );

            if (conflictOrder != null)
            {
                flag = conflictOrder;
            }

            return flag;
        }

        // --- NEW ASYNC METHODS (Implemented to fix errors) ---

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders.Include(n => n.Nurse).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}