using ndaw.Core.Routing;

namespace ndaw.Graphics.Controls
{
    public class SignalNodeViewModel: IDraggable
    {
        public ISignalNode Node { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
    }
}
