
namespace ndaw.Core.Routing
{
    public interface IMonoSignalNode: ISignalProcessNode
    {
        ISignalSink CentreIn { get; }
        ISignalSource CentreOut { get; }
    }
}
