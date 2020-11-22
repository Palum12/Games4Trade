using System;
using Games4TradeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Games4TradeAPI.Data
{
    public interface IApplicationContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Announcement> Announcements { get; set; }
        DbSet<ObservedUsersRelationship> ObservedUsersRelationship { get; set; }
        DbSet<Genre> Genres { get; set; }
        DbSet<UserLikedGenre> UserGenreRelationship { get; set; }
        DbSet<Models.System> Systems { get; set; }
        DbSet<UserOwnedSystem> UserSystemRelationship { get; set; }
        DbSet<Region> Regions { get; set; }
        DbSet<State> States { get; set; }
        DbSet<Photo> Photos { get; set; }
        DbSet<Advertisement> Advertisements { get; set; }
        DbSet<AdvertisementItem> AdvertisementItems { get; set; }
        DbSet<Models.Console> Consoles { get; set; }
        DbSet<Game> Games { get; set; }
        DbSet<Accessory> Accessories { get; set; }
        DbSet<Message> Messages { get; set; }
    }
}
