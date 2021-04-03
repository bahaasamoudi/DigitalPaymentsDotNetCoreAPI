using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalPayments.Helpers;
using DigitalPayments.Models;
using DigitalPayments.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DigitalPayments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController :  ControllerBase
    {
        private readonly IDigitalPaymentsRepository<Transfer, int> transferRepository;
        private readonly IDigitalPaymentsRepository<IdentityRole, string> roleRepository;
        private readonly IDigitalPaymentsRepository<Shop, string> shopRepository;
        private readonly IDigitalPaymentsRepository<Bill, int> billRepository;
        public ReportsController(  IDigitalPaymentsRepository<Transfer, int> transferRepository,
                                    IDigitalPaymentsRepository<IdentityRole, string> roleRepository,
                                    IDigitalPaymentsRepository<Shop, string> shopRepository,
                                     IDigitalPaymentsRepository<Bill, int> billRepository)
        {
            this.transferRepository = transferRepository;
            this.roleRepository = roleRepository;
            this.shopRepository = shopRepository;
            this.billRepository = billRepository;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("[action]")]
        public IActionResult GetProfits()
        {
            var getShopRoleId = roleRepository.List().Where(q => q.Name == "shop").ToList().FirstOrDefault().Id;
            var x = transferRepository.List().Where(q => q.TransferedTo.Roles.FirstOrDefault().RoleId == getShopRoleId);
            var transfers = transferRepository.List().Where(q => q.TransferedTo.Roles.FirstOrDefault().RoleId == getShopRoleId)
                .GroupBy(o => new
                {
                    week = DateTimeExtensions.GetWeekOfMonth(o.TransferDate),
                    month = o.TransferDate.Month,
                    year = o.TransferDate.Year,
                }).Select(g => new ReportViewModel
                {
                    Week = g.Key.week,
                    Month = g.Key.month,
                    Year = g.Key.year,
                    Money = g.Sum(q => q.TransferAmount) * (Decimal)0.01 // 1% From Profits
                })
                .OrderByDescending(a => a.Year)
                .ThenByDescending(a => a.Month)
                .ThenByDescending(a => a.Week)
                .ToList();
            
            return Ok(transfers);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("[action]")]
        public IActionResult GetBillsForShops()
        {
            var bills = billRepository.List().Where(q => q.Paid == false && q.DueDate < DateTime.Now)
                .GroupBy(o => new
                {
                    month = o.Month,
                    year = o.Year,
                    shopid = o.ShopId
                })
                .Select(g => new ProfitsViewModel
                {
                    BillId = g.Select(q => q.Id).FirstOrDefault() ,
                    ShopId = g.Key.shopid,
                    ShopName = g.Select(q => q.Shop.ShopName).FirstOrDefault(),
                    ShopPhone = g.Select(q => q.Shop.Phone).FirstOrDefault(),
                    Year = g.Key.year,
                    Month = g.Key.month,
                    Receivables = g.Select(q => q.Amount).FirstOrDefault(),
                })
                .OrderByDescending(q => q.Year)
                .OrderByDescending(q => q.Month)
                .ToList();

            return Ok(bills);
        }
    }
}