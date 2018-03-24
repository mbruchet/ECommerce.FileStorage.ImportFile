using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Import.Abstractions.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApiImport.Controllers
{
    [Route("api/[controller]")]
    public class ImportController : Controller
    {
        private readonly IImportService _storageService;
        private readonly DiagnosticSource _diagnosticSource;
        private readonly ILogger<ImportController> _logger;

        public ImportController(IImportService storageService, DiagnosticSource diagnosticSource, ILoggerFactory loggerFactory)
        {
            _storageService = storageService;
            _diagnosticSource = diagnosticSource;
            _logger = loggerFactory.CreateLogger<ImportController>();
        }

        [HttpGet]
        public async Task<IActionResult> Get(string filename, bool headerOnFirstRow, string separator)
        {
            var activity = new Activity("ImportFile");

            _logger.LogTrace($"start Import file {filename}");

            IActionResult result = null;

            try
            {
                if (_diagnosticSource?.IsEnabled(activity.OperationName) == true)
                    _diagnosticSource.StartActivity(activity, 1);

                if (string.IsNullOrEmpty(filename))
                    return BadRequest("filename is null");

                result = (await _storageService.DownloadFile(filename, headerOnFirstRow, separator))
                    ? Ok()
                    : (IActionResult) BadRequest();

                if (_diagnosticSource?.IsEnabled(activity.OperationName) == true)
                    _diagnosticSource.StopActivity(activity, 1);
            }
            catch (Exception exception)
            {
                _logger.LogError($"can not import file {exception}");
            }

            _logger.LogTrace($"end Import file {filename}");

            return result;
        }
    }
}
