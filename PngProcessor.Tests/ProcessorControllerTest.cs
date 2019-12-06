using Microsoft.VisualStudio.TestTools.UnitTesting;
using PngProcessor.Controllers;
using PngProcessor.Infrastructure;
using PngProcessor.Infrastructure.Processor;
using PngProcessor.Tests.Mock;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace PngProcessor.Tests
{
    [TestClass]
    public class ProcessorControllerTest
    {
        private MainProcessorMock _processorMock;
        private UploadImageServiceMock _uploadImageServiceMock;

        public ProcessorControllerTest()
        {
            _processorMock = new MainProcessorMock();
            _uploadImageServiceMock = new UploadImageServiceMock();
        }

        public HttpRequestMessage GetHttpRequest(bool withContent = true)
        {
            var stream = new FileStream(Path.Combine("Images", "img1.png"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            byte[] byteArray = new byte[stream.Length];
            stream.Read(byteArray, 0, (int)stream.Length);

            var content = new ByteArrayContent(byteArray);
            content.Headers.Add("Content-Disposition", "form-data");
            var request = new HttpRequestMessage();

            if (withContent)
                request.Content = new MultipartContent { content };

            return request;
        }

        /// <summary>
        /// Проверка метода Get для несуществующего id
        /// </summary>
        [TestMethod]
        public void GetMethod_IdNotExist()
        {
            ProcessorController controller = new ProcessorController(_uploadImageServiceMock.Object, _processorMock.Object);
            var result = controller.Get("1-2-3");

            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
        }

        /// <summary>
        /// Проверка метода Get для существующего id 
        /// </summary>
        [TestMethod]
        public void GetMethod_IdExist()
        {
            var status = new ProcessStatusInfo();
            status.SetProgress(1);
            status.SetStatus(ProcessStatusEnum.Done);
            _processorMock.Add("1-2-4", status);

            ProcessorController controller = new ProcessorController(_uploadImageServiceMock.Object, _processorMock.Object);
            var result = controller.Get("1-2-4");

            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<ProcessStatusInfoBase>));
            ProcessStatusInfoBase value = ((OkNegotiatedContentResult<ProcessStatusInfoBase>)result).Content;

            Assert.AreEqual(1, value.Progress);
            Assert.AreEqual(ProcessStatusEnum.Done, value.Status);
        }

        /// <summary>
        /// Проверка метода Delete
        /// </summary>
        [TestMethod]
        public void DeleteMethod()
        {
            ProcessorController controller = new ProcessorController(_uploadImageServiceMock.Object, _processorMock.Object);
            var result = controller.Delete("1-2-4");

            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            var value = ((StatusCodeResult)result).StatusCode;

            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, value);
        }

        /// <summary>
        /// Проверка метода Post с правильными данными
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PostMethod_WithContent()
        {
            ProcessorController controller = new ProcessorController(_uploadImageServiceMock.Object, _processorMock.Object);
            controller.Request = GetHttpRequest();
            var result = await controller.Post();

            Assert.IsInstanceOfType(result, typeof(CreatedAtRouteNegotiatedContentResult<string>));
            var value = (CreatedAtRouteNegotiatedContentResult<string>)result;

            Assert.AreEqual(1, _processorMock.Count);
            Assert.AreEqual("DefaultApi", value.RouteName);
            Assert.AreEqual(2, value.RouteValues.Count);
        }

        /// <summary>
        /// Проверка метода Post если файл не передан
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task PostMethod_WithoutContent()
        {
            ProcessorController controller = new ProcessorController(_uploadImageServiceMock.Object, _processorMock.Object);
            controller.Request = GetHttpRequest(false);
            var result = await controller.Post();

            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));
            var value = (BadRequestErrorMessageResult)result;

            Assert.AreEqual(0, _processorMock.Count);
            Assert.AreEqual("Не передан файл для загрузки.", value.Message);
        }

    }
}
