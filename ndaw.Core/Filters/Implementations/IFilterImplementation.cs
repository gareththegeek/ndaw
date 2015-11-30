using ndaw.Core.Routing;

namespace ndaw.Core.Filters.Implementations
{
    public interface IFilterImplementation: ISignalProcess
    {
        float[] Coefficients { get; set; }
    }
}
