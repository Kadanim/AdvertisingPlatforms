using AdvertisingPlatforms.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisingPlatforms.Controllers
{

    [Route("api/advertising")]
    [ApiController]
    public class AdvertisingController : ControllerBase
    {
        private readonly IAdvertisingService _service;

        public AdvertisingController(IAdvertisingService service)
        {
            _service = service;
        }

        /// <summary>
        /// Загружает данные о рекламных площадках из файла.
        /// </summary>
        /// <param name="file">Файл с данными в формате "Площадка:локация1,локация2".</param>
        /// <returns>200 OK при успехе или 400 BadRequest при ошибке.</returns>
        /// <exception cref="T:System.ArgumentException">Вызывается, если файл пустой или отсутствует.</exception>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            await _service.LoadPlatformsFromFileAsync(file);
            return Ok("Данные успешно загружены.");
        }

        /// <summary>
        /// Возвращает список рекламных площадок, действующих в указанной локации и её родителях.
        /// </summary>
        /// <param name="name">Путь локации в формате "/ru/svrd/revda" (должен начинаться с "/").</param>
        /// <returns>200 OK со списком площадок.</returns>
        /// <exception cref="T:System.ArgumentException">Вызывается при некорректной локации (пустая или без "/").</exception>
        [HttpGet("platforms")]
        public IActionResult GetPlatforms([FromQuery] string location)
        {
            var platforms = _service.GetPlatformsForLocation(location);
            return Ok(platforms);
        }
    }
}