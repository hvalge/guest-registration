using Microsoft.AspNetCore.Mvc;

namespace GuestRegistration.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiBaseController : ControllerBase
{
}