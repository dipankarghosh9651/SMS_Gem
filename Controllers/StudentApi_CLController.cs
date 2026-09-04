using SMS.DAL;
using SMS.Models;
using SMS_Gem.DAL;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Http;




namespace SMS.Controllers
{
    public class StudentApi_CLController : ApiController
    {
        private readonly StudentRepository_CL _repo = new StudentRepository_CL();

        private string GetCurrentBranch()
        {
            return HttpContext.Current.Session?["BranchCode"]?.ToString()
                   ?? ConfigurationManager.AppSettings["Default.Branch"]
                   ?? "CAP";
        }


        [HttpGet]
        public IHttpActionResult GetLookup(string id)
        {
            try
            {
                // 1. Safe Branch resolution with Web.config fallback
                string branch = null;
                if (HttpContext.Current.Session != null)
                {
                    branch = SessionHelper.CurrentBranch(HttpContext.Current);
                }

                if (string.IsNullOrWhiteSpace(branch))
                {
                    branch = System.Configuration.ConfigurationManager.AppSettings["Default.Branch"] ?? "CAP";
                }

                // 2. Safe RID lookup from Web.config
                string rid = ResolveRid(id);
                if (string.IsNullOrWhiteSpace(rid))
                {
                    return BadRequest($"No RID mapping found in Web.config for lookup: '{id}'");
                }

                // 3. Call StudentRepository (Place breakpoint on this line)
                DataTable dt = new StudentRepository_CL.StudentRepository_GEM().GetMasterList(branch, rid);

                return Ok(dt);
            }
            catch (Exception ex)
            {
                // Return clear error message to inspect in Network tab
                return InternalServerError(new Exception($"Failed in GetLookup({id}): {ex.Message} -> {ex.StackTrace}"));
            }
        }




        private string GetCurrentUser()
        {
            return HttpContext.Current.Session?["UserId"]?.ToString() ?? "SystemAdmin";
        }

        [HttpGet]
        public IHttpActionResult GetNextCode()
        {
            try
            {
                string code = _repo.GenerateNextStudentCode(GetCurrentBranch());
                return Ok(new ApiResponse { Success = true, Data = code });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse { Success = false, Message = ex.Message });
            }
        }

        [HttpGet]
        public IHttpActionResult GetLookupX(string id)
        {
            try
            {
                string rid = ResolveRid(id);
                var items = _repo.GetMasterLookups(GetCurrentBranch(), rid);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult SaveStudent([FromBody] StudentModel model)
        {
            if (model == null)
            {
                return Ok(new ApiResponse { Success = false, Message = "Invalid payload" });
            }

            try
            {
                model.Branch = GetCurrentBranch();
                var (success, msg) = _repo.SaveStudent(model, GetCurrentUser());
                return Ok(new ApiResponse { Success = success, Message = msg });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse { Success = false, Message = ex.Message });
            }
        }


        private string ResolveRid(string lookupType)
        {
            switch ((lookupType ?? string.Empty).Trim().ToUpperInvariant())
            {
                case "BLOODGROUP": return ConfigurationManager.AppSettings["RID.BloodGroup"];
                case "NATIONALITY": return ConfigurationManager.AppSettings["RID.Nationality"];
                case "MOTHERTONGUE": return ConfigurationManager.AppSettings["RID.MotherTongue"];
                case "ADMISSIONCATEGORY": return ConfigurationManager.AppSettings["RID.AdmissionCategory"];
                case "CASTE_CATEGORY": return ConfigurationManager.AppSettings["RID.Caste_Category"];
                case "RELIGION": return ConfigurationManager.AppSettings["RID.Religion"];
                case "GENDER": return ConfigurationManager.AppSettings["RID.Gender"];
                default: throw new ArgumentException($"Unknown lookup identifier: {lookupType}");
            }
        }

        ///////////////////////
        ///
        [HttpGet]
        [Route("api/StudentApi_CL/GetStudentList")]
        public IHttpActionResult GetStudentList(string searchTerm = "")
        {
            try
            {
                string branch = GetCurrentBranch();
                // Assuming a DAL method to fetch students by search filter
                DataTable dt = _repo.GetStudents(branch, searchTerm);
                return Ok(new ApiResponse { Success = true, Data = dt });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("api/StudentApi_CL/SaveGuardianMap")]
        public IHttpActionResult SaveGuardianMap([FromBody] StudentGuardianMapModel model)
        {
            if (model == null)
                return Ok(new ApiResponse { Success = false, Message = "Invalid payload" });

            try
            {
                model.Branch = GetCurrentBranch();
                string user = GetCurrentUser();

                var (code, msg) = DatabaseHelper.ExecuteNonQueryWithReturn("sp_SaveStudentGuardianMap",
                    new SqlParameter("@Branch", model.Branch),
                    new SqlParameter("@MapID", model.MapID),
                    new SqlParameter("@Student_Branch", model.Student_Branch ?? model.Branch),
                    new SqlParameter("@Student_Code", model.Student_Code),
                    new SqlParameter("@GuardianID", model.GuardianID),
                    new SqlParameter("@Guardian_Branch", model.Guardian_Branch ?? model.Branch),
                    new SqlParameter("@Guardian_Code", model.Guardian_Code),
                    new SqlParameter("@Rel_Code", model.Rel_Code),
                    new SqlParameter("@IsPrimaryContact", model.IsPrimaryContact),
                    new SqlParameter("@IsEmergencyContact", model.IsEmergencyContact),
                    new SqlParameter("@CanPickup", model.CanPickup),
                    new SqlParameter("@CanViewReportCard", model.CanViewReportCard),
                    new SqlParameter("@CanReceiveSMS", model.CanReceiveSMS),
                    new SqlParameter("@CanReceiveEmail", model.CanReceiveEmail),
                    new SqlParameter("@ContactPriority", model.ContactPriority),
                    new SqlParameter("@SpecificPhone", (object)model.SpecificPhone ?? DBNull.Value),
                    new SqlParameter("@Machine_Id", (object)model.Machine_Id ?? DBNull.Value),
                    new SqlParameter("@UserId", user),
                    new SqlParameter("@DMLStatus", string.IsNullOrEmpty(model.DMLStatus) ? "I" : model.DMLStatus)
                );

                return Ok(new ApiResponse { Success = (code == 1 || code == 0), Message = msg });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse { Success = false, Message = ex.Message });
            }
        }



        [HttpGet]
        [Route("api/StudentApi_CL/GetStudentByCode")]
        public IHttpActionResult GetStudentByCode(string code)
        {
            try
            {
                string branch = GetCurrentBranch();
                DataTable dt = DatabaseHelper.ExecuteQuery(
                    "usp_GetStudentByCode",
                    new SqlParameter("@Branch", branch),
                    new SqlParameter("@Student_Code", code)
                );
                return Ok(new ApiResponse { Success = true, Data = dt });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse { Success = false, Message = ex.Message });
            }
        }





    }
}