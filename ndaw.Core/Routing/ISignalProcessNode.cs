
namespace ndaw.Core.Routing
{
    public interface ISignalProcessNode: ISignalNode
    {
        bool Bypass { get; set; }

        ISignalProcess SignalProcess { get; }
    }
}
