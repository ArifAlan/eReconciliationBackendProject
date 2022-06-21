using Business.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register(UserAndCompanyRegisterDto userAndCompanyRegister)
        {
            var userExists = _authService.UserExists(userAndCompanyRegister.userForRegister.Email);//user var mı diye kontrol ediyor.
            if (!userExists.Success)
            {
                return BadRequest(userExists.Message);
            }

            var companyExists = _authService.CompanyExists(userAndCompanyRegister.Company);
            if (!companyExists.Success)
            {
                return BadRequest(companyExists.Message);
            }
           
            var registerResult = _authService.Register(userAndCompanyRegister.userForRegister, userAndCompanyRegister.userForRegister.Password, userAndCompanyRegister.Company);
             var result = _authService.CreateAccessToken(registerResult.Data,registerResult.Data.CompanyId);
            if (result.Success)
            {
                return Ok(result.Data);
            }

            //if (registerResult.Success)
            //{
            //    return Ok(registerResult);
            //}
            return BadRequest(registerResult.Message);  
        }

        [HttpPost("registerSecondAccount")] // 44
        public IActionResult RegisterSecondAccount(UserForRegister userForRegister, int companyId)
        {
            var userExists = _authService.UserExists(userForRegister.Email);
            if (!userExists.Success)
            {
                return BadRequest(userExists.Message);
            }

            var registerResult = _authService.RegisterSecondAccount(userForRegister, userForRegister.Password);
            var result = _authService.CreateAccessToken(registerResult.Data, companyId);
            if (result.Success)
            {
                return Ok(result.Data);
            }

            //if (registerResult.Success)
            //{
            //    return Ok(registerResult);
            //}
            return BadRequest(registerResult.Message);
        }


        [HttpPost("login")]
        public IActionResult Login(UserForLogin userForLogin)
        {
            var userTologin = _authService.Login(userForLogin);
            if (!userTologin.Success)
            {
                return BadRequest(userTologin.Message);
            }
            var result=_authService.CreateAccessToken(userTologin.Data,0);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }
    }
}
