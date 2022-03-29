using Microsoft.AspNetCore.Mvc;

namespace StocksProcessing.API.Controllers;

[Route("api/{version:apiVersion}/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
}