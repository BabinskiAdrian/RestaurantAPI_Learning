using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace RestaurantAPI.Controllers
{
    [Route("file")]
    [Authorize]
    public class FileController : ControllerBase
    {
        public ActionResult GetFile([FromQuery]string fileName)
        {
            // Pobieranie ścieżki do bazowego katalogu aplikacji
            var rootPath = Directory.GetCurrentDirectory();

            // Dodajemy ścieżkę do konkretengo folderu oraz nazwę pliku
            var filePath = $"{rootPath}/PrivateFiles/{fileName}";

            // Sprawdzamy czy plik istnieje
            var fileExist = System.IO.File.Exists(filePath);
            if (!fileExist)
            {
                return NotFound($"File {fileName} not found");
            }


            // Tworzymy instancję
            var contentProvicer = new FileExtensionContentTypeProvider();
            // Sprawdzamy jaki jest typ pliku na podstawie rozszerzenia i zapisujemy tą informację w zmiennej korzystając ze słowa kluczowego "out"
            contentProvicer.TryGetContentType(fileName, out string contentType);


            //załadowanie pliku do pamięci
            var fileContents = System.IO.File.ReadAllBytes(filePath);


            // Zwracamy plik jako odpowiedź, tym razem nie przez OK()
            return File(fileContents, contentType, fileName);
        }

    }
}
