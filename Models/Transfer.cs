using DigitalPayments.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DigitalPayments.Models
{
    public class Transfer
    {
        public int Id { get; set; }
        public string TransferedFromId { get; set; }
        public virtual ApplicationUser TransferedFrom { get; set; }
        public string TransferedToId { get; set; }
        public virtual ApplicationUser TransferedTo { get; set; }

        public decimal? TransferAmount { get; set; }


        public DateTime TransferDate { get; set; }


    }
}