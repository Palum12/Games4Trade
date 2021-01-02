using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Games4TradeAPI.Dtos;
using Games4TradeAPI.Common;
using Games4TradeAPI.Interfaces.Repositories;
using Games4TradeAPI.Interfaces.Services;

namespace Games4TradeAPI.Services
{
    public class SystemService : ISystemService
    {
        private readonly ISystemRepository repository;
        private readonly IMapper mapper;

        public SystemService(ISystemRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<IList<SystemDto>> GetSystems()
        {
            var repoResponse = await repository.GetAllAsync();
            var systems = mapper.Map<IEnumerable<Models.System>, IEnumerable<SystemDto>>(repoResponse);
            return systems.OrderBy(s => s.Manufacturer).ThenByDescending(s=> s.Model).ToList();
        }

        public async Task<IList<SystemDto>> GetSystemsForUser(int userId)
        {
            var repoResponse = await repository.GetSystemsForUser(userId);
            var systems = mapper.Map<IEnumerable<Models.System>, IEnumerable<SystemDto>>(repoResponse);
            return systems.OrderBy(s => s.Manufacturer).ThenByDescending(s => s.Model).ToList();
        }

        public async Task<OperationResult> CreateSystem(SystemCreateOrUpdateDto system)
        {
            var systemModel = mapper.Map<SystemCreateOrUpdateDto, Models.System>(system);
            var doesExists = await repository.GetSameSystem(systemModel);
            if (doesExists != null)
            {
                return new OperationResult()
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Payload = doesExists
                };
            }

            await repository.AddAsync(systemModel);
            var repoResult = await repository.SaveChangesAsync();
            if (repoResult > 0)
            {
                return new OperationResult()
                {
                    IsSuccessful = true,
                    Payload = mapper.Map<Models.System, SystemDto>(systemModel)
                };
            }
            else
            {
                return OtherServices.GetIncorrectDatabaseConnectionResult();
            }
        }

        public async Task<OperationResult> EditSystem(int id, SystemCreateOrUpdateDto system)
        {
            var systemModel = mapper.Map<SystemCreateOrUpdateDto, Models.System>(system);
            var doesExists = await repository.GetSameSystem(systemModel);
            if (doesExists != null && doesExists.Id == id)
            {
                return new OperationResult()
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Payload = doesExists
                };
            }

            var systemInDb = await repository.GetAsync(id);
            if (systemInDb != null)
            {
                systemInDb.Model = system.Model;
                systemInDb.Manufacturer = system.Manufacturer;
                var repoResult = await repository.SaveChangesAsync();
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
                    Message = "Obiekt nie istnieje"
                };
            }
        }

        public async Task<OperationResult> DeleteSystem(int id)
        {
            var systemInDb = await repository.GetSystemWithItems(id);
            if (systemInDb != null)
            {
                if (systemInDb.AdvertisementItems.Any())
                {
                    return new OperationResult()
                    {
                        IsSuccessful = false,
                        IsClientError = true,
                        Message = "Istnieją ogłoszenia związane z tym systemem, usuń je przed usunięciem systemu",
                        Payload = systemInDb.AdvertisementItems
                    };
                }
                repository.Remove(systemInDb);
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
