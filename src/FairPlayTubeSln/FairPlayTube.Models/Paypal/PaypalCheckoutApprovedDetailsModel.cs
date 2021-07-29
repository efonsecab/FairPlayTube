using System;

namespace FairPlayTube.Models.Paypal
{

    /// <summary>
    /// Represents the PayPal Checkout Approval Details
    /// </summary>
    public class PaypalCheckoutApprovedDetailsModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// Intent
        /// </summary>
        public string intent { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// Purchase Units
        /// </summary>
        public Purchase_Units[] purchase_units { get; set; }
        /// <summary>
        /// Payer
        /// </summary>
        public Payer payer { get; set; }
        /// <summary>
        /// Create DateTime
        /// </summary>
        public DateTime create_time { get; set; }
        /// <summary>
        /// Update DateTime
        /// </summary>
        public DateTime update_time { get; set; }
        /// <summary>
        /// Links
        /// </summary>
        public Link[] links { get; set; }
    }

    /// <summary>
    /// Represents the PayPal Payer
    /// </summary>
    public class Payer
    {
        /// <summary>
        /// Name
        /// </summary>
        public Name name { get; set; }
        /// <summary>
        /// Email Address
        /// </summary>
        public string email_address { get; set; }
        /// <summary>
        /// Payer Id
        /// </summary>
        public string payer_id { get; set; }
        /// <summary>
        /// Address
        /// </summary>
        public Address address { get; set; }
    }

    /// <summary>
    /// Represents the Name
    /// </summary>
    public class Name
    {
        /// <summary>
        /// Given Name
        /// </summary>
        public string given_name { get; set; }
        /// <summary>
        /// Surname
        /// </summary>
        public string surname { get; set; }
    }

