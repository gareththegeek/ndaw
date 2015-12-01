using NAudio.Wave;

namespace ndaw.Core.Routing
{
    public interface ISignalProcess: INamed
    {
        WaveFormat Format { get; set; }

        void Process(float[][] buffers, int count);
    }
}
