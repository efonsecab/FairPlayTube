using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Paypal
{

    public class PaypalCheckoutApprovedDataModel
    {
        public string orderID { get; set; }
        public string payerID { get; set; }
        public object paymentID { get; set; }
        public object billingToken { get; set; }
        public string facilitatorAccessToken { get; set; }
    }

}
