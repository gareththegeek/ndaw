
namespace ndaw.Core.Routing
{
    public interface IMonoSignalNode: ISignalNode
    {
        ISignalSink CentreIn { get; }
        ISignalSource CentreOut { get; }
    }
}
