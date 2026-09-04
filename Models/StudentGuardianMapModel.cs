using System;

namespace SMS.Models
{
    public class StudentGuardianMapModel
    {
        public string Branch { get; set; }
        public int MapID { get; set; }
        public string Student_Branch { get; set; }
        public string Student_Code { get; set; }
        public int GuardianID { get; set; }
        public string Guardian_Branch { get; set; }
        public string Guardian_Code { get; set; }
        public string Rel_Code { get; set; }
        public bool IsPrimaryContact { get; set; }
        public bool IsEmergencyContact { get; set; }
        public bool CanPickup { get; set; }
        public bool CanViewReportCard { get; set; }
        public bool CanReceiveSMS { get; set; }
        public bool CanReceiveEmail { get; set; }
        public int ContactPriority { get; set; }
        public string SpecificPhone { get; set; }
        public string Machine_Id { get; set; }
        public string DMLStatus { get; set; } // 'I', 'U', 'D'
    }
}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace SMS_Gem.Models
//{
//    public class StudentGuardianMapModel
//    {
//    }
//}