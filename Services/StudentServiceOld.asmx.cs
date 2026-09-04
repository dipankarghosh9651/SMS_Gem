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

        // 1. Strongly-Typed Data Transfer Classes
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

        public class StudentInputModel
        {
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
            public string GovtIdNumber { get; set; }
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
            public string StudentPhoto { get; set; }
            public string Remarks { get; set; }
        }


        //private readonly string _connectionString = ConfigurationManager.ConnectionStrings["SMSDb"].ConnectionString;

        //public class LookupItem
        //{
        //    public string Code { get; set; }
        //    public string Desc { get; set; }
        //}

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

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public AllLookupsResponse GetAllLookups()
        {
            var result = new AllLookupsResponse();
            string branch = Session["BranchCode"]?.ToString()
                            ?? ConfigurationManager.AppSettings["Default.Branch"]
                            ?? "CAP";
            branch = branch.PadRight(3).Substring(0, 3);

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
                                    Code = dr["Code"] != DBNull.Value ? dr["Code"].ToString().Trim() : "",
                                    Desc = dr["Desc"] != DBNull.Value ? dr["Desc"].ToString().Trim() : ""
                                });
                            }
                        }
                    }
                }
            }

            return result;
        }

        private static string PadRid(string rid)
        {
            if (string.IsNullOrWhiteSpace(rid)) return "".PadRight(4);
            return rid.Trim().PadRight(4).Substring(0, 4);
        }




        // 2. Dropdown Lookup WebMethod
        //[WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //public List<LookupItem> GetLookup(string lookupType)
        //{
        //    var list = new List<LookupItem>();

        //    // 1. Resolve Branch (CHAR(3))
        //    string branch = Session["BranchCode"]?.ToString()
        //                    ?? ConfigurationManager.AppSettings["Default.Branch"]
        //                    ?? "CAP";
        //    branch = (branch.Length > 3 ? branch.Substring(0, 3) : branch).Trim();

        //    // 2. Resolve RID (CHAR(4)) from Web.config
        //    string rid = ResolveRid(lookupType);
        //    //string rid = (lookupType);

        //    if (string.IsNullOrWhiteSpace(rid)) return list;
        //    rid = (rid.Length > 4 ? rid.Substring(0, 4) : rid).Trim();

        //    // 3. Execute with explicit SqlDbType.Char(3) and SqlDbType.Char(4)
        //    using (SqlConnection conn = new SqlConnection(_connectionString))
        //    using (SqlCommand cmd = new SqlCommand("dbo.usp_MainMaster_GetList_CL", conn))
        //    {
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cmd.Parameters.Add(new SqlParameter("@Branch", SqlDbType.Char, 3) { Value = branch });
        //        cmd.Parameters.Add(new SqlParameter("@RID", SqlDbType.Char, 4) { Value = rid });

        //        conn.Open();
        //        using (SqlDataReader dr = cmd.ExecuteReader())
        //        {
        //            while (dr.Read())
        //            {
        //                list.Add(new LookupItem
        //                {
        //                    Code = dr["Code"] != DBNull.Value ? dr["Code"].ToString().Trim() : "",
        //                    Desc = dr["Desc"] != DBNull.Value ? dr["Desc"].ToString().Trim() : ""
        //                });
        //            }
        //        }
        //    }
        //    return list;
        //}

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<LookupItem> GetLookup(string lookupType)
        {
            var list = new List<LookupItem>();

            if (string.IsNullOrWhiteSpace(lookupType)) return list;

            // 1. Resolve Branch
            string branch = Session["BranchCode"]?.ToString()
                            ?? ConfigurationManager.AppSettings["Default.Branch"]
                            ?? "CAP";
            branch = (branch.Length > 3 ? branch.Substring(0, 3) : branch).Trim();

            // 2. Resolve RID
            string rid = ResolveRid(lookupType);
            if (string.IsNullOrWhiteSpace(rid)) return list;
            rid = (rid.Length > 4 ? rid.Substring(0, 4) : rid).Trim();

            // 3. Execute Stored Procedure
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
                        // Read by column index (0 = Code, 1 = Desc) to prevent any column naming mismatches
                        string code = dr[0] != DBNull.Value ? dr[0].ToString().Trim() : "";
                        string desc = dr[1] != DBNull.Value ? dr[1].ToString().Trim() : code;

                        list.Add(new LookupItem
                        {
                            Code = code,
                            Desc = desc
                        });
                    }
                }
            }
            return list;
        }




        // 3. Next Student Code Sequence WebMethod
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetNextStudentCode()
        {
            string branch = Session["BranchCode"]?.ToString() ?? ConfigurationManager.AppSettings["Default.Branch"] ?? "CAP"; 
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

        // 4. Save Record WebMethod
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public ServiceResponse SaveStudent(StudentInputModel model)
        {
            try
            {
                if (model == null)
                {
                    return new ServiceResponse { Success = false, Message = "Invalid input payload." };
                }


                // Convert base64 string to byte array for VARBINARY(MAX)
                byte[] photoBytes = null;
                if (!string.IsNullOrWhiteSpace(model.StudentPhoto))
                {
                    try
                    {
                        // Check if the string starts with a valid data URI scheme
                        if (model.StudentPhoto.StartsWith("data:image/jpeg;base64,", StringComparison.OrdinalIgnoreCase))
                        {
                            // Remove the data URI scheme part
                            string base64Data = model.StudentPhoto.Substring(model.StudentPhoto.IndexOf(',') + 1);
                            photoBytes = Convert.FromBase64String(base64Data);
                        }
                        else
                        {
                            photoBytes = Convert.FromBase64String(model.StudentPhoto);
                        }
                    }
                    catch
                    {
                        photoBytes = null;
                    }
                }




                string branch = Session["BranchCode"]?.ToString() ?? ConfigurationManager.AppSettings["Default.Branch"] ?? "CAP"; 

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("dbo.usp_Student_Insert", conn))
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
                    cmd.Parameters.AddWithValue("@AadhaarNumber", (object)model.GovtIdNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PreviousSchool", (object)model.PreviousSchool ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TC_Number", (object)model.TC_Number ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@AddressLine1", (object)model.AddressLine1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AddressLine2", (object)model.AddressLine2 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", (object)model.City ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@State", (object)model.State ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PinCode", (object)model.PinCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Country", (object)model.Country ?? "India");

                    cmd.Parameters.AddWithValue("@Phone", (object)model.Phone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AlternatePhone", (object)model.AlternatePhone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)model.Email ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@RFID_Tag", (object)model.RFID_Tag ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PortalAccess", model.PortalAccess);
                    cmd.Parameters.AddWithValue("@PhotoUrl", (object)model.PhotoUrl ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@StudentPhoto", (object)model.StudentPhoto ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Remarks", (object)model.Remarks ?? DBNull.Value);
                    cmd.Parameters.Add(new SqlParameter("@ReturnCode", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.InputOutput,
                        Value = 0
                    });
                    cmd.Parameters.Add(new SqlParameter("@ReturnMessage", SqlDbType.NVarChar, 255)
                    {
                        Direction = ParameterDirection.InputOutput,
                        Value = string.Empty
                    });

                    conn.Open();
                    BindDeclaredParameters(cmd);
                    cmd.ExecuteNonQuery();

                    SqlParameter pOutCode = FindParameter(cmd, "@ReturnCode");
                    SqlParameter pOutMsg = FindParameter(cmd, "@ReturnMessage");
                    int returnCode = pOutCode != null && pOutCode.Value != DBNull.Value
                        ? Convert.ToInt32(pOutCode.Value)
                        : 0;
                    string returnMsg = pOutMsg != null && pOutMsg.Value != DBNull.Value
                        ? pOutMsg.Value.ToString()
                        : "";

                    return new ServiceResponse { Success = (returnCode == 0), Message = returnMsg };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse { Success = false, Message = ex.Message };
            }
        }

        private string ResolveRid(string lookupType)
        {
            switch ((lookupType ?? "").Trim().ToUpperInvariant())
            {
                case "BLOODGROUP": return ConfigurationManager.AppSettings["RID.BloodGroup"]; 
                case "NATIONALITY": return ConfigurationManager.AppSettings["RID.Nationality"]; 
                case "MOTHERTONGUE": return ConfigurationManager.AppSettings["RID.MotherTongue"]; 
                case "ADMISSIONCATEGORY": return ConfigurationManager.AppSettings["RID.AdmissionCategory"]; 
                case "CASTE_CATEGORY": return ConfigurationManager.AppSettings["RID.Caste_Category"]; 
                case "RELIGION": return ConfigurationManager.AppSettings["RID.Religion"]; 
                case "GENDER": return ConfigurationManager.AppSettings["RID.Gender"]; 
                default: return null;
            }
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
                    if (string.Equals(declared.ParameterName, parameter.ParameterName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        if (parameter.Value != null)
                        {
                            declared.Value = ConvertParameterValue(parameter.Value, declared.SqlDbType);
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
                if (string.Equals(parameter.ParameterName, parameterName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return parameter;
                }
            }

            return null;
        }

        private static object ConvertParameterValue(object value, SqlDbType targetType)
        {
            if (targetType == SqlDbType.Binary ||
                targetType == SqlDbType.VarBinary ||
                targetType == SqlDbType.Image)
            {
                if (value is string text)
                {
                    int commaIndex = text.IndexOf(',');
                    if (commaIndex >= 0)
                    {
                        text = text.Substring(commaIndex + 1);
                    }

                    return Convert.FromBase64String(text);
                }
            }

            return value;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public AllLookupsResponse GetAllLookupsX()
        {
            var result = new AllLookupsResponse();

            try
            {
                string branch = Session["BranchCode"]?.ToString()
                                ?? ConfigurationManager.AppSettings["Default.Branch"]
                                ?? "CAP";
                branch = SafeFormat(branch, 3);

                var map = new Dictionary<string, (string RID, List<LookupItem> Target)>
                {
                    { "BloodGroup",        (SafeFormat(ConfigurationManager.AppSettings["RID.BloodGroup"], 4), result.BloodGroup) },
                    { "Gender",            (SafeFormat(ConfigurationManager.AppSettings["RID.Gender"], 4), result.Gender) },
                    { "Nationality",       (SafeFormat(ConfigurationManager.AppSettings["RID.Nationality"], 4), result.Nationality) },
                    { "MotherTongue",      (SafeFormat(ConfigurationManager.AppSettings["RID.MotherTongue"], 4), result.MotherTongue) },
                    { "AdmissionCategory", (SafeFormat(ConfigurationManager.AppSettings["RID.AdmissionCategory"], 4), result.AdmissionCategory) },
                    { "Religion",          (SafeFormat(ConfigurationManager.AppSettings["RID.Religion"], 4), result.Religion) },
                    { "Caste_Category",    (SafeFormat(ConfigurationManager.AppSettings["RID.Caste_Category"], 4), result.Caste_Category) }
                };

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    foreach (var kvp in map)
                    {
                        string rid = kvp.Value.RID;
                        var targetList = kvp.Value.Target;

                        if (string.IsNullOrWhiteSpace(rid)) continue;

                        using (SqlCommand cmd = new SqlCommand("dbo.usp_MainMaster_GetList_CL", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@Branch", SqlDbType.Char, 3) { Value = branch });
                            cmd.Parameters.Add(new SqlParameter("@RID", SqlDbType.Char, 4) { Value = rid });

                            using (SqlDataReader dr = cmd.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    targetList.Add(new LookupItem
                                    {
                                        Code = dr["Code"] != DBNull.Value ? dr["Code"].ToString().Trim() : "",
                                        Desc = dr["Desc"] != DBNull.Value ? dr["Desc"].ToString().Trim() : ""
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or rethrow for inspection
                throw new ApplicationException("Error fetching lookups: " + ex.Message, ex);
            }

            return result;
        }

        private static string SafeFormat(string val, int length)
        {
            if (string.IsNullOrWhiteSpace(val)) return string.Empty;
            val = val.Trim();
            return val.Length > length ? val.Substring(0, length) : val;
        }


    }
}