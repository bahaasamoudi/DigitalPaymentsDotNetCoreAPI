
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DigitalPayments.Email;
using DigitalPayments.Helpers;
using DigitalPayments.Models;
using DigitalPayments.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using DigitalPayments.Models.Repositories;
using System.Globalization;

namespace DigitalPayments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppSettings _appSettings;
        private IEmailSender _emailsender;
        private readonly IDigitalPaymentsRepository<ApplicationUser, string> userRepository;
        private readonly IDigitalPaymentsRepository<Shop, string> shopRepository;
        private readonly IDigitalPaymentsRepository<Transfer, int> transferRepository;


        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
                                IOptions<AppSettings> appSettings, IEmailSender emailsender,
                                IDigitalPaymentsRepository<ApplicationUser, string> userRepository,
                                IDigitalPaymentsRepository<Shop, string> shopRepository,
                                IDigitalPaymentsRepository<Transfer, int> transferRepository)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _emailsender = emailsender;
            this.userRepository = userRepository;
            this.shopRepository = shopRepository;
            this.transferRepository = transferRepository;
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {

            List<string> errorList = new List<string>();

            if (!ModelState.IsValid)
            {
                errorList = Operations.ConvertModelStateToErrorsList(ModelState);

                return BadRequest(new JsonResult(errorList));
            }

          
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                RegisteredDate = DateTime.Now,
                Balance = 0,
                PhoneNumber = model.PhoneNumber,
                UserImage = "DefaultProfileImage.jpg",
                Country = model.Country,
                City = "",
                Gender = model.Gender,
                Birthdate = Convert.ToDateTime(model.BirthDate),
                SecurityStamp = Guid.NewGuid().ToString(),
                IdNumber = model.IdNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "user");
                // Sending Confirmation Email
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { UserId = user.Id, Code = code }, protocol: HttpContext.Request.Scheme);
                await _emailsender.SendEmailAsync(user.Email, "Confirm Your Email", "Please confirm your e-mail by clicking this link:" +
                                                                " <a href=\"" + callbackUrl + "\">click here</a>");
                return Ok(new { username = user.UserName, email = user.Email, status = 1, message = "Registration Successful" });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    errorList.Add(error.Description);
                }
            }

            return BadRequest(new JsonResult(errorList));

        }

        // Login Method
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            // Get the User from Database
            var user = await _userManager.FindByNameAsync(model.Username);

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret));

            double tokenExpiryTime = Convert.ToDouble(_appSettings.ExpireTime);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // THen Check If Email Is confirmed
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    return Unauthorized(new JsonResult(new []{ "We sent you an Confirmation Email.Please Confirm Your Registration To Log in." }));
                }

                var roles = await _userManager.GetRolesAsync(user);
                var tokenHandler = new JwtSecurityTokenHandler();

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, model.Username),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                        new Claim("LoggedOn", DateTime.Now.ToString()),

                     }),

                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _appSettings.Site,
                    Audience = _appSettings.Audience,
                    Expires = DateTime.UtcNow.AddMinutes(tokenExpiryTime)
                };

                // Generate Token

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return Ok(new { token = tokenHandler.WriteToken(token), expiration = token.ValidTo, username = user.UserName, userRole = roles.FirstOrDefault() });

            }

            return Unauthorized(new JsonResult(new[] { "Please Check the Login Credentials - Ivalid Username/Password was entered" }));

        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                return BadRequest(new JsonResult(new[] { "User Id and Code are required" }));
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new JsonResult("ERROR");
            }
            if (user.EmailConfirmed)
            {
                return Redirect("http://localhost:4200/login");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return RedirectToAction("EmailConfirmed", "Notifications", new { userId, code });
            }
            else
            {
                List<string> errors = new List<string>();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.ToString());
                }
                return BadRequest(new JsonResult(errors));
            }
        }


        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> GetUserInfo()
        {
            string userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = userRepository.List().Where(q => q.UserName.ToLower() == userName.ToLower()).FirstOrDefault();
            var role = "User";

            foreach (var item in await _userManager.GetRolesAsync(user))
            {
                role = item;
            }
            
            return Ok(new
            {
                id = user.Id,
                username = user.UserName,
                email = user.Email,
                balance = user.Balance,
                firstName = user.FirstName,
                lastName = user.LastName,
                gender = user.Gender,
                phoneNumber = user.PhoneNumber,
                idnumber = user.IdNumber,
                country = user.Country,
                birthdate = user.Birthdate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                role

            }
            );

        }

        [HttpPost("[action]")]
        [Authorize]
        public IActionResult ChangePersonalInformation([FromBody] ChangePersonalViewModel model)
        {

            if (!ModelState.IsValid)
            {
                var errors = Operations.ConvertModelStateToErrorsList(ModelState);
                return BadRequest(new JsonResult(errors));
            }

            string userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = userRepository.List().Where(q => q.UserName.ToLower() == userName.ToLower()).FirstOrDefault();
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.Gender = model.Gender;
            user.IdNumber = model.IdNumber;
            user.Country = model.Country;
            user.Birthdate = model.Birthdate;
            userRepository.Update(user);


            return Ok();

        }

        [HttpPost("[action]")]
        [Authorize(Roles ="shop")]
        public IActionResult ChangeShopInformation([FromBody] ChangeShopInformationViewModel model)
        {

            if (!ModelState.IsValid)
            {
                var errors = Operations.ConvertModelStateToErrorsList(ModelState);
                return BadRequest(new JsonResult(errors));
            }

            string userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var shop = shopRepository.List().Where(q => q.User.UserName == userName.ToLower()).FirstOrDefault();
            shop.Location = model.Location;
            shop.Phone = model.Phone;
            shop.ShopName = model.ShopName;
            shop.TypeOfService = model.TypeOfService;
            shop.WebSite = model.WebSite;
            shopRepository.Update(shop);
            return Ok();
        }



        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordVM model)
        {


            if (!ModelState.IsValid)
            {
                var errors = Operations.ConvertModelStateToErrorsList(ModelState);
                return BadRequest(new JsonResult(errors));
            }

            string userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = userRepository.List().Where(q => q.UserName.ToLower() == userName.ToLower()).FirstOrDefault();

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            var errorsSu = Operations.ConvertModelStateToErrorsList(ModelState);
            return BadRequest(new JsonResult(errorsSu));


        }

   
        [HttpGet("[action]")]
        [Authorize]
        public  IActionResult GetUserTransactions()
        {
            string userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = userRepository.List().Where(q => q.UserName.ToLower() == userName.ToLower()).FirstOrDefault();
            string userid = user.Id;

            var transafers = transferRepository.List().Where(q => q.TransferedFromId == userid || q.TransferedToId == userid).OrderByDescending(q => q.TransferDate).ToList();
            List<Object> transactions = new List<object>();
            
            if(transafers.Count() != 0)
            {
                foreach(var tra in transafers )
                {

                    if(tra.TransferedFromId == userid)
                    {
                       
                        transactions.Add(new { amount = "-" + tra.TransferAmount, date = tra.TransferDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) });
                    } else
                    {
                        transactions.Add(new { amount = "+" +  tra.TransferAmount, date = tra.TransferDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) });
                    }
                }
            }

            return Ok(transactions );

        }

      

    }
}