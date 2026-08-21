namespace Games4TradeAPI.Core.Images;

public interface IThumbnailGenerator
{
    Task WriteJpegAsync(
        Stream input,
        Stream output,
        int maximumWidth,
        int maximumHeight,
        CancellationToken cancellationToken = default);
}
