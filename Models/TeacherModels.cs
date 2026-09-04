using System;

namespace SMS.Models
{
    public class TeacherModel
    {
        public string Branch { get; set; }
        public string Teacher_Code { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Dept_Branch { get; set; }
        public string Dept_RID { get; set; }
        public string Dept_Code { get; set; }
        public string Gender { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PinCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string AlternatePhone { get; set; }
        public string Email { get; set; }
        public DateTime HireDate { get; set; }
        public string Qualification { get; set; }
        public string Specialization { get; set; }
        public int? Experience_Years { get; set; }
        public string PhotoUrl { get; set; }
        public string AadhaarNumber { get; set; }
        public string PAN_Number { get; set; }
        public string BankAccountNo { get; set; }
        public string IFSC_Code { get; set; }
        public string PF_Number { get; set; }
        public bool IsActive { get; set; }
        public string TeacherPhotoBase64 { get; set; }
        public string Machine_Id { get; set; }
    }

    //public class LookupItem
    //{
    //    public string Code { get; set; }
    //    public string Description { get; set; }
    //}

    //public class ApiResponse
    //{
    //    public bool Success { get; set; }
    //    public string Message { get; set; }
    //    public object Data { get; set; }
    //}
}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace SMS_Gem.Models
//{
//    public class TeacherModels
//    {
//    }
//}