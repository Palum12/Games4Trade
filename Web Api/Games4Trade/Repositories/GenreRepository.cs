﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Games4Trade.Data;
using Games4Trade.Models;
using Microsoft.EntityFrameworkCore;
using Games4Trade.Interfaces.Repositories;

namespace Games4Trade.Repositories
{
    public class GenreRepository : Repository<Genre>, IGenreRepository
    {
        public GenreRepository(ApplicationContext context) : base(context) { }

        public async Task<Genre> GetGenreWithGames(int id)
        {
            return await Context.Genres.Include(g => g.Games).Where(g => g.Id == id).SingleOrDefaultAsync();
        }

        public async Task<IList<Genre>> GetGenresForUser(int userId)
        {
            var arrayOfIds = await Context.UserGenreRelationship
                .Where(x => x.UserId == userId).Select(x => x.GenreId).ToArrayAsync();
            var genres = await Context.Genres.Where(s => arrayOfIds.Contains(s.Id)).ToListAsync();

            return genres;
        }

    }
}
