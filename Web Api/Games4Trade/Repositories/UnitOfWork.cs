using Games4Trade.Data;
using System.Threading.Tasks;
using Games4Trade.Models;

namespace Games4Trade.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext _context;
        public IUserRepository Users { get; private set; }
        public IGenreRepository Genres { get; private set; }
        public ISystemRepository Systems { get; private set; }

        public UnitOfWork(ApplicationContext context)
        {
            _context = context;
            Users = new UserRepository(context);
            Genres = new GenreRepository(context);
            Systems = new SystemRepository(context);
        }

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
