using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutApp.Models;


namespace WorkoutApp.Repository
{
    public class MockRepository : IMockRepository
    {
        public void Add(MockModel model)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MockModel> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
