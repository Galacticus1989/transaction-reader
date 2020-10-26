using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace TransactionReader.Pages
{
    public class FileUploadModel : PageModel
    {
        // To save physical files to a path provided by configuration
        private readonly string _targetFilePath;
        private readonly long _fileSizeLimit;
        private readonly string[] _permittedExtensions = { ".csv", ".xml" };
        private readonly IWebHostEnvironment _environment;

        public FileUploadModel(IConfiguration config, IWebHostEnvironment env)
        {
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
            _targetFilePath = config.GetValue<string>("StoredFilesPath");
            _environment = env;
        }

        [BindProperty]
        public IFormFile FormFileUpload { get; set; }

        public string Result { get; private set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            if (!ModelState.IsValid)
            {
                Result = "Please correct the form.";
                return Page();
            }

            // TODO: Add file validation

            // For the file name of the uploaded file stored server-side,
            // use Path.GetRandomFileName to generate a safe random file name.
            var trustedFileNameForFileStorage = Path.GetRandomFileName();
            var filePath = Path.Combine(_environment.ContentRootPath, _targetFilePath, trustedFileNameForFileStorage);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await FormFileUpload.CopyToAsync(fileStream);
            }

            // TODO: parse file and save data to DB or log invalid records

            return StatusCode(StatusCodes.Status200OK, "File uploaded to server");
        }
    }
}
