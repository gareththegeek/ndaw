using ndaw.Core.Routing;
using SharpDX;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ndaw.Graphics.Controls
{
    public partial class SignalNetworkControl : DXControlBase
    {
        public SignalNetworkControl()
        {
            models = new Dictionary<ISignalNode, SignalNodeViewModel>();

            InitializeComponent();

            this.Disposed += SignalNetworkControl_Disposed;
        }

        private ObservableCollection<ISignalNode> nodes;
        private Dictionary<ISignalNode, SignalNodeViewModel> models;

        private SolidColorBrush boxBrush;

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

            DXPaint();
        }
        
        private void updateModels()
        {
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
                        X = 20,
                        Y = 20,
                        Width = 100,
                        Height = 100
                    };
                    models[node] = model;
                }
            }
        }

        protected override void DXPaint()
        {
            if (DesignMode) return;

            if (nodes == null) return;

            context.Activate();
            context.RenderTarget.BeginDraw();
            context.RenderTarget.Clear(Color4.White);

            renderModels();

            context.RenderTarget.EndDraw();
            context.Present();
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
            context.RenderTarget.DrawRectangle(
                new RectangleF
                {
                    Left = model.X,
                    Top = model.Y,
                    Height = model.Height,
                    Width = model.Width
                }, boxBrush, 1f);
        }
    }
}
