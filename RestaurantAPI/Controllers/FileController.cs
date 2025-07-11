using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace RestaurantAPI.Controllers
{
    [Route("file")]
    [Authorize]
    public class FileController : ControllerBase
    {
        [HttpGet]
        [ResponseCache(Duration = 1200, VaryByQueryKeys = new string[] { "fileName" })]
        public ActionResult GetFile([FromQuery] string fileName)
        {
            // Pobieranie ścieżki do bazowego katalogu aplikacji
            string rootPath = Directory.GetCurrentDirectory();

            // Dodajemy ścieżkę do konkretengo folderu oraz nazwę pliku
            string filePath = $"{rootPath}/PrivateFiles/{fileName}";

            // Sprawdzamy czy plik istnieje
            bool fileExist = System.IO.File.Exists(filePath);
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


        [HttpPost]
        public ActionResult Upload([FromForm]IFormFile file)
        {
            if (file != null && file.Length>0)
            {
                string rootPath = Directory.GetCurrentDirectory();
                string fileName = file.FileName;
                string fullPath = $"{rootPath}/PrivateFiles/{fileName}";

                // tworzenie pliku w bloku using
                using (var strem = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(strem);
                }

                return Ok($"New file \"{fileName}\" has been created");
            }
            else return BadRequest("File is null or empty");
        }


    }//klasa
}//name space
