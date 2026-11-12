using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Contracts.Auth;
using TaskFlow.Application.Abstractions;
using TaskFlow.Application.Common.Security;

namespace TaskFlow.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [AutoValidateAntiforgeryToken]
    public sealed class AuthController(
    IAccountService accountService,
    IAntiforgery antiforgery)
    : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("csrf")]
        [ResponseCache(
            NoStore = true,
            Location = ResponseCacheLocation.None)]
        public IActionResult GetCsrfToken()
        {
            var tokens = antiforgery.GetAndStoreTokens(HttpContext);

            return Ok(new
            {
                token = tokens.RequestToken
            });
        }

        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost("employees")]
        public async Task<IActionResult> Register(
            RegisterRequest request)
        {
            var result = await accountService.RegisterAsync(
                request.FullName,
                request.Email,
                request.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Registration", error);
                }

                return ValidationProblem(ModelState);
            }

            return StatusCode(
                StatusCodes.Status201Created,
                new { id = result.UserId });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginRequest request)
        {
            var succeeded = await accountService.SignInAsync(
                request.Email,
                request.Password);

            if (!succeeded)
            {
                return Problem(
                    title: "Giriş başarısız.",
                    detail: "E-posta veya parola hatalı ya da hesap girişe kapalı.",
                    statusCode: StatusCodes.Status401Unauthorized);
            }

            return NoContent();
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var idClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(idClaim, out var userId))
            {
                return Unauthorized();
            }

            var user = await accountService.GetByIdAsync(userId);

            if (user is null)
            {
                return Unauthorized();
            }

            return Ok(user);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await accountService.SignOutAsync();

            return NoContent();
        }
    }
}