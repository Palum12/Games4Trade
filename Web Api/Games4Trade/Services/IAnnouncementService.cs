using System.Collections.Generic;
using System.Threading.Tasks;
using Games4Trade.Dtos;
using Games4Trade.Models;

namespace Games4Trade.Services
{
    public interface IAnnouncementService
    {
        Task<AnnouncementGetDto> GetAnnouncement(int id, bool isAdmin);
        Task<IList<AnnouncementGetDto>> GetAnnouncementsPage(int page, bool isAdmin);
        Task<OperationResult> ChangeStatus(int id, AnnouncementArchiveDto value);
        Task<OperationResult> CreateAnnouncement(AnnouncementSaveDto announcement, string login);
        Task<OperationResult> EditAnnouncement(int id, AnnouncementSaveDto announcement);
        Task<OperationResult> DeleteAnnouncement(int id);
    }
}