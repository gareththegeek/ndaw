
namespace ndaw.Core.Routing
{
    public interface IStereoSignalNode: ISignalProcessNode
    {
        ISignalSink LeftIn { get; }
        ISignalSink RightIn { get; }

        ISignalSource LeftOut { get; }
        ISignalSource RightOut { get; }
    }
}