    /// <summary>
    /// Represents the Address
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Country Code
        /// </summary>
        public string country_code { get; set; }
    }

    /// <summary>
    /// Purchase Units
    /// </summary>
    public class Purchase_Units
    {
        /// <summary>
        /// Reference Id
        /// </summary>
        public string reference_id { get; set; }
        /// <summary>
        /// Amount
        /// </summary>
        public Amount amount { get; set; }
        /// <summary>
        /// Payee
        /// </summary>
        public Payee payee { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// Items
        /// </summary>
        public Item[] items { get; set; }
        /// <summary>
        /// Shipping
        /// </summary>
        public Shipping1 shipping { get; set; }
        /// <summary>
        /// Payments
        /// </summary>
        public Payments payments { get; set; }
    }

    /// <summary>
    /// Represents the Amount
    /// </summary>
    public class Amount
    {
        /// <summary>
        /// Currency Code
        /// </summary>
        public string currency_code { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// Breakdown
        /// </summary>
        public Breakdown breakdown { get; set; }
    }

    /// <summary>
    /// Represents the Breakdown
    /// </summary>
    public class Breakdown
    {
        /// <summary>
        /// Item Total
        /// </summary>
        public Item_Total item_total { get; set; }
        /// <summary>
        /// Shipping
        /// </summary>
        public Shipping shipping { get; set; }
        /// <summary>
        /// Handling
        /// </summary>
        public Handling handling { get; set; }
        /// <summary>
        /// Tax Total
        /// </summary>
        public Tax_Total tax_total { get; set; }
        /// <summary>
        /// Insurance
        /// </summary>
        public Insurance insurance { get; set; }
        /// <summary>
        /// Shipping Discount
        /// </summary>
        public Shipping_Discount shipping_discount { get; set; }
    }

    /// <summary>
    /// Represents the Item Total
    /// </summary>
    public class Item_Total
    {
        /// <summary>
        /// Currency Code
        /// </summary>
        public string currency_code { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public string value { get; set; }
    }

    /// <summary>
    /// Shipping
    /// </summary>
    public class Shipping
    {
        /// <summary>
        /// Currency Code
        /// </summary>
        public string currency_code { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public string value { get; set; }
    }

    /// <summary>
    /// Represents the Handling
    /// </summary>
    public class Handling
    {
        /// <summary>
        /// Currency Code
        /// </summary>
        public string currency_code { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public string value { get; set; }
    }

    /// <summary>
    /// Represents the Tax Total
    /// </summary>
    public class Tax_Total
    {
        /// <summary>
        /// Currency Code
        /// </summary>
        public string currency_code { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public string value { get; set; }
    }

    /// <summary>
    /// Represents the Insurance
    /// </summary>
    public class Insurance
    {
        /// <summary>
        /// Currency Code
        /// </summary>
        public string currency_code { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public string value { get; set; }
    }

    /// <summary>
    /// Shipping Discount
    /// </summary>
    public class Shipping_Discount
    {
        /// <summary>
        /// Currency Code
        /// </summary>
        public string currency_code { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public string value { get; set; }
    }

    /// <summary>
    /// Represents the Payee
    /// </summary>
    public class Payee
    {
        /// <summary>
        /// Email Address
        /// </summary>
        public string email_address { get; set; }
        /// <summary>
        /// Merchant Id
        /// </summary>
        public string merchant_id { get; set; }
    }

    /// <summary>
    /// Represents the Shipping
    /// </summary>
    public class Shipping1
    {
        /// <summary>
        /// Name
        /// </summary>
        public Name1 name { get; set; }
        /// <summary>
        /// Address
        /// </summary>
        public Address1 address { get; set; }
    }

    /// <summary>
    /// Represents the Name
    /// </summary>
    public class Name1
    {
        /// <summary>
        /// Full Name
        /// </summary>
        public string full_name { get; set; }
    }

    /// <summary>
    /// Represents the Address
    /// </summary>
    public class Address1
    {
        /// <summary>
        /// Address Line 1
        /// </summary>
        public string address_line_1 { get; set; }
        /// <summary>
        /// Admin Area 2
        /// </summary>
        public string admin_area_2 { get; set; }
        /// <summary>
        /// Admin Area 1
        /// </summary>
        public string admin_area_1 { get; set; }
        /// <summary>
        /// Postal Code
        /// </summary>
        public string postal_code { get; set; }
        /// <summary>
        /// Country Code
        /// </summary>
        public string country_code { get; set; }
    }

    /// <summary>
    /// Represents the Payments
    /// </summary>
    public class Payments
    {
        /// <summary>
        /// Captures
        /// </summary>
        public Capture[] captures { get; set; }
    }

    /// <summary>
    /// Represents the Capture
    /// </summary>
    public class Capture
    {
        /// <summary>
        /// Id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// Amount
        /// </summary>
        public Amount1 amount { get; set; }
        /// <summary>
        /// Final Capture
        /// </summary>
        public bool final_capture { get; set; }
        /// <summary>
        /// Seller Protection
        /// </summary>
        public Seller_Protection seller_protection { get; set; }
        /// <summary>
        /// Create DateTime
        /// </summary>
        public DateTime create_time { get; set; }
        /// <summary>
        /// Update DateTime
        /// </summary>
        public DateTime update_time { get; set; }
    }

    /// <summary>
    /// Represents the Amount
    /// </summary>
    public class Amount1
    {
        /// <summary>
        /// Currency Code
        /// </summary>
        public string currency_code { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public string value { get; set; }
    }

    /// <summary>
    /// Represents the Seller Protection
    /// </summary>
    public class Seller_Protection
    {
        /// <summary>
        /// Status
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// Dispute Categories
        /// </summary>
        public string[] dispute_categories { get; set; }
    }

    /// <summary>
    /// Represents the Item
    /// </summary>
    public class Item
    {
        /// <summary>
        /// Name
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Unit Amount
        /// </summary>
        public Unit_Amount unit_amount { get; set; }
        /// <summary>
        /// Tax
        /// </summary>
        public Tax tax { get; set; }
        /// <summary>
        /// Quantity
        /// </summary>
        public string quantity { get; set; }
    }

    /// <summary>
    /// Represents the Unit Amount
    /// </summary>
    public class Unit_Amount
    {
        /// <summary>
        /// Currency Code
        /// </summary>
        public string currency_code { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public string value { get; set; }
    }

    /// <summary>
    /// Represents the Tax
    /// </summary>
    public class Tax
    {
        /// <summary>
        /// Currency Code
        /// </summary>
        public string currency_code { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public string value { get; set; }
    }

    /// <summary>
    /// Represents the Link
    /// </summary>
    public class Link
    {
        /// <summary>
        /// Href
        /// </summary>
        public string href { get; set; }
        /// <summary>
        /// Rel
        /// </summary>
        public string rel { get; set; }
        /// <summary>
        /// Method
        /// </summary>
        public string method { get; set; }
    }

}
