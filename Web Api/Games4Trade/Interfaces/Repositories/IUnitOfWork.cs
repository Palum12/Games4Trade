using System;
using System.Threading.Tasks;
using Games4Trade.Models;
using Console = Games4Trade.Models.Console;

namespace Games4Trade.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IGenreRepository Genres { get; }
        ISystemRepository Systems { get; }
        IAnnouncementReposiotry Announcements { get; }
        IRepository<Photo> Photos { get; }
        IMessageRepository Messages { get; }
        IAdvertisementReposiotry Advertisements { get; }
        IRepository<Region> Regions { get; }
        IRepository<State> States { get;}
        IRepository<Console> Consoles { get;}
        IRepository<Game> Games { get;}
        IRepository<Accessory> Accessories { get;}
        IRepository<AdvertisementItem> AdvertisementItems { get; }
        Task<int> CompleteASync();
    }
}