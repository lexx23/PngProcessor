using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PngProcessor.Infrastructure.Services
{
    public interface IUploadImageService
    {
        Task<string> SaveAsync(HttpContent content);
        bool Delete(string fileName);
    }
}