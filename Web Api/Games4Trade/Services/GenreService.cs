using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Games4Trade.Dtos;
using Games4Trade.Models;
using Games4Trade.Repositories;

namespace Games4Trade.Services
{
    public class GenreService : IGenreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GenreService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IList<GenreDto>> GetGenres()
        {
            var repoResponse = await _unitOfWork.Genres.GetAllASync();
            var genres = _mapper.Map<IEnumerable<Genre>, IEnumerable<GenreDto>>(repoResponse);
            return genres.OrderBy(g => g.Value).ToList();
        }

        public async Task<IList<GenreDto>> GetGenresForUser(int userId)
        {
            var repoResponse = await _unitOfWork.Genres.GetGenresForUser(userId);
            var genres = _mapper.Map<IEnumerable<Genre>, IEnumerable<GenreDto>>(repoResponse);
            return genres.OrderBy(g => g.Value).ToList();
        }

        public async Task<OperationResult> CreateGenre(GenreCreateOrUpdateDto genre)
        {
            var genreModel = _mapper.Map<GenreCreateOrUpdateDto, Genre>(genre);
            var doesExists = await _unitOfWork.Genres.FindASync(g => g.Value.Equals(genreModel.Value, StringComparison.OrdinalIgnoreCase));
            if (doesExists.Any())
            {
                return new OperationResult()
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Payload = doesExists
                };
            }

            await _unitOfWork.Genres.AddASync(genreModel);
            var repoResult = await _unitOfWork.CompleteASync();
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
            var doesExists = await _unitOfWork.Genres.FindASync(g => g.Value.Equals(genre.Value, StringComparison.OrdinalIgnoreCase) && g.Id != id);
            if (doesExists.Any())
            {
                return new OperationResult()
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Payload = doesExists
                };
            }

            var genreInDb = await _unitOfWork.Genres.GetASync(id);
            if (genreInDb != null)
            {
                genreInDb.Value = genre.Value;
                var repoResult = await _unitOfWork.CompleteASync();
                if (repoResult > 0)
                {
                    return new OperationResult()
                    {
                        IsSuccessful = true,
                        Payload = _mapper.Map<Genre, GenreDto>(genreInDb)
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
            var genreInDb = await _unitOfWork.Genres.GetGenreWithGames(id);
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
                _unitOfWork.Genres.Remove(genreInDb);
                var repoResult = await _unitOfWork.CompleteASync();
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
