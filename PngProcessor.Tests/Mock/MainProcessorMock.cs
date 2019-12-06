using Moq;
using PngProcessor.Infrastructure;
using System.Collections.Generic;
using PngProcessor.Infrastructure.Processor;

namespace PngProcessor.Tests.Mock
{
    internal class MainProcessorMock
    {
        private Mock<IMainProcessor> _mock;
        private Dictionary<string, ProcessStatusInfoBase> _holder;
        public IMainProcessor Object => _mock.Object;

        public int Count => _holder.Count;

        public MainProcessorMock()
        {
            _holder = new Dictionary<string, ProcessStatusInfoBase>();

            _mock = new Mock<IMainProcessor>();

            _mock.SetupGet(s => s.QueueLength).Returns(_holder.Count);

            _mock.Setup(s => s.GetStatus(It.IsAny<string>())).Returns((string id) =>
            {
                ProcessStatusInfoBase result = null;
                _holder.TryGetValue(id, out result);
                return result;
            });


            _mock.Setup(s => s.Add(It.IsAny<string>())).Callback((string id) =>
            {
                _holder.Add(id,new ProcessStatusInfoBase());
            });

        }

        public void Add(string id, ProcessStatusInfoBase status)
        {
            _holder.Add(id, status);
        }
    }
}
