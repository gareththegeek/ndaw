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
        private float zoom = 1f;
        private Matrix3x2 zoomTransform = Matrix3x2.Scaling(1f);

        public event EventHandler<EventArgs> ViewPositionChange;
        public event EventHandler<EventArgs> MinimumViewChange;
        public event EventHandler<EventArgs> MaximumViewChange;
        public event EventHandler<EventArgs> ZoomChange;
        
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

        public float Zoom
        {
            get { return zoom; }
            set
            {
                zoom = value;
                zoomTransform = Matrix3x2.Scaling(zoom);

                if (ZoomChange != null)
                {
                    ZoomChange.Invoke(this, new EventArgs());
                }

                Refresh();
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

        private DragConnection dragConnection;
        private IDraggable draggedPort;

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

        private bool overlaps(int x, int y, IDraggable draggable)
        {
            return x >= draggable.X
                    && x <= draggable.X + draggable.Width
                    && y >= draggable.Y
                    && y <= draggable.Y + draggable.Height;
        }

        private IDraggable getDraggableAt(int x, int y)
        {
            foreach (var sink in models.Values.SelectMany(m => m.Sinks))
            {
                if (overlaps(x, y, sink))
                {
                    return sink;
                }
            }

            foreach (var source in models.Values.SelectMany(m => m.Sources))
            {
                if (overlaps(x, y, source))
                {
                    return source;
                }
            }

            foreach (var model in models.Values)
            {
                if (overlaps(x, y, model))
                {
                    return model;
                }
            }

            return null;
        }

        public IDraggable BeginDragAt(int x, int y)
        {
            var draggable = getDraggableAt(x, y);

            if (draggable is SignalSinkViewModel)
            {
                var sink = draggable as SignalSinkViewModel;
                if (sink.Sink.Source != null)
                {
                    draggedPort = getSourceModelForSink(sink);

                    disconnect(sink);

                    dragConnection = new DragConnection
                    {
                        X = sink.X,
                        Y = sink.Y,
                        Width = sink.Width,
                        Height = sink.Height
                    };
                    return dragConnection;
                }
            }

            if (draggable is SignalSinkViewModel
                || draggable is SignalSourceViewModel)
            {

                draggedPort = draggable;
                dragConnection = new DragConnection
                {
                    X = draggable.X,
                    Y = draggable.Y,
                    Width = draggable.Width,
                    Height = draggable.Height
                };
                return dragConnection;
            }

            return draggable;
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
                        //TODO find sensible place to dump nodes on canvas
                        //TODO generation of models should be delegated to another class
                        X = i,
                        Y = 20,
                        Sinks = node.Sinks.Select(s => new SignalSinkViewModel { Sink = s }).ToList(),
                        Sources = node.Sources.Select(s => new SignalSourceViewModel { Source = s }).ToList(),
                    };
                    models[node] = model;
                }
                i += 400;
            }
        }

        private void applyTransform(Matrix3x2 modelTransform)
        {
            var worldTransform = Matrix3x2.Translation(-viewPosition.X, -viewPosition.Y);

            context.RenderTarget.Transform = worldTransform * modelTransform * zoomTransform;
        }

        protected override void paint()
        {
            if (nodes == null) return;

            context.RenderTarget.BeginDraw();
            context.RenderTarget.Clear(Color4.White);

            renderModels();
            renderConnections();
            renderDragConnection();

            context.RenderTarget.EndDraw();
        }

        private void renderModels()
        {
            foreach (var model in models.Values)
            {
                renderModel(model);
            }
        }

        private void renderModel(SignalNodeViewModel model)
        {
            var modelTransform = Matrix3x2.Translation(model.X, model.Y);
            applyTransform(modelTransform);

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

            model.Width = boxWidth;
            model.Height = boxHeight;

            context.RenderTarget.DrawRectangle(
                new RectangleF
                {
                    Left = 0,
                    Top = 0,
                    Height = boxHeight,
                    Width = boxWidth
                }, boxBrush, 1f/zoom);

            var fontX = (boxWidth - (int)nodeTitle.Metrics.Width) / 2;
            var fontY = (margin >> 1);
            context.RenderTarget.DrawTextLayout(new Vector2(fontX, fontY), nodeTitle, boxBrush);

            renderPorts(
                model.Sinks, 
                modelTransform, 
                margin,
                nodeTitleHeight,
                portSize,
                portColumnHeight);

            renderPorts(
                model.Sources, 
                modelTransform, 
                boxWidth - portSize - margin, 
                nodeTitleHeight, 
                portSize,
                portColumnHeight);
        }

        private void renderPorts(
            IEnumerable<IDraggable> ports,
            Matrix3x2 modelTransform,
            int x,
            int y,
            int size,
            int stride)
        {
            var portTransform = Matrix3x2.Translation(x, y);
            
            foreach (var port in ports)
            {
                port.X = (int)(modelTransform.TranslationVector.X + portTransform.TranslationVector.X);
                port.Y = (int)(modelTransform.TranslationVector.Y + portTransform.TranslationVector.Y);
                port.Width = size;
                port.Height = size;

                applyTransform(modelTransform * portTransform);

                var sinkTitle = new TextLayout(context.FontFactory, port.Name, portFont, 10000f, 0f);
                context.RenderTarget.DrawTextLayout(Vector2.Zero, sinkTitle, boxBrush);

                context.RenderTarget.DrawRectangle(new RectangleF
                {
                    Left = 0,
                    Top = 0,
                    Width = size,
                    Height = size
                }, boxBrush, 1f / zoom);

                portTransform *= Matrix3x2.Translation(0f, stride);
            }
        }

        private void renderConnections()
        {
            applyTransform(Matrix3x2.Identity);

            foreach (var sink in models.Values.SelectMany(m => m.Sinks).Where(s => s.Sink.Source != null))
            {
                var source = getSourceModelForSink(sink);

                var v0 = new Vector2(sink.X + sink.Width / 2, sink.Y + sink.Height / 2);
                var v1 = new Vector2(source.X + source.Width / 2, source.Y + source.Height / 2);

                context.RenderTarget.DrawLine(v0, v1, boxBrush, 1f / zoom);
            }
        }

        private SignalSourceViewModel getSourceModelForSink(SignalSinkViewModel sink)
        {
            var source = sink.Sink.Source;
            var owner = source.Owner;
            var ownerModel = models[owner];
            return ownerModel.Sources.Single(s => s.Name == source.Name);
        }

        private void renderDragConnection()
        {
            if (dragConnection == null) return;

            applyTransform(Matrix3x2.Identity);

            var hWidth = draggedPort.Width / 2f;
            var hHeight = draggedPort.Height / 2f;

            var x0 = draggedPort.X + hWidth;
            var y0 = draggedPort.Y + hHeight;

            var x1 = dragConnection.X;
            var y1 = dragConnection.Y;

            context.RenderTarget.DrawLine(new Vector2(x0, y0), new Vector2(x1, y1), boxBrush, 1f / zoom);
        }

        public void DragComplete(IDraggable draggable)
        {
            if (draggable == null) return;

            if (draggable == dragConnection)
            {
                var target = getDraggableAt(dragConnection.X, dragConnection.Y);
                
                if (target is SignalSourceViewModel && draggedPort is SignalSinkViewModel)
                {
                    connect(target as SignalSourceViewModel, draggedPort as SignalSinkViewModel);
                }
                else if (target is SignalSinkViewModel && draggedPort is SignalSourceViewModel)
                {
                    connect(draggedPort as SignalSourceViewModel, target as SignalSinkViewModel);
                }

                dragConnection = null;
                draggedPort = null;

                Refresh();
            }
        }

        private void connect(SignalSourceViewModel source, SignalSinkViewModel sink)
        {
            sink.Sink.Source = source.Source;
        }

        private void disconnect(SignalSinkViewModel sink)
        {
            sink.Sink.Source = null;
        }
    }
}
