
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MinyToDo.Abstract.Services;
using MinyToDo.Api.Models.Auth;
using MinyToDo.Api.Services.Abstract;
using MinyToDo.Entity.Models;

namespace MinyToDo.Api.Controllers
{
    public class AuthController : ApiController
    {
        private UserManager<AppUser> _userManager;
        private IJwtTokenService _jwtTokenService;
        private IUserCategoryService _userCategoryService;
        private IMapper _mapper;
        public AuthController(IMapper mapper,IJwtTokenService jwtTokenService, UserManager<AppUser> userManager, IUserCategoryService userCategoryService)
        {
            _mapper = mapper;
            _jwtTokenService = jwtTokenService;
            _userManager = userManager;
            _userCategoryService = userCategoryService;
        }

        [HttpPost("Signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel value)
        {
            if (User.Identity.IsAuthenticated) return BadRequest();

            var newAppUser = _mapper.Map<AppUser>(value);

            var result = await _userManager.CreateAsync(newAppUser, value.Password);
            if (result.Succeeded)
            {
                var firstCategoryForUser = new UserCategory(newAppUser.Id, "General");
                #pragma warning disable 4014
                _userCategoryService.InsertAsync(firstCategoryForUser);
                #pragma warning disable 4014
                
                var token = await _jwtTokenService.CreateTokenAsync(newAppUser);
                return Ok(new { token });
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("Signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInModel value)
        {
            if (User.Identity.IsAuthenticated) return BadRequest();

            var isIdentifierEmail = value.Identifier.Contains('@');

            var appUser = isIdentifierEmail
            ? await _userManager.FindByEmailAsync(value.Identifier)
            : await _userManager.FindByNameAsync(value.Identifier);

            var isPasswordRelatedToFoundUser = appUser != null && await _userManager.CheckPasswordAsync(appUser, value.Password);
            if (isPasswordRelatedToFoundUser)
            {
                var token = await _jwtTokenService.CreateTokenAsync(appUser);
                return Ok(new { token });
            }
            return Unauthorized();
        }
    }
}