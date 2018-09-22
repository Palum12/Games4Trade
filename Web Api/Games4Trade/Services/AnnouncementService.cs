using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Games4Trade.Dtos;
using Games4Trade.Models;
using Games4Trade.Repositories;

namespace Games4Trade.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AnnouncementService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IList<AnnouncementGetListDto>> GetAnnouncements()
        {
            throw new System.NotImplementedException();
        }

        public async Task<AnnouncementGetDetailDto> GetAnnouncement(int id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<OperationResult> CreateAnnouncement(AnnouncementSaveDto announcement)
        {
            throw new System.NotImplementedException();
        }

        public async Task<OperationResult> EditAnnouncement(int id, AnnouncementSaveDto announcement)
        {
            throw new System.NotImplementedException();
        }

        public async Task<OperationResult> DeleteAnnouncement(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
