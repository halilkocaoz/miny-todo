using Microsoft.AspNetCore.Mvc;
using MinyToDo.Models;
using MinyToDo.Models.Enums;

namespace MinyToDo.WebAPI.Controllers
{
    public class BaseController : ControllerBase
    {
        [NonAction]
        public IActionResult ApiReturn(ApiResponse response)
        {
            switch (response.Type)
            {
                case ApiResponseType.Ok:
                    return Ok(response.Data);

                case ApiResponseType.Created:
                    return StatusCode(201);

                case ApiResponseType.NoContent:
                    return NoContent();

                case ApiResponseType.BadRequest:
                    return BadRequest(new { response.Message });

                case ApiResponseType.NotFound:
                    return NotFound(new { response.Message });

                case ApiResponseType.Unauthorized:
                    return Unauthorized();

                case ApiResponseType.Forbidden:
                    return Forbid();

                default:
                    return Accepted();
            }
        }
    }
}