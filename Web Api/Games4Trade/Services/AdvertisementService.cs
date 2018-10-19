using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Games4Trade.Dtos;
using Games4Trade.Repositories;

namespace Games4Trade.Services
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdvertisementService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private async Task<(bool, string)> CheckIfRelationshipsAreCorrect(AdvertisementSaveDto ad)
        {
            IList<Object> objects;
            if (ad.Discriminator.Equals("Game"))
            {
                var region = await _unitOfWork.Regions.GetASync(ad.RegionId);
                var state = await _unitOfWork.States.GetASync(ad.StateId);
                var genre = await _unitOfWork.Genres.GetASync(ad.GenreId);
                var system = await _unitOfWork.Systems.GetASync(ad.SystemId);
                objects = new List<object> { region, system, state, genre};

                if (objects.Any(o => o == null))
                {
                    var message = objects.Where(o => o == null).Select(o => nameof(o)) + "nie mogą być puste !";
                    return (false, message);
                }
            }

            if (ad.Discriminator.Equals("Console"))
            {
                var region = await _unitOfWork.Regions.GetASync(ad.RegionId);
                var state = await _unitOfWork.States.GetASync(ad.StateId);
                var system = await _unitOfWork.Systems.GetASync(ad.SystemId);
                objects = new List<object> { region, system, state };

                if (objects.Any(o => o == null))
                {
                    var message = objects.Where(o => o == null).Select(o => nameof(o)) + "nie mogą być puste !";
                    return (false, message);
                }
            }

            if (ad.Discriminator.Equals("Accessory"))
            {
                var state = await _unitOfWork.States.GetASync(ad.StateId);
                var system = await _unitOfWork.Systems.GetASync(ad.SystemId);
                objects = new List<object> { system, state };

                if (objects.Any(o => o == null))
                {
                    var message = objects.Where(o => o == null).Select(o => nameof(o)) + "nie mogą być puste !";
                    return (false, message);
                }
            }
            return (true, null);
        }
    }
}
