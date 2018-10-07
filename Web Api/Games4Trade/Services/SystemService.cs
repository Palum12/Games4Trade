using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Games4Trade.Dtos;
using Games4Trade.Models;
using Games4Trade.Repositories;

namespace Games4Trade.Services
{
    public class SystemService : ISystemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SystemService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IList<SystemDto>> GetSystems()
        {
            var repoResponse = await _unitOfWork.Systems.GetAllASync();
            var systems = _mapper.Map<IEnumerable<Models.System>, IEnumerable<SystemDto>>(repoResponse);
            return systems.OrderBy(s => s.Manufacturer).ThenByDescending(s=> s.Model).ToList();
        }

        public async Task<IList<SystemDto>> GetSystemsForUser(int userId)
        {
            var repoResponse = await _unitOfWork.Systems.GetSystemsForUser(userId);
            var systems = _mapper.Map<IEnumerable<Models.System>, IEnumerable<SystemDto>>(repoResponse);
            return systems.OrderBy(s => s.Manufacturer).ThenByDescending(s => s.Model).ToList();
        }

        public async Task<OperationResult> CreateSystem(SystemCreateOrUpdateDto system)
        {
            var systemModel = _mapper.Map<SystemCreateOrUpdateDto, Models.System>(system);
            var doesExists = await _unitOfWork.Systems.GetSameSystem(systemModel);
            if (doesExists != null)
            {
                return new OperationResult()
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Payload = doesExists
                };
            }

            await _unitOfWork.Systems.AddASync(systemModel);
            var repoResult = await _unitOfWork.CompleteASync();
            if (repoResult > 0)
            {
                return new OperationResult()
                {
                    IsSuccessful = true,
                    Payload = _mapper.Map<Models.System, SystemDto>(systemModel)
                };
            }
            else
            {
                return OtherServices.GetIncorrectDatabaseConnectionResult();
            }
        }

        public async Task<OperationResult> EditSystem(int id, SystemCreateOrUpdateDto system)
        {
            var systemModel = _mapper.Map<SystemCreateOrUpdateDto, Models.System>(system);
            var doesExists = await _unitOfWork.Systems.GetSameSystem(systemModel);
            if (doesExists != null && doesExists.Id == id)
            {
                return new OperationResult()
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Payload = doesExists
                };
            }

            var systemInDb = await _unitOfWork.Systems.GetASync(id);
            if (systemInDb != null)
            {
                systemInDb.Model = system.Model;
                systemInDb.Manufacturer = system.Manufacturer;
                var repoResult = await _unitOfWork.CompleteASync();
                if (repoResult > 0)
                {
                    return new OperationResult()
                    {
                        IsSuccessful = true,
                        Payload = systemInDb
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

        public async Task<OperationResult> DeleteSystem(int id)
        {
            var systemInDb = await _unitOfWork.Systems.GetSystemWithItems(id);
            if (systemInDb != null)
            {
                if (systemInDb.AdvertisementItems.Any())
                {
                    return new OperationResult()
                    {
                        IsSuccessful = false,
                        IsClientError = true,
                        Message = "Genre has connected games with it, please delete them first",
                        Payload = systemInDb.AdvertisementItems
                    };
                }
                _unitOfWork.Systems.Remove(systemInDb);
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
