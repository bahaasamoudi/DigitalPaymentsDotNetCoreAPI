using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DigitalPayments.Helpers;
using DigitalPayments.Models;
using DigitalPayments.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPayments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopsController : ControllerBase
    {

        private readonly IDigitalPaymentsRepository<Shop, string> shopRepository;
        private readonly IDigitalPaymentsRepository<Bill, int> receivableRepository;
        private readonly IDigitalPaymentsRepository<ApplicationUser, string> userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public ShopsController(IDigitalPaymentsRepository<Shop, string> shopRepository,
            IDigitalPaymentsRepository<Bill, int> receivableRepository,
            IDigitalPaymentsRepository<ApplicationUser, string> userRepository,
            UserManager<ApplicationUser> userManager)
        {
             this.shopRepository = shopRepository;
             this.receivableRepository = receivableRepository;
             this.userRepository = userRepository;
            _userManager = userManager;

        }

        [HttpGet("[action]")]
        public IActionResult GetAllShops()
        {

            var shopes = shopRepository.List()
                .Select(q => new { q.Description, q.Id, q.Location, q.Phone, q.ShopName, q.TypeOfService, q.WebSite, q.UserId })
                .ToList();

            return Ok(shopes);

        }

        [HttpPost("[action]")]
        public IActionResult SearchShopes(SeaechShopViewModel vmodel)
        {
            if(vmodel.SearchText == null ||  vmodel.SearchText == "")
            {
                return GetAllShops();
            }

            string searchtext = vmodel.SearchText.Trim().ToLower();

            var shopes = shopRepository.List()
                .Where(q => q.ShopName.ToLower().Contains(searchtext) || q.Description.ToLower().Contains(searchtext) || q.TypeOfService.ToLower().Contains(searchtext) || q.Location.ToLower().Contains(searchtext))
                .Select(q => new { q.Description, q.Id, q.Location, q.Phone, q.ShopName, q.TypeOfService, q.WebSite, q.UserId })
                .ToList();

            return Ok(shopes);

        }

        [HttpPost("[action]")]
        [Authorize(Roles = "user")]
        public IActionResult AddShop([FromBody] AddShopViewModel model)
        {

            if (!ModelState.IsValid)
            {
                var errors = Operations.ConvertModelStateToErrorsList(ModelState);
                return BadRequest(new JsonResult(errors));
            }

            string userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = userRepository.List().Where(q => q.UserName.ToLower() == userName.ToLower()).FirstOrDefault();

            Shop checkShopExist = shopRepository.Find(user.Id);

            if(checkShopExist != null)
            {
                shopRepository.Delete(checkShopExist.Id);
            }

            Shop shop = new Shop();
            shop.Id = user.Id;
            shop.ShopName = model.ShopName;
            shop.TypeOfService = model.TypeOfService;
            shop.WebSite = model.ShopWebsite;
            shop.Phone = model.ShopPhone;
            shop.Description = model.Description;
            shop.Location = model.ShopLocation;
            shop.UserId = user.Id;
            shop.Accepted = false;

            shopRepository.Add(shop);



            return Ok();


        }

        [HttpPost("[action]")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AcceptShop([FromBody] AcceptShopViewModel model)
        {

            if (!ModelState.IsValid)
            {
                var errors = Operations.ConvertModelStateToErrorsList(ModelState);
                return BadRequest(new JsonResult(errors));
            }

            var shop = shopRepository.Find(model.ShopId);

            if(shop == null)
            {
                return BadRequest(new JsonResult(new[] { "Shop Not Found" }));
            }

            shop.Accepted = true;
            shopRepository.Update(shop);


            await _userManager.RemoveFromRoleAsync(shop.User, "user");
            await _userManager.AddToRoleAsync(shop.User, "shop");

            return Ok();


        }

        [Authorize(Roles = "admin")]
        [HttpGet("[action]")]
        public IActionResult GetNotAccesptedShops()
        {
            var bills = shopRepository.List().Where(q => q.Accepted == false).ToList();
            return Ok(bills);
        }


        [Authorize(Roles = "shop")]
        [HttpGet("[action]")]
        public IActionResult GetShopInformation()
        {
            string userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var shop = shopRepository.List().Where(q => q.User.UserName == userName.ToLower()).FirstOrDefault();


            return Ok(new
            {
                id = shop.Id,
                shopName = shop.ShopName,
                location = shop.Location,
                phone = shop.Phone,
                typeOfService = shop.TypeOfService,
                website = shop.WebSite,
                description = shop.Description,
            });
        }
               





    }
}