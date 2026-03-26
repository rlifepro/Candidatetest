using Microsoft.AspNetCore.Mvc;

namespace CandidateTest.Api.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Candidate() => View();
        public IActionResult Admin() => View();
    }
}
