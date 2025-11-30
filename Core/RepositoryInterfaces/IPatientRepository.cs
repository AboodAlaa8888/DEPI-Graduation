using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.RepositoryInterfaces
{
    public interface IPatientRepository
    {
        public IEnumerable<Patient> GetAll();

        public Patient GetById(string id);

        public Patient GetByUserName(string userName);

        public void Add(Patient patient);

        public void Update(Patient patient);

        public void Delete(Patient patient);
    }
}
