using B2B.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace B2B.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        public record LoginViewModel(string Username, string Password);

        private readonly ILogger<TokenController> _logger;

        private JwtHelper _jwt;

        public TokenController(ILogger<TokenController> logger, JwtHelper jwt)
        {
            _logger = logger;
            _jwt = jwt;
        }

        private bool ValidateUser(LoginViewModel login)
        {
            // TODO 驗證使用者

            return true;
        }

        /// <summary>
        /// 登入並取得 JWT Token
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("Signin")]
        public IResult Signin(LoginViewModel login)
        {
            if (ValidateUser(login))
            {
                var token = _jwt.GenerateToken(login.Username);

                return Results.Ok(new { token });
            }
            else
            {
                return Results.BadRequest();
            }
        }

        /// <summary>
        /// 取得 JWT Token 中的所有 Claims
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("Claims")]
        public IResult Claims(ClaimsPrincipal user)
        {
            return Results.Ok(user.Claims.Select(p => new { p.Type, p.Value }));
        }

        /// <summary>
        /// 取得 JWT Token 中的使用者名稱
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("Username")]
        public IResult Username(ClaimsPrincipal user)
        {
            return Results.Ok(user.Identity?.Name);
        }

        /// <summary>
        /// 取得使用者是否擁有特定角色
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("IsInRole")]
        public IResult IsInRole(ClaimsPrincipal user, string name)
        {
            return Results.Ok(user.IsInRole(name));
        }

        /// <summary>
        /// 取得 JWT Token 中的 JWT ID
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("JwtId")]
        public IResult JwtId(ClaimsPrincipal user)
        {
            return Results.Ok(user.Claims.FirstOrDefault(p => p.Type == "jti")?.Value);
        }
    }
}
