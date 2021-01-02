using Games4TradeAPI.Consts;
using System.IO;
using System.Threading.Tasks;

namespace Games4TradeAPI.Interfaces.Services
{

    public interface IImageService
    {
        Task<Stream> CreatePhotoMinature(Stream source, int width = CommonConsts.DefaultMiniatureWidth);
    }
}
