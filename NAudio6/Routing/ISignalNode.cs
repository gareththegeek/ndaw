
namespace ndaw.Routing
{
    public interface ISignalNode
    {
        bool Bypass { get; set; }

        ISignalProcess SignalProcess { get; }
    }
}
