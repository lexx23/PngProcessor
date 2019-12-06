using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using PngProcessor.Infrastructure.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PngProcessor.Tests
{
    [TestClass]
    public class UploadImageSeviceTest
    {
        public HttpRequestMessage GetHttpRequest()
        {
            var stream = new FileStream(Path.Combine("Images", "img1.png"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            byte[] byteArray = new byte[stream.Length];
            stream.Read(byteArray, 0, (int)stream.Length);

            var content = new ByteArrayContent(byteArray);
            content.Headers.Add("Content-Disposition", "form-data");
            var request = new HttpRequestMessage
            {
                Content = new MultipartContent { content }
            };
            return request;
        }

        /// <summary>
        /// Проверка сохранения png файла
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task SaveImage()
        {
            var uploadPathName = "upload";
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, uploadPathName);
            IUploadImageService imageService = new UploadImageService(uploadPathName);

            string guid = await imageService.SaveAsync(GetHttpRequest().Content);

            Assert.IsNotNull(guid);

            var newFilePath = Path.Combine(path, guid);
            var fileExist = File.Exists(newFilePath);

            Assert.IsTrue(fileExist);

            var deleteResult = imageService.Delete(guid);
            Assert.IsTrue(deleteResult);
        }

    }
}
