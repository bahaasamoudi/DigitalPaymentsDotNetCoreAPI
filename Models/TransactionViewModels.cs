using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DigitalPayments.Models
{
    public class BarcodeVM
    {
        [Required]
        public string Barcode { get; set; }
    }

    public class GenerateBarcodeVM
    {
        [Required]
        public ushort Amount { get; set; }
        [Required]
        public string UsedFor { get; set; }
    }

    public class GetBillViewModel
    {
        [Required]
        public int BillId { get; set; }

    }
}