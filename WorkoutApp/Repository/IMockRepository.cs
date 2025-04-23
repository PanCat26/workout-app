using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutApp.Models;

namespace WorkoutApp.Repository
{
    public interface IMockRepository
    {
        IEnumerable<MockModel> GetAll();
        void Add(MockModel model);
        void Save();
    }
}
