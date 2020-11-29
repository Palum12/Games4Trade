using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Games4TradeAPI.Data;
using Games4TradeAPI.Interfaces.Repositories;
using Games4TradeAPI.Models;
using Microsoft.EntityFrameworkCore;
using Console = Games4TradeAPI.Models.Console;

namespace Games4TradeAPI.Repositories
{
    public class AdvertisementRepository : Repository<Advertisement>, IAdvertisementReposiotry
    {
        public AdvertisementRepository(ApplicationContext context) : base(context) { }

        public async Task<Advertisement> GetAdvertisementWithDetails(int id, int? userId = null)
        {
            return await Context.Advertisements
                .Where(a => a.IsActive || (userId.HasValue && a.UserId == userId))
                .Include(a => a.Item)
                .Include(a => a.Photos)
                .Include(a => a.User).FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Advertisement>> Get(int page, int pageSize)
        {
            var skip = page * pageSize;
            return await Context.Advertisements
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.DateCreated)
                .Skip(skip).Take(pageSize)
                .Include(a => a.Item).Include(a => a.Photos).ToListAsync();
        }

        public async Task<IEnumerable<Advertisement>> GetQueriedAds(AdQueryOptions options)
        {
            var query = Context.Advertisements.AsQueryable();
            query = query.Where(a => a.IsActive);
            if (options.Systems.Any())
            {
                query = query.Where(a => options.Systems.Contains(a.Item.SystemId));
            }

            if (options.State.HasValue)
            {
                query = query.Where(a => a.Item.StateId == options.State.Value);
            }

            //default sorting
            if (string.IsNullOrEmpty(options.Sort))
            {
                if (options.Desc.HasValue && options.Desc.Value)
                {
                    query = query.OrderByDescending(a => a.DateCreated);
                }
                else
                {
                    query = query.OrderBy(a => a.DateCreated);
                }
            }
            else
            {
                query = query.OrderByPropertyName(options.Sort, !(options.Desc.HasValue && options.Desc.Value));
            }

            if (!string.IsNullOrEmpty(options.Search))
            {
                query = query.Where(a => EF.Functions.ILike(a.Title, "%" + options.Search + "%"));
            }


            if (!string.IsNullOrEmpty(options.Type))
            {
                options.Type = char.ToUpper(options.Type[0]) + options.Type.Substring(1);
                switch (options.Type)
                {
                    case nameof(Game):
                    {
                        query = query.Where(a => a.Item is Game);
                        if (options.Genres.Any())
                        {
                            query = query.Where(a => options.Genres.Contains(((Game)a.Item).GenreId));
                        }

                        if (options.Region.HasValue)
                        {
                            query = query.Where(a => ((Game)a.Item).GameRegionId == options.Region.Value);
                        }
                        break;
                    }
                    case nameof(Accessory):
                    {
                        query = query.Where(a => a.Item is Accessory);
                        break;
                    }
                    case nameof(Console):
                    {
                        query = query.Where(a => a.Item is Console);
                        if (options.Region.HasValue)
                        {
                            query = query.Where(a => ((Console)a.Item).ConsoleRegionId == options.Region.Value);
                        }
                        break;
                    }
                    default:
                    {
                        throw new ArgumentException("No such category!");
                    }
                }
            }

            if (options.Page.HasValue && options.PageSize.HasValue)
            {
                var skip = options.Page.Value * options.PageSize.Value;
                return await query.Skip(skip).Take(options.PageSize.Value).ToListAsync();
            }

            var result = await query.Include(a => a.Photos).ToListAsync();

            return result;
        }

        public async Task<IEnumerable<Advertisement>> GetRecommendedAdvertisements(int userId, int page, int pageSize)
        {
            var genres = await Context.UserGenreRelationship
                .Where(x => x.UserId == userId).Select(x => x.GenreId).ToArrayAsync();
            var systems = await Context.UserSystemRelationship
                .Where(x => x.UserId == userId).Select(x => x.SystemId).ToArrayAsync();
            var observedUsers = await Context.ObservedUsersRelationship
                .Where(u => u.ObservingUserId == userId).Select(x => x.ObservedUserId).ToArrayAsync();

            var skip = page * pageSize;

            var ads = await Context.Advertisements.Include(a => a.Item)
                .Where( a => a.IsActive && a.UserId != userId && (observedUsers.Contains(a.UserId) || systems.Contains(a.Item.SystemId) || 
                            (a.Item is Game && genres.Contains(((Game)a.Item).GenreId)))  )
                .Skip(skip).Take(pageSize)
                .Include(a => a.Photos)
                .OrderByDescending(a => a.DateCreated)
                .ToArrayAsync();

            return ads;
        }

        public async Task<IEnumerable<Advertisement>> GetAdsForUser(int userId, int page, int pageSize, bool skipInactive)
        {
            var skip = page * pageSize;
            return await Context.Advertisements
                .Where(a => a.UserId == userId && (a.IsActive || skipInactive))
                .OrderByDescending(a => a.DateCreated)
                .Skip(skip).Take(pageSize)
                .Include(a => a.Photos)
                .ToListAsync();
        }
    }
}
