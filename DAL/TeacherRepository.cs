using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using SMS.Models;
    

namespace SMS.DAL
{
    public class TeacherRepository
    {
        private readonly string _connectionString;

        public TeacherRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SMSDb"]?.ConnectionString
                                ?? ConfigurationManager.ConnectionStrings["SMSConnectionString"]?.ConnectionString
                                ?? ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

            if (string.IsNullOrWhiteSpace(_connectionString))
                throw new InvalidOperationException("No database connection string is configured. Expected 'SMSDb'.");
        }

        public string GenerateNextTeacherCode(string branch)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.usp_GetNextTeacherCode", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Branch", branch);
                conn.Open();
                var res = cmd.ExecuteScalar();
                return res != null ? res.ToString() : "T0001";
            }
        }

        public List<LookupItem> GetMasterLookups(string branch, string rid)
        {
            var list = new List<LookupItem>();
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.usp_MainMaster_GetList_CL", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Branch", branch);
                cmd.Parameters.AddWithValue("@RID", rid);
                conn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new LookupItem
                        {
                            Code = dr["Code"].ToString(),
                            Description = dr["Desc"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public (bool Success, string Message) SaveTeacher(TeacherModel model, string user)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("dbo.usp_SaveTeacher", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Branch", model.Branch ?? "CAP");
                    cmd.Parameters.AddWithValue("@Teacher_Code", model.Teacher_Code);
                    cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("@MiddleName", (object)model.MiddleName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastName", model.LastName);
                    cmd.Parameters.AddWithValue("@DateOfBirth", model.DateOfBirth);
                    cmd.Parameters.AddWithValue("@Dept_Branch", model.Dept_Branch ?? "CAP");
                    cmd.Parameters.AddWithValue("@Dept_RID", model.Dept_RID ?? "FN");
                    cmd.Parameters.AddWithValue("@Dept_Code", model.Dept_Code);
                    cmd.Parameters.AddWithValue("@Gender", (object)model.Gender ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AddressLine1", (object)model.AddressLine1 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AddressLine2", (object)model.AddressLine2 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", (object)model.City ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@State", (object)model.State ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PinCode", (object)model.PinCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Country", (object)model.Country ?? "India");
                    cmd.Parameters.AddWithValue("@Phone", (object)model.Phone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AlternatePhone", (object)model.AlternatePhone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)model.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@HireDate", model.HireDate);
                    cmd.Parameters.AddWithValue("@Qualification", (object)model.Qualification ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Specialization", (object)model.Specialization ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Experience_Years", (object)model.Experience_Years ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhotoUrl", (object)model.PhotoUrl ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@GovtIdNumber", (object)model.AadhaarNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PAN_Number", (object)model.PAN_Number ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BankAccountNo", (object)model.BankAccountNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IFSC_Code", (object)model.IFSC_Code ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PF_Number", (object)model.PF_Number ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
                    cmd.Parameters.AddWithValue("@Machine_Id", (object)model.Machine_Id ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@User", user ?? "SYSTEM");

                    if (!string.IsNullOrWhiteSpace(model.TeacherPhotoBase64))
                    {
                        byte[] photoBytes = Convert.FromBase64String(model.TeacherPhotoBase64);
                        cmd.Parameters.Add("@TeacherPhoto", SqlDbType.VarBinary, -1).Value = photoBytes;
                    }
                    else
                    {
                        cmd.Parameters.Add("@TeacherPhoto", SqlDbType.VarBinary, -1).Value = DBNull.Value;
                    }

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return (true, "Teacher profile successfully recorded.");
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public TeacherModel GetTeacherByCode(string branch, string teacherCode)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.usp_GetTeacherByCode", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Branch", branch);
                cmd.Parameters.AddWithValue("@Teacher_Code", teacherCode);
                conn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new TeacherModel
                        {
                            Branch = dr["Branch"].ToString(),
                            Teacher_Code = dr["Teacher_Code"].ToString(),
                            FirstName = dr["FirstName"].ToString(),
                            MiddleName = dr["MiddleName"] != DBNull.Value ? dr["MiddleName"].ToString() : null,
                            LastName = dr["LastName"].ToString(),
                            DateOfBirth = Convert.ToDateTime(dr["DateOfBirth"]),
                            Dept_Branch = dr["Dept_Branch"].ToString(),
                            Dept_RID = dr["Dept_RID"].ToString(),
                            Dept_Code = dr["Dept_Code"].ToString(),
                            Gender = dr["Gender"] != DBNull.Value ? dr["Gender"].ToString() : null,
                            AddressLine1 = dr["AddressLine1"] != DBNull.Value ? dr["AddressLine1"].ToString() : null,
                            AddressLine2 = dr["AddressLine2"] != DBNull.Value ? dr["AddressLine2"].ToString() : null,
                            City = dr["City"] != DBNull.Value ? dr["City"].ToString() : null,
                            State = dr["State"] != DBNull.Value ? dr["State"].ToString() : null,
                            PinCode = dr["PinCode"] != DBNull.Value ? dr["PinCode"].ToString() : null,
                            Country = dr["Country"] != DBNull.Value ? dr["Country"].ToString() : null,
                            Phone = dr["Phone"] != DBNull.Value ? dr["Phone"].ToString() : null,
                            AlternatePhone = dr["AlternatePhone"] != DBNull.Value ? dr["AlternatePhone"].ToString() : null,
                            Email = dr["Email"] != DBNull.Value ? dr["Email"].ToString() : null,
                            HireDate = Convert.ToDateTime(dr["HireDate"]),
                            Qualification = dr["Qualification"] != DBNull.Value ? dr["Qualification"].ToString() : null,
                            Specialization = dr["Specialization"] != DBNull.Value ? dr["Specialization"].ToString() : null,
                            Experience_Years = dr["Experience_Years"] != DBNull.Value ? Convert.ToInt32(dr["Experience_Years"]) : (int?)null,
                            PhotoUrl = dr["PhotoUrl"] != DBNull.Value ? dr["PhotoUrl"].ToString() : null,
                            AadhaarNumber = dr["AadhaarNumber"] != DBNull.Value ? dr["AadhaarNumber"].ToString() : null,
                            PAN_Number = dr["PAN_Number"] != DBNull.Value ? dr["PAN_Number"].ToString() : null,
                            BankAccountNo = dr["BankAccountNo"] != DBNull.Value ? dr["BankAccountNo"].ToString() : null,
                            IFSC_Code = dr["IFSC_Code"] != DBNull.Value ? dr["IFSC_Code"].ToString() : null,
                            PF_Number = dr["PF_Number"] != DBNull.Value ? dr["PF_Number"].ToString() : null,
                            IsActive = Convert.ToBoolean(dr["IsActive"]),
                            Machine_Id = dr["Machine_Id"] != DBNull.Value ? dr["Machine_Id"].ToString() : null,
                            TeacherPhotoBase64 = dr["TeacherPhoto"] != DBNull.Value ? Convert.ToBase64String((byte[])dr["TeacherPhoto"]) : null
                        };
                    }
                }
            }
            return null;
        }
    }
}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace SMS_Gem.DAL
//{
//    public class TeacherRepository
//    {
//    }
//}