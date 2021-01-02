using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Models;
using Games4TradeAPI.Common;
using Games4TradeAPI.Interfaces.Repositories;
using Games4TradeAPI.Interfaces.Services;

namespace Games4TradeAPI.Services
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository repository;
        private readonly IMapper mapper;

        public GenreService(IGenreRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<IList<GenreDto>> GetGenres()
        {
            var repoResponse = await repository.GetAllAsync();
            var genres = mapper.Map<IEnumerable<Genre>, IEnumerable<GenreDto>>(repoResponse);
            return genres.OrderBy(g => g.Value).ToList();
        }

        public async Task<IList<GenreDto>> GetGenresForUser(int userId)
        {
            var repoResponse = await repository.GetGenresForUser(userId);
            var genres = mapper.Map<IEnumerable<Genre>, IEnumerable<GenreDto>>(repoResponse);
            return genres.OrderBy(g => g.Value).ToList();
        }

        public async Task<OperationResult> CreateGenre(GenreCreateOrUpdateDto genre)
        {
            var genreModel = mapper.Map<GenreCreateOrUpdateDto, Genre>(genre);
            var doesExists = await repository.FindAsync(g => g.Value == genreModel.Value);
            if (doesExists.Any())
            {
                return new OperationResult()
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Payload = doesExists
                };
            }

            await repository.AddAsync(genreModel);
            var repoResult = await repository.SaveChangesAsync();
            if (repoResult > 0)
            {
                return new OperationResult()
                {
                    IsSuccessful = true,
                    Payload = genreModel
                };
            }
            else
            {
                return OtherServices.GetIncorrectDatabaseConnectionResult();
            }
        }

        public async Task<OperationResult> EditGenre(int id, GenreCreateOrUpdateDto genre)
        {
            var doesExists = await repository.FindAsync(g => g.Value == genre.Value && g.Id != id);
            if (doesExists.Any())
            {
                return new OperationResult()
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Payload = doesExists
                };
            }

            var genreInDb = await repository.GetAsync(id);
            if (genreInDb != null)
            {
                genreInDb.Value = genre.Value;
                var repoResult = await repository.SaveChangesAsync();
                if (repoResult > 0)
                {
                    return new OperationResult()
                    {
                        IsSuccessful = true,
                        Payload = mapper.Map<Genre, GenreDto>(genreInDb)
                    };
                }
                else
                {
                    return OtherServices.GetIncorrectDatabaseConnectionResult();
                }
            }
            else
            {
                return new OperationResult()
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Message = "Obiekt nie istnieje"
                };
            }
        }

        public async Task<OperationResult> DeleteGenre(int id)
        {
            var genreInDb = await repository.GetGenreWithGames(id);
            if (genreInDb != null)
            {
                if (genreInDb.Games.Any())
                {
                    return new OperationResult()
                    {
                        IsSuccessful = false,
                        IsClientError = true,
                        Message = "Istnieją ogłoszenia związane z tym gatunkiem, usuń je przed usunięciem gatunku",
                        Payload = genreInDb.Games
                    };
                }
                repository.Remove(genreInDb);
                var repoResult = await repository.SaveChangesAsync();
                if (repoResult > 0)
                {
                    return new OperationResult()
                    {
                        IsSuccessful = true
                    };
                }
                else
                {
                    return OtherServices.GetIncorrectDatabaseConnectionResult();
                }
            }
            else
            {
                return new OperationResult()
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Message = "Object does not exist in database"
                };
            }
        } 
    }
}
