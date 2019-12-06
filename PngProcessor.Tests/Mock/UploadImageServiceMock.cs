using Moq;
using System;
using System.IO;
using PngProcessor.Infrastructure.Services;
using System.Net.Http;

namespace PngProcessor.Tests.Mock
{
    class UploadImageServiceMock
    {
        private Mock<IUploadImageService> _mock;
        public IUploadImageService Object => _mock.Object;

        public UploadImageServiceMock()
        {
            _mock = new Mock<IUploadImageService>();
            _mock.Setup(s => s.SaveAsync(It.IsAny<HttpContent>())).ReturnsAsync((HttpContent content) =>
            {
                if (content == null)
                    return string.Empty;
                return Guid.NewGuid().ToString();
            });

        }
    }
}
