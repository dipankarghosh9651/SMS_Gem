using SMS_Gem.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Services;
using System.Web.Services;

namespace SMS_Gem.Services
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class StudentService : WebService
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["SMSDb"].ConnectionString;

        #region Transfer Models

        public class LookupItem
        {
            public string Code { get; set; }
            public string Desc { get; set; }
        }

        public class ServiceResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }

        public class StudentSummaryItem
        {
            public string Student_Code { get; set; }
            public string AdmissionNo { get; set; }
            public string RollNumber { get; set; }
            public string FullName { get; set; }
            public string Gender { get; set; }
            public string Phone { get; set; }
            public bool IsActive { get; set; }
        }

        public class AllLookupsResponse
        {
            public List<LookupItem> BloodGroup { get; set; } = new List<LookupItem>();
            public List<LookupItem> Gender { get; set; } = new List<LookupItem>();
            public List<LookupItem> Nationality { get; set; } = new List<LookupItem>();
            public List<LookupItem> MotherTongue { get; set; } = new List<LookupItem>();
            public List<LookupItem> AdmissionCategory { get; set; } = new List<LookupItem>();
            public List<LookupItem> Religion { get; set; } = new List<LookupItem>();
            public List<LookupItem> Caste_Category { get; set; } = new List<LookupItem>();
        }

        #endregion

        #region Lookup Methods

        /// <summary>
        /// Fetches lookup items for a specific lookup type and branch.
        /// </summary>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<LookupItem> GetLookup(string branch, string lookupType)
        {
            var list = new List<LookupItem>();
            branch = ResolveBranch(branch);

            string rid = ResolveRid(lookupType);
            if (string.IsNullOrWhiteSpace(rid)) return list;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.usp_MainMaster_GetList_CL", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Branch", SqlDbType.Char, 3) { Value = branch });
                cmd.Parameters.Add(new SqlParameter("@RID", SqlDbType.Char, 4) { Value = rid });

                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        string code = dr[0] != DBNull.Value ? dr[0].ToString().Trim() : "";
                        string desc = dr[1] != DBNull.Value ? dr[1].ToString().Trim() : code;

                        list.Add(new LookupItem { Code = code, Desc = desc });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Fetches all system lookups in a single batch request for a branch.
        /// </summary>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public AllLookupsResponse GetAllLookups(string branch)
        {
            var result = new AllLookupsResponse();
            branch = ResolveBranch(branch);

            var map = new Dictionary<string, (string RID, List<LookupItem> Target)>
            {
                { "BloodGroup",        (PadRid(ConfigurationManager.AppSettings["RID.BloodGroup"]), result.BloodGroup) },
                { "Gender",            (PadRid(ConfigurationManager.AppSettings["RID.Gender"]), result.Gender) },
                { "Nationality",       (PadRid(ConfigurationManager.AppSettings["RID.Nationality"]), result.Nationality) },
                { "MotherTongue",      (PadRid(ConfigurationManager.AppSettings["RID.MotherTongue"]), result.MotherTongue) },
                { "AdmissionCategory", (PadRid(ConfigurationManager.AppSettings["RID.AdmissionCategory"]), result.AdmissionCategory) },
                { "Religion",          (PadRid(ConfigurationManager.AppSettings["RID.Religion"]), result.Religion) },
                { "Caste_Category",    (PadRid(ConfigurationManager.AppSettings["RID.Caste_Category"]), result.Caste_Category) }
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                foreach (var entry in map.Values)
                {
                    if (string.IsNullOrWhiteSpace(entry.RID)) continue;

                    using (SqlCommand cmd = new SqlCommand("dbo.usp_MainMaster_GetList_CL", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@Branch", SqlDbType.Char, 3) { Value = branch });
                        cmd.Parameters.Add(new SqlParameter("@RID", SqlDbType.Char, 4) { Value = entry.RID });

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                entry.Target.Add(new LookupItem
                                {
                                    Code = dr[0] != DBNull.Value ? dr[0].ToString().Trim() : "",
                                    Desc = dr[1] != DBNull.Value ? dr[1].ToString().Trim() : ""
                                });
                            }
                        }
                    }
                }
            }
            return result;
        }

        #endregion

        #region Sequence Generation

        /// <summary>
        /// Generates the next sequential Student Code for a branch.
        /// </summary>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetNextStudentCode(string branch)
        {
            branch = ResolveBranch(branch);
            string prefix = ConfigurationManager.AppSettings["Student.CodePrefix"] ?? "S";
            int pad = 4;
            if (int.TryParse(ConfigurationManager.AppSettings["Student.CodePad"], out int p))
            {
                pad = p;
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.usp_GetNextStudentSequence", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Branch", SqlDbType.VarChar, 50).Value = branch;
                cmd.Parameters.Add("@Prefix", SqlDbType.VarChar, 20).Value = prefix;

                conn.Open();
                object result = cmd.ExecuteScalar();
                int nextSeq = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 1;
                return $"{prefix}{nextSeq.ToString().PadLeft(pad, '0')}";
            }
        }

        #endregion

        #region Student CRUD & Directory

        /// <summary>
        /// Retrieves a directory of students with search filtering.
        /// </summary>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<StudentSummaryItem> GetStudentList(string branch, string searchTerm)
        {
            var list = new List<StudentSummaryItem>();
            branch = ResolveBranch(branch);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.usp_GetStudentsList", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Branch", SqlDbType.Char, 3) { Value = branch });
                cmd.Parameters.Add(new SqlParameter("@SearchTerm", SqlDbType.NVarChar, 100)
                {
                    Value = string.IsNullOrWhiteSpace(searchTerm) ? (object)DBNull.Value : searchTerm.Trim()
                });

                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        string fName = dr["FirstName"] != DBNull.Value ? dr["FirstName"].ToString().Trim() : "";
                        string mName = dr["MiddleName"] != DBNull.Value ? dr["MiddleName"].ToString().Trim() : "";
                        string lName = dr["LastName"] != DBNull.Value ? dr["LastName"].ToString().Trim() : "";
                        string fullName = string.Join(" ", new[] { fName, mName, lName }).Replace("  ", " ").Trim();

                        list.Add(new StudentSummaryItem
                        {
                            Student_Code = dr["Student_Code"] != DBNull.Value ? dr["Student_Code"].ToString().Trim() : "",
                            AdmissionNo = dr["AdmissionNo"] != DBNull.Value ? dr["AdmissionNo"].ToString().Trim() : "",
                            RollNumber = dr["RollNumber"] != DBNull.Value ? dr["RollNumber"].ToString().Trim() : "",
                            FullName = fullName,
                            Gender = dr["Gender"] != DBNull.Value ? dr["Gender"].ToString().Trim() : "",
                            Phone = dr["Phone"] != DBNull.Value ? dr["Phone"].ToString().Trim() : "",
                            IsActive = dr["IsActive"] != DBNull.Value && Convert.ToBoolean(dr["IsActive"])
                        });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Accepts individual form parameters to allow direct browser test calls.
        /// </summary>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public ServiceResponse SaveStudentDirect(
            string branch,
            string studentCode,
            string admissionNo,
            string rollNumber,
            string admissionCategory,
            string enrollmentDate,
            string machineId,
            bool isActive,
            string firstName,
            string middleName,
            string lastName,
            string fatherName,
            string motherName,
            string dateOfBirth,
            string gender,
            string bloodGroupCode,
            string nationality,
            string motherTongue,
            string religion,
            string casteCategory,
            string AadhaarNumber,
            string previousSchool,
            string tcNumber,
            string addressLine1,
            string addressLine2,
            string city,
            string state,
            string pinCode,
            string country,
            string phone,
            string alternatePhone,
            string email,
            string rfidTag,
            bool portalAccess,
            string photoUrl,
            string remarks)
        {
            try
            {
                branch = ResolveBranch(branch);

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("dbo.usp_Student_Insert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Branch", branch);
                    cmd.Parameters.AddWithValue("@Student_Code", (object)studentCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AdmissionNo", (object)admissionNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@RollNumber", (object)rollNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AdmissionCategory", (object)admissionCategory ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EnrollmentDate", string.IsNullOrWhiteSpace(enrollmentDate) ? (object)DBNull.Value : DateTime.Parse(enrollmentDate));
                    cmd.Parameters.AddWithValue("@Machine_Id", (object)machineId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);

                    cmd.Parameters.AddWithValue("@FirstName", (object)firstName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MiddleName", (object)middleName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastName", (object)lastName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FatherName", (object)fatherName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MotherName", (object)motherName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DateOfBirth", string.IsNullOrWhiteSpace(dateOfBirth) ? (object)DBNull.Value : DateTime.Parse(dateOfBirth));
                    cmd.Parameters.AddWithValue("@Gender", (object)gender ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BloodGroup", (object)bloodGroupCode ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@Nationality", (object)nationality ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MotherTongue", (object)motherTongue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Religion", (object)religion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Caste_Category", (object)casteCategory ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AadhaarNumber", (object)AadhaarNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PreviousSchool", (object)previousSchool ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TC_Number", (object)tcNumber ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@AddressLine1", (object)addressLine1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AddressLine2", (object)addressLine2 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", (object)city ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@State", (object)state ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PinCode", (object)pinCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Country", string.IsNullOrWhiteSpace(country) ? "India" : country);

                    cmd.Parameters.AddWithValue("@Phone", (object)phone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AlternatePhone", (object)alternatePhone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)email ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@RFID_Tag", (object)rfidTag ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PortalAccess", portalAccess);
                    cmd.Parameters.AddWithValue("@PhotoUrl", (object)photoUrl ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@StudentPhoto", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Remarks", (object)remarks ?? DBNull.Value);

                    cmd.Parameters.Add(new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.InputOutput, Value = 0 });
                    cmd.Parameters.Add(new SqlParameter("@ReturnMessage", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.InputOutput, Value = string.Empty });

                    conn.Open();
                    BindDeclaredParameters(cmd);
                    cmd.ExecuteNonQuery();

                    SqlParameter pOutCode = FindParameter(cmd, "@ReturnCode");
                    SqlParameter pOutMsg = FindParameter(cmd, "@ReturnMessage");
                    int returnCode = pOutCode != null && pOutCode.Value != DBNull.Value ? Convert.ToInt32(pOutCode.Value) : 0;
                    string returnMsg = pOutMsg != null && pOutMsg.Value != DBNull.Value ? pOutMsg.Value.ToString() : "";

                    return new ServiceResponse { Success = (returnCode == 0), Message = returnMsg };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse { Success = false, Message = ex.Message };
            }
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public StudentInputModel GetStudentByCode(string branch, string studentCode)
        {
            branch = ResolveBranch(branch);
            if (string.IsNullOrWhiteSpace(studentCode)) return null;

            StudentInputModel model = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.usp_GetStudentByCode", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Branch", SqlDbType.Char, 3) { Value = branch });
                cmd.Parameters.Add(new SqlParameter("@Student_Code", SqlDbType.NVarChar, 5) { Value = studentCode.Trim() });

                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        model = new StudentInputModel
                        {





                            Student_Code = dr["Student_Code"] != DBNull.Value ? dr["Student_Code"].ToString().Trim() : "",
                            AdmissionNo = dr["AdmissionNo"] != DBNull.Value ? dr["AdmissionNo"].ToString().Trim() : "",
                            RollNumber = dr["RollNumber"] != DBNull.Value ? dr["RollNumber"].ToString().Trim() : "",
                            AdmissionCategory = dr["AdmissionCategory"] != DBNull.Value ? dr["AdmissionCategory"].ToString().Trim() : "",
                            EnrollmentDate = dr["EnrollmentDate"] != DBNull.Value ? Convert.ToDateTime(dr["EnrollmentDate"]).ToString("yyyy-MM-dd") : "",
                            Machine_Id = dr["Machine_Id"] != DBNull.Value ? dr["Machine_Id"].ToString().Trim() : "",
                            IsActive = dr["IsActive"] != DBNull.Value && Convert.ToBoolean(dr["IsActive"]),
                            FirstName = dr["FirstName"] != DBNull.Value ? dr["FirstName"].ToString().Trim() : "",
                            MiddleName = dr["MiddleName"] != DBNull.Value ? dr["MiddleName"].ToString().Trim() : "",
                            LastName = dr["LastName"] != DBNull.Value ? dr["LastName"].ToString().Trim() : "",
                            F_Name = dr["F_Name"] != DBNull.Value ? dr["F_Name"].ToString().Trim() : "",
                            M_Name = dr["M_Name"] != DBNull.Value ? dr["M_Name"].ToString().Trim() : "",
                            DateOfBirth = dr["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(dr["DateOfBirth"]).ToString("yyyy-MM-dd") : "",
                            Gender = dr["Gender"] != DBNull.Value ? dr["Gender"].ToString().Trim() : "",
                            BloodGroup_Code = dr["BloodGroup_Code"] != DBNull.Value ? dr["BloodGroup_Code"].ToString().Trim() : "",
                            Nationality = dr["Nationality"] != DBNull.Value ? dr["Nationality"].ToString().Trim() : "",
                            MotherTongue = dr["MotherTongue"] != DBNull.Value ? dr["MotherTongue"].ToString().Trim() : "",
                            Religion = dr["Religion"] != DBNull.Value ? dr["Religion"].ToString().Trim() : "",
                            Caste_Category = dr["Caste_Category"] != DBNull.Value ? dr["Caste_Category"].ToString().Trim() : "",
                            AadhaarNumber = dr["AadhaarNumber"] != DBNull.Value ? dr["AadhaarNumber"].ToString().Trim() : "",
                            PreviousSchool = dr["PreviousSchool"] != DBNull.Value ? dr["PreviousSchool"].ToString().Trim() : "",
                            TC_Number = dr["TC_Number"] != DBNull.Value ? dr["TC_Number"].ToString().Trim() : "",
                            AddressLine1 = dr["AddressLine1"] != DBNull.Value ? dr["AddressLine1"].ToString().Trim() : "",
                            AddressLine2 = dr["AddressLine2"] != DBNull.Value ? dr["AddressLine2"].ToString().Trim() : "",
                            City = dr["City"] != DBNull.Value ? dr["City"].ToString().Trim() : "",
                            State = dr["State"] != DBNull.Value ? dr["State"].ToString().Trim() : "",
                            PinCode = dr["PinCode"] != DBNull.Value ? dr["PinCode"].ToString().Trim() : "",
                            Country = dr["Country"] != DBNull.Value ? dr["Country"].ToString().Trim() : "India",
                            Phone = dr["Phone"] != DBNull.Value ? dr["Phone"].ToString().Trim() : "",
                            AlternatePhone = dr["AlternatePhone"] != DBNull.Value ? dr["AlternatePhone"].ToString().Trim() : "",
                            Email = dr["Email"] != DBNull.Value ? dr["Email"].ToString().Trim() : "",
                            RFID_Tag = dr["RFID_Tag"] != DBNull.Value ? dr["RFID_Tag"].ToString().Trim() : "",
                            PortalAccess = dr["PortalAccess"] != DBNull.Value && Convert.ToBoolean(dr["PortalAccess"]),
                            PhotoUrl = dr["PhotoUrl"] != DBNull.Value ? dr["PhotoUrl"].ToString().Trim() : "",
                            Remarks = dr["Remarks"] != DBNull.Value ? dr["Remarks"].ToString().Trim() : ""
                        };
                    }
                }
            }

            return model;
        }



        #endregion

        #region Guardian Mapping Method

        /// <summary>
        /// Saves or updates a student-guardian mapping relationship.
        /// </summary>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public ServiceResponse SaveGuardianMap(
            string branch,
            int mapId,
            string studentBranch,
            string studentCode,
            int guardianId,
            string guardianBranch,
            string guardianCode,
            string relBranch,
            string relRid,
            string relCode,
            bool isPrimaryContact,
            bool isEmergencyContact,
            bool canPickup,
            bool canViewReportCard,
            bool canReceiveSms,
            bool canReceiveEmail,
            int contactPriority,
            string specificPhone,
            string machineId,
            string dmlStatus)
        {
            try
            {
                branch = ResolveBranch(branch);
                string userId = Session["UserId"]?.ToString() ?? "SYSTEM";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("dbo.sp_SaveStudentGuardianMap", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Branch", branch);
                    cmd.Parameters.AddWithValue("@MapID", mapId);
                    cmd.Parameters.AddWithValue("@Student_Branch", string.IsNullOrWhiteSpace(studentBranch) ? branch : studentBranch);
                    cmd.Parameters.AddWithValue("@Student_Code", (object)studentCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@GuardianID", guardianId);
                    cmd.Parameters.AddWithValue("@Guardian_Branch", string.IsNullOrWhiteSpace(guardianBranch) ? branch : guardianBranch);
                    cmd.Parameters.AddWithValue("@Guardian_Code", (object)guardianCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Rel_Branch", string.IsNullOrWhiteSpace(relBranch) ? branch : relBranch);
                    cmd.Parameters.AddWithValue("@Rel_RID", string.IsNullOrWhiteSpace(relRid) ? "RL" : relRid);
                    cmd.Parameters.AddWithValue("@Rel_Code", (object)relCode ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@IsPrimaryContact", isPrimaryContact);
                    cmd.Parameters.AddWithValue("@IsEmergencyContact", isEmergencyContact);
                    cmd.Parameters.AddWithValue("@CanPickup", canPickup);
                    cmd.Parameters.AddWithValue("@CanViewReportCard", canViewReportCard);
                    cmd.Parameters.AddWithValue("@CanReceiveSMS", canReceiveSms);
                    cmd.Parameters.AddWithValue("@CanReceiveEmail", canReceiveEmail);
                    cmd.Parameters.AddWithValue("@ContactPriority", contactPriority <= 0 ? 1 : contactPriority);
                    cmd.Parameters.AddWithValue("@SpecificPhone", (object)specificPhone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Machine_Id", (object)machineId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@DMLStatus", string.IsNullOrWhiteSpace(dmlStatus) ? "I" : dmlStatus);

                    cmd.Parameters.Add(new SqlParameter("@ReturnCode", SqlDbType.Int) { Direction = ParameterDirection.InputOutput, Value = 0 });
                    cmd.Parameters.Add(new SqlParameter("@ReturnMessage", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.InputOutput, Value = string.Empty });

                    conn.Open();
                    BindDeclaredParameters(cmd);
                    cmd.ExecuteNonQuery();

                    SqlParameter pOutCode = FindParameter(cmd, "@ReturnCode");
                    SqlParameter pOutMsg = FindParameter(cmd, "@ReturnMessage");
                    int returnCode = pOutCode != null && pOutCode.Value != DBNull.Value ? Convert.ToInt32(pOutCode.Value) : 0;
                    string returnMsg = pOutMsg != null && pOutMsg.Value != DBNull.Value ? pOutMsg.Value.ToString() : "";

                    return new ServiceResponse { Success = (returnCode == 1 || returnCode == 0), Message = returnMsg };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse { Success = false, Message = ex.Message };
            }
        }

        #endregion

        #region Helper Routines

        private string ResolveBranch(string branch)
        {
            if (string.IsNullOrWhiteSpace(branch))
            {
                branch = Session["BranchCode"]?.ToString()
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

        private static void BindDeclaredParameters(SqlCommand command)
        {
            var supplied = new List<SqlParameter>();
            foreach (SqlParameter parameter in command.Parameters)
            {
                supplied.Add(parameter);
            }

            SqlCommandBuilder.DeriveParameters(command);

            foreach (SqlParameter declared in command.Parameters)
            {
                foreach (SqlParameter parameter in supplied)
                {
                    if (string.Equals(declared.ParameterName, parameter.ParameterName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (parameter.Value != null)
                        {
                            declared.Value = parameter.Value;
                        }
                        break;
                    }
                }

                if (declared.Direction != ParameterDirection.ReturnValue && declared.Value == null)
                {
                    declared.Value = DBNull.Value;
                }
            }
        }

        private static SqlParameter FindParameter(SqlCommand command, string parameterName)
        {
            foreach (SqlParameter parameter in command.Parameters)
            {
                if (string.Equals(parameter.ParameterName, parameterName, StringComparison.OrdinalIgnoreCase))
                {
                    return parameter;
                }
            }
            return null;
        }

        #endregion
    }
}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Services;

//namespace SMS_Gem.Services
//{
//    /// <summary>
//    /// Summary description for StudentService1
//    /// </summary>
//    [WebService(Namespace = "http://tempuri.org/")]
//    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
//    [System.ComponentModel.ToolboxItem(false)]
//    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
//    // [System.Web.Script.Services.ScriptService]
//    public class StudentService1 : System.Web.Services.WebService
//    {

//        [WebMethod]
//        public string HelloWorld()
//        {
//            return "Hello World";
//        }
//    }
//}
