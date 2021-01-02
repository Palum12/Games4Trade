using Games4TradeAPI.Interfaces.Services;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using Games4TradeAPI.Consts;

namespace Games4TradeAPI.Services
{
    public class ImageService : IImageService
    {
        public async Task<Stream> CreatePhotoMinature(Stream source, int width = CommonConsts.DefaultMiniatureWidth)
        {
            (Image image, IImageFormat format) =  await Image.LoadWithFormatAsync(source);
            var result = new MemoryStream();

            // setting height as 0 makes ImageSharp keep aspect ratio and calculate correct height itself
            image.Mutate(x => x.Resize(width, 0));
            await image.SaveAsync(result, format);

            result.Position = 0;
            image.Dispose();

            return result;
        }
    }
}
