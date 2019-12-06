namespace PngProcessor.Infrastructure.Processor
{
    public interface IMainProcessor
    {
        int Processed { get; }
        int QueueLength { get; }

        void Add(string id);
        void Remove(string id);
        ProcessStatusInfoBase GetStatus(string id);
        void Dispose();
    }
}