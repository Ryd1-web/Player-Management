using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Player_Management.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class JWTController : ControllerBase
	{
		private IConfiguration _configuration;

		public JWTController(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public class Users
		{
			public string username { get; set; }
			public string password { get; set; }
		}

		public class Jwt
		{
			public string key { get; set; }
			public string Issuer { get; set; }
			public string Audience { get; set; }
			public string Subject { get; set; }

		}


		[HttpPost]
		public async Task<IActionResult> generateToken(Users users)
		{
			string secretKey = "SkFabTZibXE1aE14ckpQUUxHc2dnQ2RzdlFRTTM2NFE2cGI4d3RQNjZmdEFITmdBQkE=";
			if (users != null && users.username == "ridwan" && users.password == "ridwan")
			{
				//var userdata = await _jwtServices.ValidateUser(users.username, users.password);
				var jwt = _configuration.GetSection("Jwt").Get<Jwt>();
				if (users != null)
				{
					var claims = new[]
					{
						new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
						new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
						new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
						new Claim("UserName", users.username),
						new Claim("Password", users.password),
					};
					var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.key));
					var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
					var token = new JwtSecurityToken(
						jwt.Issuer,
						jwt.Audience,
						claims,
						expires: DateTime.Now.AddSeconds(60),
						signingCredentials: signIn
						);
					//return Ok(new JwtSecurityTokenHandler().WriteToken(token));
					return Ok(secretKey);
				}
				else
				{
					return BadRequest("Invalid Credentials");
				}

			}
			else
			{
				return BadRequest("invalid credentials");
			}
		}

	}
}
