using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPayments.Models;
using DigitalPayments.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DigitalPayments.Models.Repositories;
using DigitalPayments.Helpers;
using Microsoft.AspNetCore.Identity;

namespace DigitalPayments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IDigitalPaymentsRepository<ApplicationUser, string> userRepository;
        private readonly IDigitalPaymentsRepository<Transfer, int> transferRepository;
        private readonly IDigitalPaymentsRepository<Barcode, int> barcodeRepository;
        private readonly IDigitalPaymentsRepository<IdentityRole, string> roleRepository;
        private readonly IDigitalPaymentsRepository<Shop, string> shopRepository;
        private readonly IDigitalPaymentsRepository<Bill, int> billRepository;


        public TransactionsController(IDigitalPaymentsRepository<Transfer, int> transferRepository,
            IDigitalPaymentsRepository<ApplicationUser, string> userRepository,
            IDigitalPaymentsRepository<Barcode, int> barcodeRepository,
            IDigitalPaymentsRepository<IdentityRole, string> roleRepository,
            IDigitalPaymentsRepository<Shop, string> shopRepository,
            IDigitalPaymentsRepository<Bill, int> billRepository)
        {
            this.userRepository = userRepository;
            this.barcodeRepository = barcodeRepository;
            this.transferRepository = transferRepository;
            this.roleRepository = roleRepository;
            this.shopRepository = shopRepository;
            this.billRepository = billRepository;
        }

        // POST: api/Transaction/GenerateBarcode
        [HttpPost("[action]")]
        [Authorize(Roles = "shop, admin")]
        public ActionResult GenerateBarcode(GenerateBarcodeVM model)
        {
     
            string userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = userRepository.List().Where(q => q.UserName.ToLower() == userName.ToLower()).FirstOrDefault();

            if (ModelState.IsValid)
            {
                string[] uses = { "purchase", "charge" };

                if(!uses.Contains(model.UsedFor.Trim().ToLower()))
                {
                    return BadRequest(new JsonResult(new[] { $"UsedFor must be one of these {uses.ToString() }" }));
                }

                if (user.Balance < model.Amount)
                {
                    return BadRequest(new JsonResult(new[] { "you Don't Have Enough Money to generate barcode with this amount " }));
                }


                Random radnom = new Random();
                int randomNumber = radnom.Next(1, 199999999) + model.Amount;

                string code = randomNumber + "";
                Barcode barcode = new Barcode();
                barcode.Active = true;
                barcode.Code = code;
                barcode.Amount = model.Amount;
                barcode.GeneratedDate = DateTime.Now;
                barcode.UserId = user.Id;
                barcode.UserFor = model.UsedFor;

                barcodeRepository.Add(barcode);

                return Ok(new { code = code });
            }
            else
            {
                var errors = Operations.ConvertModelStateToErrorsList(ModelState);
                return BadRequest(new JsonResult(errors));
            }
        }



        // POST: api/Transaction/Charge
        [HttpPost("[action]")]
        [Authorize]
        public ActionResult Charge(BarcodeVM barcodeVM)
        {
            return ChargeOrPurchase(barcodeVM, "charge");
        }


        // POST: api/Transaction/Purchase
        [HttpPost("[action]")]
        [Authorize]
        public ActionResult Purchase(BarcodeVM barcodeVM)
        {
            return ChargeOrPurchase(barcodeVM, "purchase");
        }


        private ActionResult ChargeOrPurchase(BarcodeVM barcodeVM, string ChargeOrPurchase)
        {

            if (!ModelState.IsValid)
            {
                var errors = Operations.ConvertModelStateToErrorsList(ModelState);
                return BadRequest(new JsonResult(errors));
            }

            var barcode = barcodeRepository.List().Where(q => q.Code == barcodeVM.Barcode).ToList().FirstOrDefault();
            if (barcode != null && barcode.Active != false)
            {
                TimeSpan difference = DateTime.Now - barcode.GeneratedDate;
                if (difference.Minutes >= 1)
                {
                    barcode.Active = false;
                }

            }

            if (barcode == null || barcode.Active == false)
            {
                return BadRequest(new JsonResult(new[] { "The Bardcode Not Active" }));
            }

            barcode.Active = false;
            barcodeRepository.Update(barcode);

            // Current User
            var user = userRepository.List().Where(q => q.UserName.ToLower() == User.FindFirstValue(ClaimTypes.NameIdentifier).ToLower()).FirstOrDefault();
           
            if (user.Id == barcode.UserId)
            {
                return BadRequest(new JsonResult(new[] { "You Cannot Charge/Purchase To Yourself" }));
            }

            ushort amount = barcode.Amount;
            Transfer transfer = new Transfer();
            transfer.TransferAmount = amount;

            if (ChargeOrPurchase == "purchase")
            {


                if (user.Balance < amount) // Not Have A Money To Purchase
                {
                    return BadRequest(new JsonResult(new[] { "You don't have enough money to purchase" }));
                }

                if(barcode.UserFor != "purchase")
                {
                    return BadRequest(new JsonResult(new[] { "You cannot use this barcode to purchase operation" }));
                }


                transfer.TransferedFromId = user.Id;
                transfer.TransferedToId = barcode.User.Id;
                user.Balance -= amount;
                barcode.User.Balance += amount;
                userRepository.Update(barcode.User);

                
                // اذا كان في فاتورة للشهر هذا جيبها
                var getLastBill = billRepository.List().Where(q => q.ShopId == barcode.User.Id && q.Month == DateTime.Now.Month && q.Year == DateTime.Now.Year).FirstOrDefault();

                if(getLastBill != null)
                {
                    getLastBill.Amount += amount * (Decimal)0.01;
                    billRepository.Update(getLastBill);
                }
                else // ضيف وحدة جديدة
                {
                 

                    DateTime thisday = DateTime.Now;
                    Bill bill = new Bill();
                    bill.Amount = amount * (Decimal)0.01;
                    bill.Month = thisday.Month;
                    bill.Year = thisday.Year;
                    bill.ShopId = barcode.User.Id;
                    bill.Paid = false;
                    bill.DueDate = new DateTime(thisday.AddMonths(1).Year, thisday.AddMonths(1).Month, 1); // First date From Next Month
                    billRepository.Add(bill);
                }
               

            }
            else if(ChargeOrPurchase == "charge")
            {

                if (barcode.UserFor != "charge")
                {
                    return BadRequest(new JsonResult(new[] { "You cannot use this barcode to charge operation" }));
                }

                if (barcode.User.Balance < amount)
                {
                    return BadRequest(new JsonResult(new[] { "The person who generate this barcode not have enough  money" }));

                }


                transfer.TransferedFromId = barcode.User.Id;
                transfer.TransferedToId = user.Id;
                user.Balance += amount;
                barcode.User.Balance -= amount;
                userRepository.Update(barcode.User);
            }
            userRepository.Update(user);

            transfer.TransferDate = DateTime.Now;
            transferRepository.Add(transfer);
            return Ok();
        }


        //POST: api/Transaction/GenerateBarcode
       [HttpPost("[action]")]
       [Authorize(Roles = "admin")]
        public ActionResult GetBill(GetBillViewModel model)
        {

            if (ModelState.IsValid)
            {

                string userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = userRepository.List().Where(q => q.UserName.ToLower() == userName.ToLower()).FirstOrDefault();

                var bill = billRepository.Find(model.BillId);

                if(bill == null)
                {
                    return BadRequest(new JsonResult(new[] { "Bill Does Not Exist" }));
                }

                bill.Paid = true;
                billRepository.Update(bill);
                return Ok();
            }
            else
            {
                var errors = Operations.ConvertModelStateToErrorsList(ModelState);
                return BadRequest(new JsonResult(errors));
            }
        }



    }
}
