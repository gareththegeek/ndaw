using ndaw.Core.Routing;

namespace ndaw.Graphics.Controls
{
    public class SignalSourceViewModel: IDraggable, INamed
    {
        public ISignalSource Source { get; set; }
        public bool Offset { get { return true; } }

        public string Name
        {
            get
            {
                if (Source == null) return string.Empty;

                return Source.Name;
            }
            set { }
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
