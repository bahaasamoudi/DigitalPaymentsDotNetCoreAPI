using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPayments.Controllers
{
    [Route("[controller]")]
    public class NotificationsController : Controller
    {
        [HttpGet("[action]")]
        public IActionResult EmailConfirmed(string userId, string code)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                return Redirect("http://localhost:4200/login");

            }
            return View();
        }
    }
}
