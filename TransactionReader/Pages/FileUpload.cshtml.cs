using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TransactionReader.Data;
using TransactionReader.Utilities.FileParser;

namespace TransactionReader.Pages
{
    public class FileUploadModel : PageModel
    {
        // To save physical files to a path provided by configuration
        private readonly string _targetFilePath;
        private readonly long _fileSizeLimit;
        private readonly string[] _permittedExtensions = { ".csv", ".xml" };
        private readonly IWebHostEnvironment _environment;
        private readonly TransationContext _context;
        private readonly ILogger<IndexModel> _logger;

        public FileUploadModel(IConfiguration config, IWebHostEnvironment env, TransationContext context, ILogger<IndexModel> logger)
        {
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
            _targetFilePath = config.GetValue<string>("StoredFilesPath");
            _environment = env;
            _context = context;
            _logger = logger;
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

            FileValidation(FormFileUpload, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // For the file name of the uploaded file stored server-side,
            // use Path.GetRandomFileName to generate a safe random file name.
            var trustedFileNameForFileStorage = Path.GetRandomFileName();
            var filePath = Path.Combine(_environment.ContentRootPath, _targetFilePath, trustedFileNameForFileStorage);
            var originFileName = FormFileUpload.FileName;
            var fileExtension = Path.GetExtension(originFileName).ToLowerInvariant();

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await FormFileUpload.CopyToAsync(fileStream);
            }

            var importer = FileImporter.Create(filePath, fileExtension);
            var result = importer.ParseTransactionFile();
            DeleteFile(filePath);

            if (result.Failed)
            {
                var invalidRecords = string.Join("; ", result.InvalidItems);
                _logger.LogError(invalidRecords);

                ModelState.AddModelError(originFileName, invalidRecords);
                return BadRequest(ModelState);
            }

            // Insert into DB
            AddNewTransactions(result);

            return StatusCode(StatusCodes.Status200OK,
                $"Added new {result.Results.Count()} record(s): {string.Join("; ", result.Results.Keys)}");
        }

        /// <summary>
        /// Check if file extantion is valid.
        /// </summary>
        private static bool IsValidFileExtention(string fileName, string[] permittedExtensions)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }

            var ext = Path.GetExtension(fileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate the uploaded file.
        /// </summary>
        private void FileValidation(IFormFile fileUpload, ModelStateDictionary modelState)
        {
            if (fileUpload == null)
            {
                modelState.AddModelError("File", "The file is not selected.");
                return;
            }

            // Check the file length.
            if (fileUpload == null || fileUpload.Length == 0)
            {
                modelState.AddModelError("File", "The file is empty.");
                return;
            }

            if (fileUpload.Length > _fileSizeLimit)
            {
                var megabyteSizeLimit = _fileSizeLimit / 1048576;
                modelState.AddModelError("File", "The file size exceeds " + $"{megabyteSizeLimit:N1} MB.");
            }

            // Check extension
            if (!IsValidFileExtention(fileUpload.FileName, _permittedExtensions))
            {
                modelState.AddModelError("File", "The file extension is not permitted. Please use 'csv' or 'xml' format.");
            }
        }

        /// <summary>
        /// Delete imported file since no need it anymore
        /// </summary>
        private void DeleteFile(string filePath)
        {
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch (IOException ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        /// <summary>
        /// Insert new transactions into DB
        /// </summary>
        private void AddNewTransactions(FileParsingResult result)
        {
            _context.AddRange(result.Results.Values);
            _context.SaveChanges();
        }
    }
}
