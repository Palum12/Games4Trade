using System.Collections.Generic;
using System.Threading.Tasks;
using Games4Trade.Dtos;
using Games4Trade.Models;

namespace Games4Trade.Services
{
    public interface IAnnouncementService
    {
        Task<IList<AnnouncementGetListDto>> GetAnnouncements();
        Task<AnnouncementGetDetailDto> GetAnnouncement(int id);

        Task<OperationResult> CreateAnnouncement(AnnouncementSaveDto announcement);
        Task<OperationResult> EditAnnouncement(int id, AnnouncementSaveDto announcement);
        Task<OperationResult> DeleteAnnouncement(int id);
    }
}