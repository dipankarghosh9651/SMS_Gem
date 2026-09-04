using System;

namespace SMS.Models
{
    public class StudentModel
    {
        public string Branch { get; set; }
        public string StudentCode { get; set; }
        public string AdmissionNo { get; set; }
        public string RollNumber { get; set; }
        public string AdmissionCategoryCode { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public string MachineId { get; set; }
        public bool IsActive { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string GenderCode { get; set; }
        public string BloodGroupCode { get; set; }

        public string NationalityCode { get; set; }
        public string MotherTongueCode { get; set; }
        public string ReligionCode { get; set; }
        public string Caste_Category { get; set; }
        public string AadhaarNumber { get; set; }
        public string PreviousSchool { get; set; }
        public string TcNumber { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PinCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string AlternatePhone { get; set; }
        public string Email { get; set; }

        public string RfidTag { get; set; }
        public bool PortalAccess { get; set; }
        public string PhotoUrl { get; set; }
        public string StudentPhotoBase64 { get; set; }
        public string Remarks { get; set; }
    }

    public class LookupItem
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}