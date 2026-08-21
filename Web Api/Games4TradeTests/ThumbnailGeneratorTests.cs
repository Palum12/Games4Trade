using Games4TradeAPI.Core.Images;
using SkiaSharp;
using Xunit;

namespace Games4TradeAPITests;

public class ThumbnailGeneratorTests
{
    [Fact]
    public async Task WriteJpegAsync_ResizesImageWithinRequestedBounds()
    {
        using var sourceBitmap = new SKBitmap(600, 400);
        sourceBitmap.Erase(SKColors.CornflowerBlue);
        using var sourceImage = SKImage.FromBitmap(sourceBitmap);
        using var sourceData = sourceImage.Encode(SKEncodedImageFormat.Png, 100);
        await using var input = new MemoryStream(sourceData.ToArray());
        await using var output = new MemoryStream();
        var generator = new SkiaThumbnailGenerator();

        await generator.WriteJpegAsync(input, output, 300, 200);

        output.Position = 0;
        using var thumbnail = SKBitmap.Decode(output);
        Assert.NotNull(thumbnail);
        Assert.Equal(300, thumbnail.Width);
        Assert.Equal(200, thumbnail.Height);
    }
}
