using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PngProcessor.Infrastructure.Processor;

namespace PngProcessor.Infrastructure
{
    public class ProcessStatusInfoBase
    {
        protected ProcessStatusEnum _status;
        protected double _progress;

        [JsonConverter(typeof(StringEnumConverter))]
        public ProcessStatusEnum Status => _status;
        public double Progress => _progress;

        public ProcessStatusInfoBase()
        {
            _status = ProcessStatusEnum.Pending;
            _progress = 0;
        }
    }
}