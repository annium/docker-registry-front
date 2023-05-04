using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[Route("/")]
public class IndexController : ControllerBase
{
    public IndexController(
    )
    {
    }

    [HttpGet]
    public IActionResult Index()
    {
        return Ok("index");
    }
}