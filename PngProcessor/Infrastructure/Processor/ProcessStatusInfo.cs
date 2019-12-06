namespace PngProcessor.Infrastructure.Processor
{
    internal class ProcessStatusInfo : ProcessStatusInfoBase
    {
        public void SetStatus(ProcessStatusEnum status)
        {
            _status = status;
        }

        public void SetProgress(double progress)
        {
            _progress = progress;
        }
    }
}