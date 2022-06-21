using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailParametersController : ControllerBase
    {
        private readonly IMailParameterService _mailParameterService;

        public MailParametersController(IMailParameterService mailParameterService)
        {
            _mailParameterService = mailParameterService;
        }
        [HttpPost("Update")]
        public IActionResult MailParameter(MailParameter mailParameter)
        {
            var result = _mailParameterService.Update(mailParameter);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
    }
}
