using FoodDlvAPI.Interfaces;
using FoodDlvAPI.Models;
using FoodDlvAPI.Models.DTOs;
using FoodDlvAPI.Models.Services;
using FoodDlvAPI.Models.ViewModels;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Utils;
using MimeKit;
using Newtonsoft.Json;
using NuGet.Protocol.Core.Types;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Principal;
using static FoodDlvAPI.Models.Repositories.MemberRespitory;

namespace FoodDlvAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MembersController : Controller
	{
		private readonly MemberService memberservice;
		private readonly IConfiguration _configuration;
		private readonly EmailController _emailController;

		public MembersController(IConfiguration configuration, EmailController emailController)
		{
			var db = new AppDbContext();
			IMemberRepository repository = new MemberRepository(db);
			this.memberservice = new MemberService(repository);
			this._configuration = configuration;
			_emailController = emailController;
		}

		[HttpPost("register")]
		public async Task<ActionResult<string>> Register([FromForm] MemberRegisterVM member)
		{

			if (ModelState.IsValid)
			{
				try
				{
					//return await memberservice.RegisterAsync(member.ToMemberEditDto());
					var result = await memberservice.RegisterAsync(member.ToMemberEditDto());
					if (result == "新增成功")
					{
						await _emailController.SendEmail(member.ToMember());
					}
					return result;
				}
				catch (Exception ex)
				{
					throw new Exception(ex.Message);
				}
			}
			else
			{
				foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
				{
					ModelState.AddModelError(string.Empty, error.ErrorMessage);
				}
				return BadRequest(ModelState);
			}

		}

		[HttpPost("login")]
		public async Task<ActionResult<object>> Login(MemberLoginVM model)
		{
			MemberLoginresponse response = await memberservice.Login(model.Account, model.Password);

			if (response.IsSuccess)
			{
				object token = CreateToken(response);
				return Ok(token);

			}

			ModelState.AddModelError(string.Empty, response.ErrorMessage);

			return BadRequest(ModelState);
		}
		private object CreateToken(MemberLoginresponse response)
		{
			List<Claim> claims = new List<Claim>
			{
                //使用者的名字
                new Claim(ClaimTypes.Name, response.Username),
                //身分，在這裡可以是外送員、用戶、店家等等
                new Claim(ClaimTypes.Role, "Member"),
                //除此之外，Claim 還可以做非常多事情，請自己去查~
                new Claim(ClaimTypes.NameIdentifier,response.Id.ToString())
			};

			var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("Appsettings:Token").Value));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

			var token = new JwtSecurityToken
				(
				claims: claims,
				expires: DateTime.Now.AddDays(7),
				signingCredentials: creds
				);

			var jwt = new JwtSecurityTokenHandler().WriteToken(token);
			var userId = response.Id;
			var userAccount = response.Username;
			var role = "Member";

			return new { jwt, userId, userAccount, role } ;
		}
		// GET: api/Members/5
		[HttpGet("{id}")]
		public async Task<ActionResult<MemberDetailVM>> Detail(int id)
		{
			try
			{
				var data = await memberservice.GetmemberAsync(id);
				return data.ToMemberDetailVM();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
		[HttpPut("Edit/{id}")]
		public async Task<ActionResult<string>> Edit(MemberEditVM member)
		{
			if (ModelState.IsValid)
			{
				try
				{
					return await memberservice.EditAsync(member.ToMemberRegisterDto());
				}
				catch (Exception ex)
				{
					ModelState.AddModelError(string.Empty, ex.Message);
					return BadRequest(ModelState);
				}
			}
			else
			{
				foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
				{
					ModelState.AddModelError(string.Empty, error.ErrorMessage);
				}
				return BadRequest(ModelState);
			}
		}
		
	}
}
