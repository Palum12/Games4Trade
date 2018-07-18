using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Games4Trade.Data;
using Games4Trade.Models;
using Microsoft.EntityFrameworkCore;

namespace Games4Trade.Repositories
{
    public class DummyRepository : Repository<Dummy>, IDummyRepository
    {
        public DummyRepository(ApplicationContext context) : base(context)
        {
        }

        public IEnumerable<Dummy> GetDummiesWihtSomething(int a, int b)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Dummy> GetTopDummies(int count)
        {
            throw new NotImplementedException();
            /*return PlutoContext.Courses.Include(bla).OrderBy(2bla);*/
        }

        public ApplicationContext ApplicationContext
         {
             get { return Context as ApplicationContext; }
         }
        
    }
}
