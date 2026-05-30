using FileMonitorWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FileMonitorWebApp.Controllers
{
    public class MonitorController : Controller
    {
        private readonly ILogger<MonitorController> _logger;

        public MonitorController(ILogger<MonitorController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
