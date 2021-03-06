﻿using ndaw.Core.Routing;
using System.Collections.Generic;

namespace ndaw.Graphics.Controls
{
    public class SignalNodeViewModel: IDraggable
    {
        public ISignalNode Node { get; set; }
        public bool Offset { get { return true; } }

        public string Name
        {
            get
            {
                if (Node == null) return string.Empty;

                return Node.Name;
            }
            set { }
        }

        public IEnumerable<SignalSourceViewModel> Sources { get; set; }
        public IEnumerable<SignalSinkViewModel> Sinks { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
    }
}
