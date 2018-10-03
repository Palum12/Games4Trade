using System.Collections.Generic;
using System.Threading.Tasks;
using Games4Trade.Dtos;
using Games4Trade.Models;

namespace Games4Trade.Services
{
    public interface IGenreService
    {
        Task<IList<GenreGetDto>> GetGenres();
        Task<IList<GenreGetDto>> GetGenresForUser(int userId);
        Task<OperationResult> CreateGenre(GenreCreateOrUpdateDto genre);
        Task<OperationResult> EditGenre(int id, GenreCreateOrUpdateDto genre);
        Task<OperationResult> DeleteGenre(int id);      
    }
}
