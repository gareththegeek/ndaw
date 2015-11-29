using ndaw.Routing;

namespace ndaw.Filters.Implementations
{
    public interface IFilterImplementation: ISignalProcess
    {
        float[] Coefficients { get; set; }
    }
}
