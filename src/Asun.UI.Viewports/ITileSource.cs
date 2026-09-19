namespace Asun.UI.Viewports;

/// <summary>
/// Backend-neutral asynchronous tile source. Concrete image/decoder implementations
/// can be supplied without coupling the viewport runtime to a specific imaging library.
/// </summary>
public interface ITileSource<TTile>
{
    ValueTask<TTile> LoadAsync(
        TileRequest request,
        System.Drawing.RectangleF imageRectangle,
        CancellationToken cancellationToken = default);
}
