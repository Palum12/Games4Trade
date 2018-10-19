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
        public IAnnouncementReposiotry Announcements { get; private set; }
        public IRepository<Photo> Photos { get; private set; }
        public IMessageRepository Messages { get; private set; }
        public IAdvertisementReposiotry Advertisements { get; private set; }
        public Repository<Console>Consoles { get; private set; }
        public Repository<Game> Games { get; private set; }
        public Repository<Accessory> Accessories { get; private set; }
        public IRepository<Region> Regions { get; private set; }
        public IRepository<State> States { get; private set; }

        public UnitOfWork(ApplicationContext context)
        {
            _context = context;
            Users = new UserRepository(context);
            Genres = new GenreRepository(context);
            Systems = new SystemRepository(context);
            Announcements = new AnnouncementRepository(context);
            Photos = new Repository<Photo>(context);
            Messages = new MessageRepository(context);
            Advertisements = new AdvertisementRepository(context);
            Regions = new Repository<Region>(context);
            States = new Repository<State>(context);
            Games = new Repository<Game>(context);
            Consoles = new Repository<Console>(context);
            Accessories = new Repository<Accessory>(context);
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
