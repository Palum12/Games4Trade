using System;
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
    public class AnnouncementService : IAnnouncementService
    {
        private readonly IAnnouncementRepository repository;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private const int PageSize = 10;

        public AnnouncementService(IAnnouncementRepository repository, IUserRepository userRepository, IMapper mapper)
        {
            this.repository = repository;
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public async Task<AnnouncementGetDto> GetAnnouncement(int id, bool isAdmin)
        {
            var result = await repository.GetAnnouncementWithAuthor(id, isAdmin);
            return mapper.Map<Announcement, AnnouncementGetDto>(result);
        }

        public async Task<IList<AnnouncementGetDto>> GetAnnouncementsPage(int page, bool isAdmin)
        {
            var result = await repository.GetAnnouncementsPageWithAuthors(page, PageSize, isAdmin);
            var response = mapper.Map<IEnumerable<Announcement>, IEnumerable<AnnouncementGetDto>>(result);
            return response.ToList();
        }

        public async Task<OperationResult> ChangeStatus(int id, AnnouncementArchiveDto value)
        {
            var announcement = await repository.GetAsync(id);
            if (announcement != null)
            {
                announcement.IsActive = value.IsActive;
                var result = await repository.SaveChangesAsync();

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
            var currentUser = await userRepository.GetUserByLogin(login);
            var announcementModel = mapper.Map<AnnouncementSaveDto, Announcement>(announcement);
            announcementModel.UserId = currentUser.Id;
            announcementModel.DateCreated = DateTime.Now;
            await repository.AddAsync(announcementModel);
            var result = await repository.SaveChangesAsync();
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
            var announcementInDb = await repository.GetAsync(id);
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

            var result = await repository.SaveChangesAsync();
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
            var entity = await repository.GetAsync(id);
            if (entity == null)
            {
                return new OperationResult()
                {
                    IsSuccessful = false,
                    IsClientError = true,
                    Message = "Announcement was not found"
                };
            }
            repository.Remove(entity);
            var result = await repository.SaveChangesAsync();
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
