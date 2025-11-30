using Core.Models;
using Core.RepositoryInterfaces;
using Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly NursingServicesDbContext _context;

        public PatientRepository(NursingServicesDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Patient> GetAll()
        {
            return _context.Patients.Include(n => n.Orders).ToList();
        }

        public Patient GetById(string id)
        {
            return _context.Patients.Include(n => n.Orders).FirstOrDefault(c => c.Id == id);
        }

        public Patient GetByUserName(string userName)
        {
            return _context.Patients.Include(n => n.Orders).FirstOrDefault(n => n.UserName == userName);
        }

        public void Add(Patient patient)
        {
            _context.Patients.Add(patient);
            _context.SaveChanges();
        }

        public void Update(Patient patient)
        {
            _context.Patients.Update(patient);
            _context.SaveChanges();
        }

        public void Delete(Patient patient)
        {
            _context.Patients.Remove(patient);
            _context.SaveChanges();
        }
    }
}
