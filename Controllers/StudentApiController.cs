using SMS.DAL;
using SMS.Models;
using SMS.Security;
using SMS_Gem.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Http;

namespace SMS_Gem.Controllers
{
    [JwtAuth]
    [RoutePrefix("api/students")]
    public class StudentApiController : ApiController
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["SMSDb"].ConnectionString;

        #region Lookup Endpoints

        /// <summary>
        /// GET api/students/lookups?branch=CAP&type=BloodGroup
        /// </summary>
        [HttpGet]
        [Route("lookups")]
        public IHttpActionResult GetLookup([FromUri] string branch, [FromUri] string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return BadRequest("Lookup 'type' query parameter is required.");

            branch = ResolveBranch(branch);
            string rid = ResolveRid(type);
            if (string.IsNullOrWhiteSpace(rid))
                return BadRequest($"Unknown lookup type identifier: '{type}'.");

            var list = new List<object>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.usp_MainMaster_GetList_CL", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Branch", SqlDbType.Char, 3) { Value = branch });
                cmd.Parameters.Add(new SqlParameter("@RID", SqlDbType.Char, 4) { Value = rid });

                conn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new
                        {
                            Code = dr[0] != DBNull.Value ? dr[0].ToString().Trim() : "",
                            Desc = dr[1] != DBNull.Value ? dr[1].ToString().Trim() : ""
                        });
                    }
                }
            }

            return Ok(new { Success = true, Data = list });
        }

        /// <summary>
        /// GET api/students/lookups/all?branch=CAP
        /// </summary>
        [HttpGet]
        [Route("lookups/all")]
        public IHttpActionResult GetAllLookups([FromUri] string branch)
        {
            branch = ResolveBranch(branch);

            var result = new Dictionary<string, List<object>>
            {
                { "BloodGroup", new List<object>() },
                { "Gender", new List<object>() },
                { "Nationality", new List<object>() },
                { "MotherTongue", new List<object>() },
                { "AdmissionCategory", new List<object>() },
                { "Religion", new List<object>() },
                { "Caste_Category", new List<object>() }
            };

            var map = new Dictionary<string, string>
            {
                { "BloodGroup", PadRid(ConfigurationManager.AppSettings["RID.BloodGroup"]) },
                { "Gender", PadRid(ConfigurationManager.AppSettings["RID.Gender"]) },
                { "Nationality", PadRid(ConfigurationManager.AppSettings["RID.Nationality"]) },
                { "MotherTongue", PadRid(ConfigurationManager.AppSettings["RID.MotherTongue"]) },
                { "AdmissionCategory", PadRid(ConfigurationManager.AppSettings["RID.AdmissionCategory"]) },
                { "Religion", PadRid(ConfigurationManager.AppSettings["RID.Religion"]) },
                { "Caste_Category", PadRid(ConfigurationManager.AppSettings["RID.Caste_Category"]) }
            };

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                foreach (var entry in map)
                {
                    if (string.IsNullOrWhiteSpace(entry.Value)) continue;

                    using (var cmd = new SqlCommand("dbo.usp_MainMaster_GetList_CL", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@Branch", SqlDbType.Char, 3) { Value = branch });
                        cmd.Parameters.Add(new SqlParameter("@RID", SqlDbType.Char, 4) { Value = entry.Value });

                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                result[entry.Key].Add(new
                                {
                                    Code = dr[0] != DBNull.Value ? dr[0].ToString().Trim() : "",
                                    Desc = dr[1] != DBNull.Value ? dr[1].ToString().Trim() : ""
                                });
                            }
                        }
                    }
                }
            }

            return Ok(new { Success = true, Data = result });
        }

        #endregion

        #region Sequence Endpoints

        /// <summary>
        /// GET api/students/next-code?branch=CAP
        /// </summary>
        [HttpGet]
        [Route("next-code")]
        public IHttpActionResult GetNextStudentCode([FromUri] string branch)
        {
            branch = ResolveBranch(branch);
            string prefix = ConfigurationManager.AppSettings["Student.CodePrefix"] ?? "S";
            int pad = int.TryParse(ConfigurationManager.AppSettings["Student.CodePad"], out int p) ? p : 4;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.usp_GetNextStudentSequence", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Branch", SqlDbType.VarChar, 50).Value = branch;
                cmd.Parameters.Add("@Prefix", SqlDbType.VarChar, 20).Value = prefix;

                conn.Open();
                object result = cmd.ExecuteScalar();
                int nextSeq = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 1;
                string generatedCode = $"{prefix}{nextSeq.ToString().PadLeft(pad, '0')}";

                return Ok(new { Success = true, Data = generatedCode });
            }
        }

        #endregion

        #region Student CRUD Endpoints

        /// <summary>
        /// GET api/students/directory?branch=CAP&search=John
        /// </summary>
        [HttpGet]
        [Route("directory")]
        public IHttpActionResult GetStudentList([FromUri] string branch, [FromUri] string search = "")
        {
            branch = ResolveBranch(branch);
            var list = new List<object>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.sp_GetStudentsList", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Branch", SqlDbType.Char, 3) { Value = branch });
                cmd.Parameters.Add(new SqlParameter("@SearchTerm", SqlDbType.NVarChar, 100)
                {
                    Value = string.IsNullOrWhiteSpace(search) ? (object)DBNull.Value : search.Trim()
                });

                conn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new
                        {
                            Student_Code = dr["Student_Code"] != DBNull.Value ? dr["Student_Code"].ToString().Trim() : "",
                            AdmissionNo = dr["AdmissionNo"] != DBNull.Value ? dr["AdmissionNo"].ToString().Trim() : "",
                            RollNumber = dr["RollNumber"] != DBNull.Value ? dr["RollNumber"].ToString().Trim() : "",
                            FirstName = dr["FirstName"] != DBNull.Value ? dr["FirstName"].ToString().Trim() : "",
                            MiddleName = dr["MiddleName"] != DBNull.Value ? dr["MiddleName"].ToString().Trim() : "",
                            LastName = dr["LastName"] != DBNull.Value ? dr["LastName"].ToString().Trim() : "",
                            Gender = dr["Gender"] != DBNull.Value ? dr["Gender"].ToString().Trim() : "",
                            Phone = dr["Phone"] != DBNull.Value ? dr["Phone"].ToString().Trim() : "",
                            IsActive = dr["IsActive"] != DBNull.Value && Convert.ToBoolean(dr["IsActive"])
                        });
                    }
                }
            }

            return Ok(new { Success = true, Data = list });
        }

        /// <summary>
        /// GET api/students/{studentCode}?branch=CAP
        /// </summary>
        [HttpGet]
        [Route("{studentCode}")]
        public IHttpActionResult GetStudentByCode([FromUri] string branch, string studentCode)
        {
            branch = ResolveBranch(branch);
            var result = new Dictionary<string, object>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.sp_GetStudentByCode", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Branch", SqlDbType.Char, 3) { Value = branch });
                cmd.Parameters.Add(new SqlParameter("@Student_Code", SqlDbType.NVarChar, 5) { Value = studentCode });

                conn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            string columnName = dr.GetName(i);
                            object value = dr.IsDBNull(i) ? null : dr.GetValue(i);

                            if (columnName.Equals("StudentPhoto", StringComparison.OrdinalIgnoreCase) && value != null)
                            {
                                byte[] photoBytes = value as byte[];
                                result[columnName] = photoBytes != null
                                    ? Convert.ToBase64String(photoBytes)
                                    : value.ToString();
                            }
                            else
                            {
                                result[columnName] = value;
                            }
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return Ok(new { Success = true, Data = result });
        }

        /// <summary>
        /// POST api/students/save
        /// </summary>
        [HttpPost]
        [Route("save")]
        public IHttpActionResult SaveStudent([FromBody] StudentInputModel model)
        {
            if (model == null)
                return BadRequest("Invalid JSON payload.");

            string branch = ResolveBranch(model.Branch);

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.usp_Student_Insert", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Branch", branch);
                cmd.Parameters.AddWithValue("@Student_Code", (object)model.Student_Code ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AdmissionNo", (object)model.AdmissionNo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RollNumber", (object)model.RollNumber ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AdmissionCategory", (object)model.AdmissionCategory ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EnrollmentDate", string.IsNullOrWhiteSpace(model.EnrollmentDate) ? (object)DBNull.Value : DateTime.Parse(model.EnrollmentDate));
                cmd.Parameters.AddWithValue("@Machine_Id", (object)model.Machine_Id ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", model.IsActive);

                cmd.Parameters.AddWithValue("@FirstName", (object)model.FirstName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MiddleName", (object)model.MiddleName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@LastName", (object)model.LastName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FatherName", (object)model.F_Name ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MotherName", (object)model.M_Name ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DateOfBirth", string.IsNullOrWhiteSpace(model.DateOfBirth) ? (object)DBNull.Value : DateTime.Parse(model.DateOfBirth));
                cmd.Parameters.AddWithValue("@Gender", (object)model.Gender ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@BloodGroup", (object)model.BloodGroup_Code ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@Nationality", (object)model.Nationality ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MotherTongue", (object)model.MotherTongue ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Religion", (object)model.Religion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Caste_Category", (object)model.Caste_Category ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AadhaarNumber", (object)model.AadhaarNumber ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PreviousSchool", (object)model.PreviousSchool ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TC_Number", (object)model.TC_Number ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@AddressLine1", (object)model.AddressLine1 ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AddressLine2", (object)model.AddressLine2 ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@City", (object)model.City ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@State", (object)model.State ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PinCode", (object)model.PinCode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Country", string.IsNullOrWhiteSpace(model.Country) ? "India" : model.Country);

                cmd.Parameters.AddWithValue("@Phone", (object)model.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AlternatePhone", (object)model.AlternatePhone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object)model.Email ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@RFID_Tag", (object)model.RFID_Tag ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PortalAccess", model.PortalAccess);
                cmd.Parameters.AddWithValue("@PhotoUrl", (object)model.PhotoUrl ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@StudentPhoto", DBNull.Value);
                cmd.Parameters.AddWithValue("@Remarks", (object)model.Remarks ?? DBNull.Value);

                cmd.Parameters.Add(new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.InputOutput, Value = 0 });
                cmd.Parameters.Add(new SqlParameter("@ReturnMessage", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.InputOutput, Value = string.Empty });

                conn.Open();
                cmd.ExecuteNonQuery();

                int returnCode = Convert.ToInt32(cmd.Parameters["@ReturnCode"].Value ?? 0);
                string returnMsg = cmd.Parameters["@ReturnMessage"].Value?.ToString() ?? "";

                return Ok(new { Success = (returnCode == 0), Message = returnMsg });
            }
        }

        #endregion

        #region Guardian Mapping Endpoints

        /// <summary>
        /// GET api/students/guardians?branch=CAP&studentCode=S0001
        /// </summary>
        [HttpGet]
        [Route("guardians")]
        public IHttpActionResult GetGuardianMappings([FromUri] string branch, [FromUri] string studentCode)
        {
            branch = ResolveBranch(branch);
            var list = new List<object>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.sp_GetStudentGuardianMappings", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Branch", SqlDbType.Char, 3) { Value = branch });
                cmd.Parameters.Add(new SqlParameter("@Student_Code", SqlDbType.NVarChar, 5) { Value = studentCode });

                conn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            row[dr.GetName(i)] = dr.IsDBNull(i) ? null : dr.GetValue(i);
                        }
                        list.Add(row);
                    }
                }
            }

            return Ok(new { Success = true, Data = list });
        }

        /// <summary>
        /// POST api/students/guardians/save
        /// </summary>
        [HttpPost]
        [Route("guardians/save")]
        public IHttpActionResult SaveGuardianMap([FromBody] StudentGuardianMapModel model)
        {
            if (model == null)
                return BadRequest("Invalid mapping payload.");

            string branch = ResolveBranch(model.Branch);
            string userId = User?.Identity?.Name ?? "SYSTEM";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.sp_SaveStudentGuardianMap", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Branch", branch);
                cmd.Parameters.AddWithValue("@MapID", model.MapID);
                cmd.Parameters.AddWithValue("@Student_Branch", string.IsNullOrWhiteSpace(model.Student_Branch) ? branch : model.Student_Branch);
                cmd.Parameters.AddWithValue("@Student_Code", (object)model.Student_Code ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@GuardianID", model.GuardianID);
                cmd.Parameters.AddWithValue("@Guardian_Code", (object)model.Guardian_Code ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Rel_Code", (object)model.Rel_Code ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@IsPrimaryContact", model.IsPrimaryContact);
                cmd.Parameters.AddWithValue("@IsEmergencyContact", model.IsEmergencyContact);
                cmd.Parameters.AddWithValue("@CanPickup", model.CanPickup);
                cmd.Parameters.AddWithValue("@CanViewReportCard", model.CanViewReportCard);
                cmd.Parameters.AddWithValue("@CanReceiveSMS", model.CanReceiveSMS);
                cmd.Parameters.AddWithValue("@CanReceiveEmail", model.CanReceiveEmail);
                cmd.Parameters.AddWithValue("@ContactPriority", model.ContactPriority <= 0 ? 1 : model.ContactPriority);
                cmd.Parameters.AddWithValue("@SpecificPhone", (object)model.SpecificPhone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Machine_Id", (object)model.Machine_Id ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@DMLStatus", string.IsNullOrWhiteSpace(model.DMLStatus) ? "I" : model.DMLStatus);

                cmd.Parameters.Add(new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.InputOutput, Value = 0 });
                cmd.Parameters.Add(new SqlParameter("@ReturnMessage", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.InputOutput, Value = string.Empty });

                conn.Open();
                cmd.ExecuteNonQuery();

                int returnCode = Convert.ToInt32(cmd.Parameters["@ReturnCode"].Value ?? 0);
                string returnMsg = cmd.Parameters["@ReturnMessage"].Value?.ToString() ?? "";

                return Ok(new { Success = (returnCode == 1 || returnCode == 0), Message = returnMsg });
            }
        }

        #endregion

        #region Helpers

        private string ResolveBranch(string branch)
        {
            if (string.IsNullOrWhiteSpace(branch))
            {
                branch = HttpContext.Current?.Items["BranchCode"]?.ToString()
                         ?? ConfigurationManager.AppSettings["Default.Branch"]
                         ?? "CAP";
            }
            branch = branch.Trim();
            return branch.Length > 3 ? branch.Substring(0, 3) : branch.PadRight(3);
        }

        private string ResolveRid(string lookupType)
        {
            switch ((lookupType ?? "").Trim().ToUpperInvariant())
            {
                case "DEPARTMENT": return ConfigurationManager.AppSettings["RID.Department"] ?? "FN";
                case "BLOODGROUP": return PadRid(ConfigurationManager.AppSettings["RID.BloodGroup"]);
                case "NATIONALITY": return PadRid(ConfigurationManager.AppSettings["RID.Nationality"]);
                case "MOTHERTONGUE": return PadRid(ConfigurationManager.AppSettings["RID.MotherTongue"]);
                case "ADMISSIONCATEGORY": return PadRid(ConfigurationManager.AppSettings["RID.AdmissionCategory"]);
                case "CASTE_CATEGORY": return PadRid(ConfigurationManager.AppSettings["RID.Caste_Category"]);
                case "RELIGION": return PadRid(ConfigurationManager.AppSettings["RID.Religion"]);
                case "GENDER": return PadRid(ConfigurationManager.AppSettings["RID.Gender"]);
                case "RELATIONSHIP": return PadRid(ConfigurationManager.AppSettings["RID.Relationship"] ?? "RL");
                default: return null;
            }
        }

        private static string PadRid(string rid)
        {
            if (string.IsNullOrWhiteSpace(rid)) return string.Empty;
            return rid.Trim().Length > 4 ? rid.Trim().Substring(0, 4) : rid.Trim().PadRight(4);
        }

        #endregion
    }

    public class StudentInputModel
    {
        public string Branch { get; set; }
        public string Student_Code { get; set; }
        public string AdmissionNo { get; set; }
        public string RollNumber { get; set; }
        public string AdmissionCategory { get; set; }
        public string EnrollmentDate { get; set; }
        public string Machine_Id { get; set; }
        public bool IsActive { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string F_Name { get; set; }
        public string M_Name { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string BloodGroup_Code { get; set; }
        public string Nationality { get; set; }
        public string MotherTongue { get; set; }
        public string Religion { get; set; }
        public string Caste_Category { get; set; }
        public string AadhaarNumber { get; set; }
        public string PreviousSchool { get; set; }
        public string TC_Number { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PinCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string AlternatePhone { get; set; }
        public string Email { get; set; }
        public string RFID_Tag { get; set; }
        public bool PortalAccess { get; set; }
        public string PhotoUrl { get; set; }
        public string Remarks { get; set; }
    }

}
