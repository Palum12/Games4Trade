using Games4TradeAPI.Common;
using Games4TradeAPI.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Games4TradeAPI.Interfaces.Repositories
{
    public interface IPhotoRepository
    {
        public Task<Photo> AddAsync(Photo photo, IFormFile file);
        public Task<IEnumerable<Photo>> AddRangeAsync(IEnumerable<(Photo, IFormFile)> files);
        public Task<Photo> GetAsync(int id, ImageDownloadFormat downloadFormat = ImageDownloadFormat.FullSized);
        public Task<Photo> GetDefaultUserPhoto();
        public Task<Photo> GetDefaultAdPhoto();
        public Task<Photo> FirstOrDefaultAsync(Predicate<Photo> predicate, ImageDownloadFormat downloadFormat = ImageDownloadFormat.FullSized);
        public Task<IEnumerable<Photo>> FindAsync(Predicate<Photo> predicate, ImageDownloadFormat downloadFormat = ImageDownloadFormat.FullSized);
        public Task RemoveAsync(int id);
        public Task RemoveAsync(Photo photo);
        public Task RemoveRangeAsync(IEnumerable<Photo> photo);
        public Task<int> SaveChangesAsync();
    }
}
