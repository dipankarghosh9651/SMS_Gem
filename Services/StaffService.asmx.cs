using SMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace SMS_Gem.Services
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.Web.Script.Services.ScriptService]
    public class StaffService : WebService
    {
        // ==================== GUARDIAN ENDPOINTS ====================

        [WebMethod(EnableSession = true)]
        public string GetNextGuardianCode()
        {
            // Implementation calls DAL to get next code (e.g., G0001)
            return "G0001";
        }

        [WebMethod(EnableSession = true)]
        public List<GuardianModel> GetGuardianList()
        {
            // Call DAL: SELECT * FROM Guardians WHERE Branch = @Branch
            return new List<GuardianModel>();
        }

        [WebMethod(EnableSession = true)]
        public GuardianModel GetGuardianByCode(string guardianCode)
        {
            // Call DAL: SELECT * FROM Guardians WHERE Branch = @Branch AND Guardian_Code = @Code
            return new GuardianModel();
        }

        [WebMethod(EnableSession = true)]
        public ApiResponse SaveGuardian(GuardianModel model)
        {
            // Call DAL: Insert / Update logic
            return new ApiResponse { Success = true, Message = "Guardian record saved successfully." };
        }

        // ==================== TEACHER ENDPOINTS ====================

        [WebMethod(EnableSession = true)]
        public string GetNextTeacherCode()
        {
            // Implementation calls DAL to get next code (e.g., T0001)
            return "T0001";
        }

        [WebMethod(EnableSession = true)]
        public List<TeacherModel> GetTeacherList()
        {
            // Call DAL: SELECT * FROM Teachers WHERE Branch = @Branch
            return new List<TeacherModel>();
        }

        [WebMethod(EnableSession = true)]
        public TeacherModel GetTeacherByCode(string teacherCode)
        {
            // Call DAL: SELECT * FROM Teachers WHERE Branch = @Branch AND Teacher_Code = @Code
            return new TeacherModel();
        }

        [WebMethod(EnableSession = true)]
        public ApiResponse SaveTeacher(TeacherModel model)
        {
            // Call DAL: Insert / Update logic with Base64 to byte[] conversion for TeacherPhoto
            return new ApiResponse { Success = true, Message = "Teacher record saved successfully." };
        }

        [WebMethod(EnableSession = true)]
        public List<LookupItem> GetLookup(string lookupType)
        {
            // Generic lookup caller matching StudentService logic
            return new List<LookupItem>();
        }


        private string ResolveRid(string lookupType)
        {
            switch ((lookupType ?? string.Empty).Trim().ToUpperInvariant())
            {
                case "GENDER":
                    return ConfigurationManager.AppSettings["RID.Gender"] ?? "MF";
                case "DEPARTMENT":
                    return ConfigurationManager.AppSettings["RID.Department"] ?? "FN";
                case "RELATIONSHIP":
                    return ConfigurationManager.AppSettings["RID.Relationship"] ?? "RL";
                case "BLOODGROUP":
                    return ConfigurationManager.AppSettings["RID.BloodGroup"] ?? "BG";
                case "NATIONALITY":
                    return ConfigurationManager.AppSettings["RID.Nationality"] ?? "NT";
                case "MOTHERTONGUE":
                    return ConfigurationManager.AppSettings["RID.MotherTongue"] ?? "MT";
                case "ADMISSIONCATEGORY":
                    return ConfigurationManager.AppSettings["RID.AdmissionCategory"] ?? "AC";
                case "CASTE_CATEGORY":
                    return ConfigurationManager.AppSettings["RID.Caste_Category"] ?? "CC";
                case "RELIGION":
                    return ConfigurationManager.AppSettings["RID.Religion"] ?? "RL";
                default:
                    throw new ArgumentException($"No RID mapping configured for lookup type: '{lookupType}'");
            }
        }

        //[WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public List<LookupItem> GetLookup(string lookupType)
        //{
        //    string branch = ConfigurationManager.AppSettings["RID.Branch"] ?? "CAP";     //GetCurrentBranch();
        //    string rid = ResolveRid(lookupType);

        //    return _repo.GetMasterLookups(branch, rid);
        //}

    }

    }



//namespace SMS_Gem.Services
//{
//    /// <summary>
//    /// Summary description for StaffService
//    /// </summary>
//    [WebService(Namespace = "http://tempuri.org/")]
//    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
//    [System.ComponentModel.ToolboxItem(false)]
//    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
//    // [System.Web.Script.Services.ScriptService]
//    public class StaffService : System.Web.Services.WebService
//    {

//        [WebMethod]
//        public string HelloWorld()
//        {
//            return "Hello World";
//        }
//    }
//}
