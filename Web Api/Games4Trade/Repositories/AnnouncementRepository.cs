﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Games4TradeAPI.Data;
using Games4TradeAPI.Models;
using Microsoft.EntityFrameworkCore;
using Games4TradeAPI.Interfaces.Repositories;

namespace Games4TradeAPI.Repositories
{
    public class AnnouncementRepository : Repository<Announcement>, IAnnouncementRepository
    {
        public AnnouncementRepository(ApplicationContext context) : base(context) {}

        public async Task<Announcement> GetAnnouncementWithAuthor(int id, bool isAdmin)
        {
            return await Context.Announcements.Include(a => a.User)
                .SingleOrDefaultAsync(a => a.Id == id && (isAdmin || a.IsActive));
        }

        public async Task<IEnumerable<Announcement>> GetAnnouncementsPageWithAuthors(int page, int pageSize, bool isAdmin)
        {
            var skip = page * pageSize;
            return await Context.Announcements
                .Where(a => isAdmin || a.IsActive)
                .OrderByDescending(a => a.DateCreated)
                .Skip(skip).Take(pageSize)
                .Include(a => a.User).ToListAsync();
        }
    }
}
