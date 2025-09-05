using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using cuestionarioNom.Models.Dtos;
using cuestionarioNom.Services.Interfaces;

namespace cuestionarioNom.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ImportController : Controller
    {
        private readonly IImportService _import;
        public ImportController(IImportService import) => _import = import;

        public IActionResult Index() => View();

        [HttpPost]
        [RequestSizeLimit(20_000_000)]
        public async Task<IActionResult> UploadJson([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Err"] = "Selecciona un archivo .json";
                return RedirectToAction(nameof(Index));
            }

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;

            var dto = await JsonSerializer.DeserializeAsync<ImportQuestionnaireDto>(
                ms,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (dto == null)
            {
                TempData["Err"] = "JSON inválido.";
                return RedirectToAction(nameof(Index));
            }

            var id = await _import.ImportAsync(dto);
            TempData["Ok"] = $"Cuestionario importado con Id={id}";
            return RedirectToAction("Edit", "Questionnaires", new { area = "Admin", id });
        }

        // Endpoint alterno para importar pegando JSON en un textarea
        [HttpPost]
        public async Task<IActionResult> PasteJson([FromForm] string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                TempData["Err"] = "Pega un JSON válido.";
                return RedirectToAction(nameof(Index));
            }

            var dto = JsonSerializer.Deserialize<ImportQuestionnaireDto>(
                json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (dto == null)
            {
                TempData["Err"] = "JSON inválido.";
                return RedirectToAction(nameof(Index));
            }

            var id = await _import.ImportAsync(dto);
            TempData["Ok"] = $"Cuestionario importado con Id={id}";
            return RedirectToAction("Edit", "Questionnaires", new { area = "Admin", id });
        }
    }
}
