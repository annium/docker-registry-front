using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[Route("/v2/webhooks/events")]
public class EventsController : ControllerBase
{
    public IActionResult HandleEvent()
    {
        return Ok();
    }
}
