using System;

namespace SMS.Models
{
    public class GuardianModel
    {
        public int GuardianID { get; set; }
        public string Branch { get; set; }
        public string Guardian_Code { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string AlternatePhone { get; set; }
        public string WhatsAppNumber { get; set; }
        public string Email { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PinCode { get; set; }
        public string Occupation { get; set; }
        public string Organization { get; set; }
        public decimal? AnnualIncome { get; set; }
        public bool IsActive { get; set; }
        public string PhotoUrl { get; set; }
        public string Machine_Id { get; set; }
    }

}