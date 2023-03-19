using FoodDlvAPI.Infrastructures;
using FoodDlvAPI.Models;
using FoodDlvAPI.Models.DTOs;
using Google.Apis.Auth;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Text;
using MimeKit.Utils;
using System.Security.Claims;
using System.Security.Principal;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace FoodDlvAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EmailController : ControllerBase
	{
		private readonly AppDbContext _context;

		public EmailController(AppDbContext context)
		{
			_context = context;
		}
		[HttpPost("SendEmail")]
		public async Task<ActionResult> SendEmail(Member member)
		{

            var message = new MimeMessage();
			// 寄件者
			message.From.Add(new MailboxAddress("FASPAN!", "testfuen25@gmail.com"));
			// 收件者
			message.To.Add(new MailboxAddress("新會員", $"{member.Account}"));

			message.Subject = "歡迎註冊FASPAN";

			// 產生驗證碼
			var confirmCode = Guid.NewGuid().ToString();

			// 使用 BodyBuilder 建立郵件內容
			var bodyBuilder = new BodyBuilder();

			// 產生驗證連結
			string confirmationLink = $"{Request.Scheme}://{Request.Host}/api/Email/EmailConfirm?memberid={member.Id}";

			// 設定 HTML 內容
			bodyBuilder.HtmlBody = "<h1 style='text-align: center; color:darkgreen; font-size:60px;'>FASPAN!</h1>" +
									"<br>" +
									"<h3 style='text-align: center;font-size:30px;'>歡迎使用FASPAN!!</h3>" +
									"<h3 style='text-align: center;font-size:30px;'>請點擊下方連結以驗證信箱</h3>" +
									"<p style='text-align: center;'>" +
									$"<a style= font-size:20px;' href=\"{confirmationLink}\">-----點此連結驗證------</a>" +
									"</p>" +
									"<br>";
			// 設定郵件內容
			message.Body = bodyBuilder.ToMessageBody();

			using (var client = new SmtpClient())
			{
				client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
				client.Authenticate("testfuen25@gmail.com", "pymjkruxgrfjbisy");
				client.Send(message);
				client.Disconnect(true);
			}
			return Ok("成功");

		}
		private static bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			// Ignore certificate errors
			return true;
		}




		[HttpGet("EmailConfirm")]
		public async Task<ActionResult> EmailConfirm(int memberid)
		{
			// 從資料庫中找出符合帳號及驗證碼的會員資料
			var member = await _context.Members
				.SingleOrDefaultAsync(m => m.Id == memberid);

			// 如果找到會員資料，則將會員狀態改為已驗證
			if (member != null)
			{
				member.AccountStatusId = 2; // 已驗證
				await _context.SaveChangesAsync();
				return Ok("您的電子郵件已驗證成功！");
			}
			else
			{
				return BadRequest("找不到符合的會員資料，請確認連結是否正確！");
			}
		}
		[HttpPost("ForgetPassword")]
		public string ForgetPassword(ForgetPasswordDTO ForgePWMember)
		{
			var member = _context.Members.SingleOrDefault(x => x.Account == ForgePWMember.Account);
			if (member == null) 
			{ return "信箱輸入錯誤"; 
			}
			else
			{
				string newpassword = Guid.NewGuid().ToString("N").Substring(0, 8);
				SendForgetPasswordEmail(member, newpassword);
				member.Password = HashUtility.ToSHA256(newpassword, MemberRegisterDto.SALT);
				_context.SaveChanges();
				return "已寄出重設密碼電子郵件";
			}
		}

		[HttpPost("SendForgetPasswordEmail")]
		public async Task<ActionResult> SendForgetPasswordEmail(Member member,string newPassword)
		{
			var message = new MimeMessage();
			// 寄件者
			message.From.Add(new MailboxAddress("FASPAN!", "testfuen25@gmail.com"));
			// 收件者
			message.To.Add(new MailboxAddress("新會員", $"{member.Account}"));

			message.Subject = "FASPAN忘記密碼";
			var bodyBuilder = new BodyBuilder();

			string forgetpasswordlink = $"{Request.Scheme}://{Request.Host}/api/Members/Login/Account={member.Account}newPassword={member.Password}";

			// 設定 HTML 內容
			bodyBuilder.HtmlBody = "<h1 style='text-align: center; color:darkgreen; font-size:60px;'>FASPAN!</h1>" +
									"<br>" +
									"<h3 style='text-align: center;font-size:30px;'>歡迎使用FASPAN,以下是您的新密碼,請使用新密碼登入後至個人資料修改密碼</h3>" +
									"<h3 style='text-align: center;font-size:30px;'>以下是您的新密碼,請使用新密碼重新登入</h3>" +
									$"<p  style='text-align: center;font-size:30px;'>新密碼:{newPassword}</p>" +
									"<p style='text-align: center;'>" +
									
									"</p>" +
									"<br>";
			// 設定郵件內容
			message.Body = bodyBuilder.ToMessageBody();

			using (var client = new SmtpClient())
			{
				client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
				client.Authenticate("testfuen25@gmail.com", "pymjkruxgrfjbisy");
				client.Send(message);
				client.Disconnect(true);
			}
			return Ok("成功");


		}
		[HttpPost("ResetPassword")]
		public string ResetPassword(ResetPasswordDTO model)
		{

			var member = _context.Members.SingleOrDefault(x => x.Id == model.memberid);
			var encryptedPassword = HashUtility.ToSHA256(model.OldPassword, MemberRegisterDto.SALT);
			if (member == null)
			{
				return "帳號或密碼輸入錯誤";
			}
			else if (string.Compare(member.Password, encryptedPassword) == 0)
			{
				member.Password = HashUtility.ToSHA256(model.NewPassword, MemberRegisterDto.SALT);
				_context.SaveChanges();
				return "修改成功";
			}
			else
			{
				return "帳號或密碼輸入錯誤";
			}
		}

		/// <summary>
		/// 驗證 Google 登入授權
		/// </summary>
		/// <returns></returns>
		[HttpPost("GoogleLogin")]
		public IActionResult ValidGoogleLogin()
		{

                //< div id = "g_id_onload"

                //     data - client_id = "1002573051499-qgpbgbhkk9hdd3f73om9ahpbj4apr37f.apps.googleusercontent.com"

                //     data - login_uri = "https://localhost:7093/api/Email/ValidGoogleLogin"

                //     data - auto_prompt = "false" >

                //</ div >
            string ? formCredential = Request.Form["credential"]; //回傳憑證
			string? formToken = Request.Form["g_csrf_token"]; //回傳令牌
			string? cookiesToken = Request.Cookies["g_csrf_token"]; //Cookie 令牌

			// 驗證 Google Token
			GoogleJsonWebSignature.Payload? payload = VerifyGoogleToken(formCredential, formToken, cookiesToken).Result;
			if (payload == null)
			{
				return Redirect("https://localhost:5129/Login");
			}
			else

			{
				bool isExist = _context.Members.Any(x => x.Account == payload.Email);

				if (isExist == false)
				{
					Member member = new Member
					{	FirstName= payload.Name,
						LastName =payload.FamilyName,
						AccountStatusId = 1,
						Account = payload.Email,
						Password = "",
						Balance=0,
						RegistrationTime = DateTime.Now,
					};
					_context.Members.Add(member);
					_context.SaveChanges();
				}

				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name,payload.Email),
					new Claim("email",payload.Email),

				};

				var cookieOptions = new CookieOptions
				{
					HttpOnly = true,
					SameSite = SameSiteMode.None,
					Secure = true,
				};
				var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
				HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));


				return Redirect("https://localhost:5129/Login");
			}


		}
		/// <summary>
		/// 驗證 Google Token
		/// </summary>
		/// <param name="formCredential"></param>
		/// <param name="formToken"></param>
		/// <param name="cookiesToken"></param>
		/// <returns></returns>
		[HttpPost("VerifyGoogle")]
		public async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(string? formCredential, string? formToken, string? cookiesToken)
		{
			// 檢查空值
			if (formCredential == null || formToken == null && cookiesToken == null)
			{
				return null;
			}

			GoogleJsonWebSignature.Payload? payload;
			try
			{
				// 驗證 token
				if (formToken != cookiesToken)
				{
					return null;
				}

				// 驗證憑證
				IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
				string GoogleApiClientId = Config.GetSection("GoogleApiClientId").Value;
				var settings = new GoogleJsonWebSignature.ValidationSettings()
				{
					Audience = new List<string>() { GoogleApiClientId }
				};
				payload = await GoogleJsonWebSignature.ValidateAsync(formCredential, settings);
				if (!payload.Issuer.Equals("accounts.google.com") && !payload.Issuer.Equals("https://accounts.google.com"))
				{
					return null;
				}
				if (payload.ExpirationTimeSeconds == null)
				{
					return null;
				}
				else
				{
					DateTime now = DateTime.Now.ToUniversalTime();
					DateTime expiration = DateTimeOffset.FromUnixTimeSeconds((long)payload.ExpirationTimeSeconds).DateTime;
					if (now > expiration)
					{
						return null;
					}
				}
			}
			catch
			{
				return null;
			}
			return payload;
		}


	}
}
	

