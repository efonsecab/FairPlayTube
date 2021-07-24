using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Models.Paypal
{

    public class PaypalCheckoutApprovedDetailsModel
    {
        public string id { get; set; }
        public string intent { get; set; }
        public string status { get; set; }
        public Purchase_Units[] purchase_units { get; set; }
        public Payer payer { get; set; }
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }
        public Link[] links { get; set; }
    }

    public class Payer
    {
        public Name name { get; set; }
        public string email_address { get; set; }
        public string payer_id { get; set; }
        public Address address { get; set; }
    }

    public class Name
    {
        public string given_name { get; set; }
        public string surname { get; set; }
    }

    public class Address
    {
        public string country_code { get; set; }
    }

    public class Purchase_Units
    {
        public string reference_id { get; set; }
        public Amount amount { get; set; }
        public Payee payee { get; set; }
        public string description { get; set; }
        public Item[] items { get; set; }
        public Shipping1 shipping { get; set; }
        public Payments payments { get; set; }
    }

    public class Amount
    {
        public string currency_code { get; set; }
        public string value { get; set; }
        public Breakdown breakdown { get; set; }
    }

    public class Breakdown
    {
        public Item_Total item_total { get; set; }
        public Shipping shipping { get; set; }
        public Handling handling { get; set; }
        public Tax_Total tax_total { get; set; }
        public Insurance insurance { get; set; }
        public Shipping_Discount shipping_discount { get; set; }
    }

    public class Item_Total
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class Shipping
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class Handling
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class Tax_Total
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class Insurance
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class Shipping_Discount
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class Payee
    {
        public string email_address { get; set; }
        public string merchant_id { get; set; }
    }

    public class Shipping1
    {
        public Name1 name { get; set; }
        public Address1 address { get; set; }
    }

    public class Name1
    {
        public string full_name { get; set; }
    }

    public class Address1
    {
        public string address_line_1 { get; set; }
        public string admin_area_2 { get; set; }
        public string admin_area_1 { get; set; }
        public string postal_code { get; set; }
        public string country_code { get; set; }
    }

    public class Payments
    {
        public Capture[] captures { get; set; }
    }

    public class Capture
    {
        public string id { get; set; }
        public string status { get; set; }
        public Amount1 amount { get; set; }
        public bool final_capture { get; set; }
        public Seller_Protection seller_protection { get; set; }
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }
    }

    public class Amount1
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class Seller_Protection
    {
        public string status { get; set; }
        public string[] dispute_categories { get; set; }
    }

    public class Item
    {
        public string name { get; set; }
        public Unit_Amount unit_amount { get; set; }
        public Tax tax { get; set; }
        public string quantity { get; set; }
    }

    public class Unit_Amount
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class Tax
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class Link
    {
        public string href { get; set; }
        public string rel { get; set; }
        public string method { get; set; }
    }

}
