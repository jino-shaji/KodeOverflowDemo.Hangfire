using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KodeOverflowDemo.Hangfire.Models;
using Hangfire;

namespace KodeOverflowDemo.Hangfire.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var id = BackgroundJob.Enqueue(() => DoSomeTask());
            // to start the second task after the completion of first one. The second task will be in que until the completion of first task
            BackgroundJob.ContinueJobWith(id, () => DoSomeOtherTask());
            var jobId = BackgroundJob.Schedule(() => Console.WriteLine("Delayed!"),TimeSpan.FromDays(7));

            return View();
        }
        void DoSomeTask()
        {
            Console.WriteLine("First task goes here");
        }
        void DoSomeOtherTask()
        {
            Console.WriteLine("Another task goes here");
        }
       

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
