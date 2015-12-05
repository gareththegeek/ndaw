using ndaw.Core.Routing;
using System.Linq;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ndaw.Graphics.Controls
{
    public partial class SignalNetworkControl : DXControlBase, IScrollableArea
    {
        private Point viewPosition;
        private Point minimumView = new Point(-100, -100);
        private Point maximumView = new Point(1000, 1000);

        public event EventHandler<EventArgs> ViewPositionChange;
        public event EventHandler<EventArgs> MinimumViewChange;
        public event EventHandler<EventArgs> MaximumViewChange;

        public Point ViewPosition
        {
            get { return viewPosition; }
            set
            {
                viewPosition = value;

                if (ViewPositionChange != null)
                {
                    ViewPositionChange.Invoke(this, new EventArgs());
                }

                Refresh();
            }
        }

        public Point MinimumView
        {
            get { return minimumView; }
            set
            {
                minimumView = value;

                if (MinimumViewChange != null)
                {
                    MinimumViewChange.Invoke(this, new EventArgs());
                }
            }
        }

        public Point MaximumView
        {
            get { return maximumView; }
            set
            {
                maximumView = value;

                if (MaximumViewChange != null)
                {
                    MaximumViewChange.Invoke(this, new EventArgs());
                }
            }
        }

        public SignalNetworkControl()
        {
            models = new Dictionary<ISignalNode, SignalNodeViewModel>();

            InitializeComponent();

            this.Disposed += SignalNetworkControl_Disposed;
        }

        private ObservableCollection<ISignalNode> nodes;
        private Dictionary<ISignalNode, SignalNodeViewModel> models;

        private SolidColorBrush boxBrush;
        private TextFormat nodeFont;
        private TextFormat portFont;

        public ObservableCollection<ISignalNode> Nodes
        {
            get { return nodes; }
            set
            {
                if (nodes != null)
                {
                    nodes.CollectionChanged -= nodes_CollectionChanged;
                }

                nodes = value;

                if (nodes != null)
                {
                    nodes.CollectionChanged += nodes_CollectionChanged;
                }
            }
        }

        private void SignalNetworkControl_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;

            boxBrush = new SolidColorBrush(context.RenderTarget, Color4.Black);
            nodeFont = new TextFormat(context.FontFactory, "Consolas", 24f);
            portFont = new TextFormat(context.FontFactory, "Consolas", 12f);
        }

        private void SignalNetworkControl_Disposed(object sender, EventArgs e)
        {
            if (boxBrush != null)
            {
                boxBrush.Dispose();
                boxBrush = null;
            }
        }

        private void nodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            updateModels();

            Refresh();
        }
        
        private void updateModels()
        {
            var i = 20;
            foreach (var node in nodes)
            {
                SignalNodeViewModel model;
                if (!models.TryGetValue(node, out model))
                {
                    model = new SignalNodeViewModel
                    {
                        Node = node,
                        //TODO find sensible place to dump nodes
                        //TODO generation of models should be delegated to another class
                        X = i,
                        Y = 20
                    };
                    models[node] = model;
                }
                i += 400;
            }
        }

        protected override void paint()
        {
            if (nodes == null) return;

            context.RenderTarget.BeginDraw();
            context.RenderTarget.Clear(Color4.White);

            renderModels();

            context.RenderTarget.EndDraw();
        }

        private void renderModels()
        {
            var worldTransform = Matrix3x2.Translation(-viewPosition.X, -viewPosition.Y);

            foreach (var model in models.Values)
            {
                renderModel(worldTransform, model);
            }
        }

        private void renderModel(Matrix3x2 worldTransform, SignalNodeViewModel model)
        {
            var modelTransform = Matrix3x2.Translation(model.X, model.Y) * worldTransform;
            context.RenderTarget.Transform = modelTransform;

            const int portSize = 32;
            const int margin = 20;

            var nodeTitle = new TextLayout(context.FontFactory, model.Node.Name, nodeFont, 10000f, 0f);

            var nodeTitleWidth = (int)nodeTitle.Metrics.Width + margin;
            var nodeTitleHeight = (int)nodeTitle.Metrics.Height + margin * 2;

            var portCount = Math.Max(model.Node.Sources.Count(), model.Node.Sinks.Count());
            var portColumnWidth = portSize + margin;
            var portColumnHeight = portSize + margin;

            var boxWidth = Math.Max(portColumnWidth * 2, nodeTitleWidth);
            var boxHeight = nodeTitleHeight + portColumnHeight * portCount;

            context.RenderTarget.DrawRectangle(
                new RectangleF
                {
                    Left = 0,
                    Top = 0,
                    Height = boxHeight,
                    Width = boxWidth
                }, boxBrush);

            var fontX = (boxWidth - (int)nodeTitle.Metrics.Width) / 2;
            var fontY = (margin >> 1);
            context.RenderTarget.DrawTextLayout(new Vector2(fontX, fontY), nodeTitle, boxBrush);

            renderPorts(
                model.Node.Sinks, 
                modelTransform, 
                margin,
                nodeTitleHeight,
                portSize,
                portColumnHeight);

            renderPorts(
                model.Node.Sources, 
                modelTransform, 
                boxWidth - portSize - margin, 
                nodeTitleHeight, 
                portSize,
                portColumnHeight);
        }

        private void renderPorts(
            IEnumerable<INamed> ports,
            Matrix3x2 modelTransform,
            int x, 
            int y,
            int size, 
            int stride)
        {
            var portTransform = Matrix3x2.Translation(x, y);

            foreach (var port in ports)
            {
                context.RenderTarget.Transform = modelTransform * portTransform;

                var sinkTitle = new TextLayout(context.FontFactory, port.Name, portFont, 10000f, 0f);
                context.RenderTarget.DrawTextLayout(Vector2.Zero, sinkTitle, boxBrush);

                context.RenderTarget.DrawRectangle(new RectangleF
                {
                    Left = 0,
                    Top = 0,
                    Width = size,
                    Height = size
                }, boxBrush);

                portTransform *= Matrix3x2.Translation(0f, stride);
            }
        }
    }
}
