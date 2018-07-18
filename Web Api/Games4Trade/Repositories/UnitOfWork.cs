using Games4Trade.Data;
using System.Threading.Tasks;

namespace Games4Trade.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext _context;

        public UnitOfWork(ApplicationContext context)
        {
            _context = context;
            Users = new UserRepository(context);
        }

        public IUserRepository Users { get; private set; }

        public async Task<int> CompleteASync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
