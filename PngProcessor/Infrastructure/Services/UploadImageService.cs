using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PngProcessor.Infrastructure.Services
{
    public class UploadImageService : IUploadImageService
    {
        private string _path;

        public UploadImageService(string path = null)
        {
            if (string.IsNullOrEmpty(path))
                path = "upload";
            _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);
        }

        /// <summary>
        /// Сохранение, полученного от пользователя, файла
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<string> SaveAsync(HttpContent content)
        {
            if (content == null || !content.IsMimeMultipartContent())
                return string.Empty;

            var provider = new MultipartMemoryStreamProvider();
            await content.ReadAsMultipartAsync(provider);
            if (provider.Contents.Count == 0)
                return string.Empty;

            try
            {
                var file = provider.Contents[0];
                var buffer = await file.ReadAsByteArrayAsync();
                var guid = Guid.NewGuid().ToString();

                var filePath = Path.Combine(_path, guid);
                using (var localFile = new FileStream(filePath, FileMode.Create))
                {
                    await localFile.WriteAsync(buffer, 0, buffer.Length);
                    await localFile.FlushAsync();
                    localFile.Close();
                }
                return guid;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Удалить аватар.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool Delete(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_path, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }

        }
    }
}