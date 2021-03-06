﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Models;

namespace Games4TradeAPI.Interfaces.Services
{
    public interface IGenreService
    {
        Task<IList<GenreDto>> GetGenres();
        Task<IList<GenreDto>> GetGenresForUser(int userId);
        Task<OperationResult> CreateGenre(GenreCreateOrUpdateDto genre);
        Task<OperationResult> EditGenre(int id, GenreCreateOrUpdateDto genre);
        Task<OperationResult> DeleteGenre(int id);      
    }
}
