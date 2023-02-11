using AppCoreLite.Enums;
using AppCoreLite.Models;
using AppCoreLite.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AccountsController(AccountService accountService)
        {
            _accountService = accountService;
            _accountService.Set(Languages.English);
        }

        [HttpPost("Login")]
        public IActionResult Login(AccountLogin user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = _accountService.CreateJwt(user);
            if (!result.IsSuccessful)
                return BadRequest(result.Message);
            return Ok(result.Data);
        }

        [HttpPost("Register")]
        public IActionResult Register(AccountRegister user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = _accountService.Register(user);
            if (!result.IsSuccessful)
                return BadRequest(result.Message);
            return Ok(result);
        }
    }
}
