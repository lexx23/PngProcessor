using System.Net;
using System.Web.Http;
using System.Threading.Tasks;
using System.Web.Http.Results;
using PngProcessor.Infrastructure;
using System.Web.Http.Description;
using PngProcessor.Infrastructure.Services;
using PngProcessor.Infrastructure.Processor;

namespace PngProcessor.Controllers
{
    public class ProcessorController : ApiController
    {
        private IUploadImageService _imageService;
        private IMainProcessor _processor;

        public ProcessorController(IUploadImageService imageService,IMainProcessor processor)
        {
            _processor = processor;
            _imageService = imageService;
        }

        [HttpGet]
        [ResponseType(typeof(ProcessStatusInfoBase))]
        public IHttpActionResult Get(string id)
        {
            var status = _processor.GetStatus(id);
            if(status == null)
                return BadRequest($"id [{id}] не найден.");

            return Ok(status);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post()
        {
            string guid = await _imageService.SaveAsync(Request.Content);

            if (!string.IsNullOrEmpty(guid))
            {
                _processor.Add(guid);
                return CreatedAtRoute<string>("DefaultApi", new { controller = "processor", id = guid }, guid);
            }

            return BadRequest("Не передан файл для загрузки.");
        }

        [HttpDelete]
        public IHttpActionResult Delete(string id)
        {
            _processor.Remove(id);
            return new StatusCodeResult(HttpStatusCode.NoContent, this);
        }
    }
}
