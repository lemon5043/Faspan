using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodDlvAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FoodDlvAPI.Models.DTOs;
using FoodDlvAPI.Models.ViewModels;

namespace FoodDlvAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorePrincipalsController : ControllerBase
    {
        private readonly AppDbContext _context;
		private readonly IConfiguration _configuration;

		public StorePrincipalsController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

		}

		[HttpPost("login")]
		public async Task<ActionResult<string>> Login(StorePrincipalLoginVM model)
		{
			StorePrincipalLoginresponse response = await Login(model.Account, model.Password);


			if (response.IsSuccess)
			{
				string token = CreateToken(response);
				return Ok(token);

			}

			ModelState.AddModelError(string.Empty, response.ErrorMessage);

			return response.ErrorMessage;
		}
		private async Task<StorePrincipalLoginresponse> Login(string account, string password)
		{
			StorePrincipal storePrincipal = await _context.StorePrincipals.Where(x => x.Account == account && x.Password == password).SingleOrDefaultAsync();

			if (storePrincipal == null)
			{
				return StorePrincipalLoginresponse.Fail("帳密有誤");
			}
			return StorePrincipalLoginresponse.Success(storePrincipal.Id, storePrincipal.FirstName + storePrincipal.LastName);
		}
		private string CreateToken(StorePrincipalLoginresponse response)
		{
			List<Claim> claims = new List<Claim>
			{
                //使用者的名字
                new Claim(ClaimTypes.Name, response.Username),
                //身分，在這裡可以是外送員、用戶、店家等等
                new Claim(ClaimTypes.Role, "Store"),
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

			return jwt;
		}

		// GET: api/StorePrincipals
		//[HttpGet]
  //      public async Task<ActionResult<IEnumerable<StorePrincipal>>> GetStorePrincipals()
  //      {
  //          return await _context.StorePrincipals.ToListAsync();
  //      }

  //      // GET: api/StorePrincipals/5
  //      [HttpGet("{id}")]
  //      public async Task<ActionResult<StorePrincipal>> GetStorePrincipal(int id)
  //      {
  //          var storePrincipal = await _context.StorePrincipals.FindAsync(id);

  //          if (storePrincipal == null)
  //          {
  //              return NotFound();
  //          }

  //          return storePrincipal;
  //      }


		[HttpGet("GetStorePrincipalEditVM{id}")]
		public async Task<ActionResult<StorePrincipalEditVM>> GetStorePrincipalEditVM(int id)
		{
			StorePrincipalEditVM storePrincipalEditVM=new StorePrincipalEditVM();
			try
			{
				
				var storePrincipal = await _context.StorePrincipals.FirstOrDefaultAsync(x => x.Id == id);

				storePrincipalEditVM.FirstName = storePrincipal.FirstName;
				storePrincipalEditVM.LastName = storePrincipal.LastName;
				storePrincipalEditVM.Phone = storePrincipal.Phone;
				storePrincipalEditVM.Gender = storePrincipal.Gender;
				storePrincipalEditVM.Birthday = storePrincipal.Birthday;
				storePrincipalEditVM.Email = storePrincipal.Email;
				storePrincipalEditVM.Password = storePrincipal.Password;

				_context.StorePrincipals.Update(storePrincipal);
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}

			return storePrincipalEditVM;
		}








		// PUT: api/StorePrincipals/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("storePrincipalEditVM{id}")]
		public async Task<string> PutStorePrincipal(int id, StorePrincipalEditVM storePrincipalEditVM)
		{
			try {

				var storePrincipal = await _context.StorePrincipals.FirstOrDefaultAsync(x => x.Id == id);
				
				storePrincipal.FirstName = storePrincipalEditVM.FirstName;
				storePrincipal.LastName = storePrincipalEditVM.LastName;
				storePrincipal.Phone = storePrincipalEditVM.Phone;
				storePrincipal.Gender = storePrincipalEditVM.Gender;
				storePrincipal.Birthday = storePrincipalEditVM.Birthday;
				storePrincipal.Email = storePrincipalEditVM.Email;
				storePrincipal.Password = storePrincipalEditVM.Password;

				_context.StorePrincipals.Update(storePrincipal);
				await _context.SaveChangesAsync();
			} 
			catch (Exception ex)
			{ 
				throw new Exception(ex.Message);
			}

			return "修改成功";

		}

		// POST: api/StorePrincipals
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
        public async Task<string> PostStorePrincipal(StorePrincipalCreateVM storePrincipalCreateVM)
        {
			try 
			{
				var storePrincipal = new StorePrincipal
				{
					AccountStatusId = 1,
					FirstName = storePrincipalCreateVM.FirstName,
					LastName = storePrincipalCreateVM.LastName,
					Phone = storePrincipalCreateVM.Phone,
					Gender = storePrincipalCreateVM.Gender,
					Birthday = storePrincipalCreateVM.Birthday,
					Email = storePrincipalCreateVM.Email,
					Account = storePrincipalCreateVM.Account,
					Password = storePrincipalCreateVM.Password,
					RegistrationTime = DateTime.Now
				};
				_context.StorePrincipals.Add(storePrincipal);
				await _context.SaveChangesAsync();
			}
			catch(Exception ex)
			{
				throw new Exception(ex.Message);
			}

			return "註冊成功";
			//_context.StorePrincipals.Add(storePrincipal);
			//await _context.SaveChangesAsync();

			//return CreatedAtAction("GetStorePrincipal", new { id = storePrincipal.Id }, storePrincipal);
		}

        //// DELETE: api/StorePrincipals/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteStorePrincipal(int id)
        //{
        //    var storePrincipal = await _context.StorePrincipals.FindAsync(id);
        //    if (storePrincipal == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.StorePrincipals.Remove(storePrincipal);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool StorePrincipalExists(int id)
        //{
        //    return _context.StorePrincipals.Any(e => e.Id == id);
        //}
    }
}
