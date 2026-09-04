using System;
using System.Configuration;
using System.Web;
using System.Web.Http;
using SMS.DAL;
using SMS.Models;

namespace SMS.Controllers
{
    [RoutePrefix("api/TeacherApi")]
    public class TeacherApiController : ApiController
    {
        private readonly TeacherRepository _repo = new TeacherRepository();

        private string GetCurrentBranch()
        {
            return HttpContext.Current.Session?["BranchCode"]?.ToString()
                   ?? ConfigurationManager.AppSettings["Default.Branch"]
                   ?? "CAP";
        }

        private string GetCurrentUser()
        {
            return HttpContext.Current.Session?["UserId"]?.ToString() ?? "ADMIN";
        }

        [HttpGet]
        [Route("GetLookup")]
        public IHttpActionResult GetLookup([FromUri] string id)
        {
            try
            {
                string branch = GetCurrentBranch();
                string rid = ResolveRid(id);
                var items = _repo.GetMasterLookups(branch, rid);
                return Ok(new ApiResponse { Success = true, Data = items });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Failed in GetLookup({id}): {ex.Message}"));
            }
        }

        [HttpGet]
        [Route("GetNextCode")]
        public IHttpActionResult GetNextCode()
        {
            try
            {
                string code = _repo.GenerateNextTeacherCode(GetCurrentBranch());
                return Ok(new ApiResponse { Success = true, Data = code });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse { Success = false, Message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetTeacherByCode")]
        public IHttpActionResult GetTeacherByCode([FromUri] string code)
        {
            try
            {
                var teacher = _repo.GetTeacherByCode(GetCurrentBranch(), code);
                if (teacher == null)
                    return Ok(new ApiResponse { Success = false, Message = "Teacher not found." });

                return Ok(new ApiResponse { Success = true, Data = teacher });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("SaveTeacher")]
        public IHttpActionResult SaveTeacher([FromBody] TeacherModel model)
        {
            if (model == null)
                return Ok(new ApiResponse { Success = false, Message = "Payload empty." });

            model.Branch = GetCurrentBranch();
            var (success, msg) = _repo.SaveTeacher(model, GetCurrentUser());
            return Ok(new ApiResponse { Success = success, Message = msg });
        }

        private string ResolveRid(string lookupType)
        {
            switch ((lookupType ?? string.Empty).Trim().ToUpperInvariant())
            {
                case "DEPARTMENT": return ConfigurationManager.AppSettings["RID.Department"] ?? "FN";
                case "GENDER": return ConfigurationManager.AppSettings["RID.Gender"] ?? "MF";
                default: throw new ArgumentException($"No RID mapping configured for lookup: '{lookupType}'");
            }
        }
    }
}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace SMS_Gem.Controllers
//{
//    public class TeacherApiController
//    {
//    }
//}