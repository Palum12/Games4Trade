using System;
using System.Collections.Generic;
using System.Linq;
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
        private const int PageSize = 10;

        public AnnouncementService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IList<AnnouncementGetDto>> GetAnnouncements()
        {
            var result = await _unitOfWork.Announcements.GetAnnouncementsWithAuthors();
            var response = _mapper.Map<IEnumerable<Announcement>, IEnumerable<AnnouncementGetDto>>(result)
                .ToList();
            return response;
        }

        public async Task<IList<AnnouncementGetDto>> GetAnnouncementsPage(int page)
        {
            var result = await _unitOfWork.Announcements.GetAnnouncementsPageWithAuthors(page, PageSize);
            var response = _mapper.Map<IEnumerable<Announcement>, IEnumerable<AnnouncementGetDto>>(result);
            return response.ToList();
        }

        public async Task<AnnouncementGetDto> GetAnnouncement(int id)
        {
            var result = await _unitOfWork.Announcements.GetAnnouncementWithAuthor(id);
            return _mapper.Map<Announcement, AnnouncementGetDto>(result);
        }

        public async Task<OperationResult> CreateAnnouncement(AnnouncementSaveDto announcement, string login)
        {
            var currentUser = await _unitOfWork.Users.GetUserByLogin(login);
            var announcementModel = _mapper.Map<AnnouncementSaveDto, Announcement>(announcement);
            announcementModel.UserId = currentUser.Id;
            announcementModel.DateCreated = DateTime.Now;
            await _unitOfWork.Announcements.AddASync(announcementModel);
            var result = await _unitOfWork.CompleteASync();
            if (result > 0)
            {
                return new OperationResult()
                {
                    IsSuccessful = true,
                    Payload = announcementModel
                };
            }
            else
            {
                return OtherServices.GetIncorrectDatabaseConnectionResult();
            }
        }

        public async Task<OperationResult> EditAnnouncement(int id, AnnouncementSaveDto announcement)
        {
            var announcementInDb = await _unitOfWork.Announcements.GetASync(id);
            if (announcementInDb == null)
            {
                return new OperationResult()
                {
                    IsSuccessful = false,
                    IsClientError = true
                };
            }

            if (announcementInDb.Title.Equals(announcement.Title) &&
                announcementInDb.Content.Equals(announcement.Content))
            {
                return new OperationResult()
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Message = "Nothing has changed operation aborted",
                    Payload = announcement
                };
            }

            announcementInDb.Content = announcement.Content;
            announcementInDb.Title = announcement.Title;

            var result = await _unitOfWork.CompleteASync();
            if (result > 0)
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

        public async Task<OperationResult> DeleteAnnouncement(int id)
        {
            var entity = await _unitOfWork.Announcements.GetASync(id);
            if (entity == null)
            {
                return new OperationResult()
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Message = "Announcement was not found"
                };
            }
            _unitOfWork.Announcements.Remove(entity);
            var result = await _unitOfWork.CompleteASync();
            if (result > 0)
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
    }
}
