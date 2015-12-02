using System.Collections.ObjectModel;

namespace ndaw.Core.Routing
{
    public interface ISignalNetwork
    {
        ObservableCollection<ISignalNode> Nodes { get; }
    }
}
