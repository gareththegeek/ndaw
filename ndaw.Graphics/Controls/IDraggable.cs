using ndaw.Core.Routing;

namespace ndaw.Graphics.Controls
{
    public interface IDraggable: INamed
    {
        int X { get; set; }
        int Y { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        bool Offset { get; }
    }
}
