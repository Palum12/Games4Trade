using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Games4Trade.Dtos;
using Games4Trade.Models;
using Games4Trade.Interfaces.Repositories;
using Games4Trade.Interfaces.Services;


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

        public async Task<AnnouncementGetDto> GetAnnouncement(int id, bool isAdmin)
        {
            var result = await _unitOfWork.Announcements.GetAnnouncementWithAuthor(id, isAdmin);
            return _mapper.Map<Announcement, AnnouncementGetDto>(result);
        }

        public async Task<IList<AnnouncementGetDto>> GetAnnouncementsPage(int page, bool isAdmin)
        {
            var result = await _unitOfWork.Announcements.GetAnnouncementsPageWithAuthors(page, PageSize, isAdmin);
            var response = _mapper.Map<IEnumerable<Announcement>, IEnumerable<AnnouncementGetDto>>(result);
            return response.ToList();
        }

        public async Task<OperationResult> ChangeStatus(int id, AnnouncementArchiveDto value)
        {
            var announcement = await _unitOfWork.Announcements.GetASync(id);
            if (announcement != null)
            {
                announcement.IsActive = value.IsActive;
                var result = await _unitOfWork.CompleteASync();

                return new OperationResult()
                {
                    IsSuccessful = true
                };

            }
            return new OperationResult()
            {
                IsSuccessful = false,
                IsClientError = true
            };
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
