using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DigitalPayments.Email;
using DigitalPayments.Helpers;
using DigitalPayments.Models;
using DigitalPayments.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DigitalPayments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IDigitalPaymentsRepository<ApplicationUser, string> userRepository;
        private readonly IDigitalPaymentsRepository<IdentityRole, string> rolesRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public UsersController(IDigitalPaymentsRepository<ApplicationUser, string> userRepository,
                             IDigitalPaymentsRepository<IdentityRole, string> rolesRepository,
                             UserManager<ApplicationUser> userManager)
        {
            this.userRepository = userRepository;
            this.rolesRepository = rolesRepository;
            _userManager = userManager;
        }

        [HttpGet("[action]")]
        [Authorize(Roles = "admin")]
        public IActionResult GetAllUsers()
        {
            var users = userRepository.List().Select(q => new {
                id = q.Id,
                balance = q.Balance,
                firstName = q.FirstName,
                lastName = q.LastName,
                email = q.Email,
                phoneNumber = q.PhoneNumber,
                country = q.Country,
                role = rolesRepository.Find(q.Roles.FirstOrDefault().RoleId).Name                   ,
                registeredDate = q.RegisteredDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            });


            return Ok(users);

        }

        [HttpPost("[action]")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ConvertUserToAdmin([FromBody] ConvertToAdminViewModel model)
        {


            if (!ModelState.IsValid)
            {
                var errors = Operations.ConvertModelStateToErrorsList(ModelState);
                return BadRequest(new JsonResult(errors));
            }


            var user = userRepository.Find(model.UserId);

            if (user == null)
            {
                return BadRequest(new JsonResult(new[] { "user Not Found" }));
            }


            var role = rolesRepository.Find(user.Roles.FirstOrDefault().RoleId).Name;

             await _userManager.RemoveFromRoleAsync(user, role);
             await _userManager.AddToRoleAsync(user, "admin");
            return Ok();

        }

    }
}