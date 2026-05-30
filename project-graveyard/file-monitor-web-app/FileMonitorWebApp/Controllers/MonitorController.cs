using FileMonitorWebApp.Models;
using FileMonitorWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FileMonitorWebApp.Controllers
{
    /// <summary>
    /// Controller responsible for handling requests related to monitoring the directory.
    /// </summary>
    public class MonitorController : Controller
    {
        private readonly ILogger<MonitorController> _logger;
        private readonly MonitorService _monitorService;

        private const string TempDataKey = "ScanResult";

        public MonitorController(
            ILogger<MonitorController> logger,
            MonitorService monitorService)
        {
            _logger = logger;
            _monitorService = monitorService;
        }

        public IActionResult Index()
        {
            // Pass the scan result from TempData to the view if it exists
            if (TempData[TempDataKey] is string json)
            {
                var result = JsonSerializer.Deserialize<ScanResult>(json);
                return View(result);
            }

            return View(model: null);
        }

        /// <summary>
        /// Scans the specified directory for changes.
        /// </summary>
        /// <param name="directoryPath">The path of the directory to scan.</param>
        /// <returns>The view containing the scan results.</returns>
        [HttpPost]
        public IActionResult Scan(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                ModelState.AddModelError("DirectoryPath", "Directory path is required.");
                return View("Index", new ScanResult());
            }

            var result = _monitorService.Analyze(directoryPath);

            if (result.HasError)
            {
                _logger.LogWarning("Scan failed for directory: {DirectoryPath}. Error: {Error}", directoryPath, result.ErrorMessage);
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "An error occurred while scanning the directory.");
            }

            TempData[TempDataKey] = JsonSerializer.Serialize(result);

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
