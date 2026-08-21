using SkiaSharp;

namespace Games4TradeAPI.Core.Images;

public sealed class SkiaThumbnailGenerator : IThumbnailGenerator
{
    private const int JpegQuality = 85;

    public async Task WriteJpegAsync(
        Stream input,
        Stream output,
        int maximumWidth,
        int maximumHeight,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumWidth);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumHeight);
        cancellationToken.ThrowIfCancellationRequested();

        using var original = SKBitmap.Decode(input)
            ?? throw new InvalidDataException("The uploaded file is not a supported image.");

        var scale = Math.Min(
            1d,
            Math.Min(
                (double)maximumWidth / original.Width,
                (double)maximumHeight / original.Height));
        var width = Math.Max(1, (int)Math.Round(original.Width * scale));
        var height = Math.Max(1, (int)Math.Round(original.Height * scale));

        using var resized = original.Resize(
            new SKImageInfo(width, height),
            new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear))
            ?? throw new InvalidOperationException("The image could not be resized.");
        using var image = SKImage.FromBitmap(resized);
        using var encoded = image.Encode(SKEncodedImageFormat.Jpeg, JpegQuality)
            ?? throw new InvalidOperationException("The thumbnail could not be encoded.");

        encoded.SaveTo(output);
        await output.FlushAsync(cancellationToken);
    }
}
