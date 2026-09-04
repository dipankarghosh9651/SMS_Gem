using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using SMS.Models;

namespace SMS.DAL
{

    public class StudentRepository_CL
    {
        public class StudentRepository_GEM
        {
            private readonly string _connectionString = ConfigurationManager.ConnectionStrings["SMSDb"].ConnectionString;

            /// <summary>
            /// Retrieves master dropdown code/description pairs from h_Main_Master for a given branch and RID.
            /// </summary>
            /// <param name="branch">Branch identifier (e.g., "CAP")</param>
            /// <param name="rid">Master category RID from Web.config (e.g., "76", "MF", "SC")</param>
            /// <returns>DataTable containing Code and Desc columns</returns>
            public DataTable GetMasterList(string branch, string rid)
            {
                DataTable dt = new DataTable();

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    // Option A: Using Stored Procedure (Recommended)
                    using (SqlCommand cmd = new SqlCommand("dbo.usp_GetMasterLookups", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Branch", SqlDbType.VarChar, 50).Value = (object)branch ?? DBNull.Value;
                        cmd.Parameters.Add("@RID", SqlDbType.VarChar, 50).Value = (object)rid ?? DBNull.Value;

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            conn.Open();
                            da.Fill(dt);
                        }
                    }

                    /* Option B: Using Direct SQL Text (if stored procedure is not yet created)
                    string query = @"
                        SELECT 
                            Main_Code AS Code, 
                            Main_Descr AS [Desc] 
                        FROM h_Main_Master 
                        WHERE Branch = @Branch 
                          AND RID = @RID 
                          AND (IsActive = 1 OR IsActive IS NULL)
                        ORDER BY Main_Descr ASC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@Branch", SqlDbType.VarChar, 50).Value = (object)branch ?? DBNull.Value;
                        cmd.Parameters.Add("@RID", SqlDbType.VarChar, 50).Value = (object)rid ?? DBNull.Value;

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            conn.Open();
                            da.Fill(dt);
                        }
                    }
                    */
                } // Connection is automatically closed and returned to the pool

                return dt;
            }

            /// <summary>
            /// Generates the next sequential student code based on branch and settings.
            /// </summary>
            public string GenerateStudentCode(string branch)
            {
                string prefix = ConfigurationManager.AppSettings["Student.CodePrefix"] ?? "STU";
                int pad = Convert.ToInt32(ConfigurationManager.AppSettings["Student.CodePad"] ?? "7");

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(CAST(SUBSTRING(Student_Code, LEN(@Prefix) + 1, 20) AS INT)), 0) + 1 FROM Student_Master WHERE Branch = @Branch", conn))
                    {
                        cmd.Parameters.AddWithValue("@Prefix", prefix);
                        cmd.Parameters.AddWithValue("@Branch", (object)branch ?? DBNull.Value);

                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        int nextSeq = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 1;
                        return $"{prefix}{nextSeq.ToString().PadLeft(pad, '0')}";
                    }
                }
            }
        }

        public List<LookupItem> GetMasterLookups(string branch, string rid)
        {
            var list = new List<LookupItem>();
            using (var conn = DatabaseHelper.GetOpenConnection())
            using (var cmd = new SqlCommand("dbo.usp_MainMaster_GetList_CL", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Branch", (object)branch ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RID", (object)rid ?? DBNull.Value);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LookupItem
                        {
                            Code = reader["Main_Code"].ToString(),
                            Description = reader["Main_Descr"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public string GenerateNextStudentCode(string branch)
        {
            string prefix = ConfigurationManager.AppSettings["Student.CodePrefix"] ?? "STU";
            int pad = Convert.ToInt32(ConfigurationManager.AppSettings["Student.CodePad"] ?? "7");

            using (var conn = DatabaseHelper.GetOpenConnection())
            using (var cmd = new SqlCommand("dbo.usp_Student_GetNextCode_CL", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Branch", branch);
                object result = cmd.ExecuteScalar();
                int nextSeq = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 1;
                return $"{prefix}{nextSeq.ToString().PadLeft(pad, '0')}";
            }
        }

        public (bool Success, string Message) SaveStudent(StudentModel s, string user)
        {
            byte[] photoBytes = null;
            if (!string.IsNullOrWhiteSpace(s.StudentPhotoBase64))
            {
                try
                {
                    string base64 = s.StudentPhotoBase64;
                    int commaIndex = base64.IndexOf(',');
                    if (commaIndex >= 0)
                    {
                        base64 = base64.Substring(commaIndex + 1);
                    }

                    photoBytes = Convert.FromBase64String(base64);
                }
                catch (FormatException)
                {
                    return (false, "The student photograph is not a valid image.");
                }
            }

            var p = new List<SqlParameter>
            {
                new SqlParameter("@Branch", (object)s.Branch ?? DBNull.Value),
                new SqlParameter("@Student_Code", (object)s.StudentCode ?? DBNull.Value),
                new SqlParameter("@AdmissionNo", (object)s.AdmissionNo ?? DBNull.Value),
                new SqlParameter("@RollNumber", (object)s.RollNumber ?? DBNull.Value),
                new SqlParameter("@AdmissionCategory", (object)s.AdmissionCategoryCode ?? DBNull.Value),
                new SqlParameter("@EnrollmentDate", (object)s.EnrollmentDate ?? DBNull.Value),
                new SqlParameter("@MachineId", (object)s.MachineId ?? DBNull.Value),
                new SqlParameter("@IsActive", s.IsActive),
                new SqlParameter("@FirstName", (object)s.FirstName ?? DBNull.Value),
                new SqlParameter("@MiddleName", (object)s.MiddleName ?? DBNull.Value),
                new SqlParameter("@LastName", (object)s.LastName ?? DBNull.Value),
                new SqlParameter("@FatherName", (object)s.FatherName ?? DBNull.Value),
                new SqlParameter("@MotherName", (object)s.MotherName ?? DBNull.Value),
                new SqlParameter("@DateOfBirth", (object)s.DateOfBirth ?? DBNull.Value),
                new SqlParameter("@Gender", (object)s.GenderCode ?? DBNull.Value),
                new SqlParameter("@BloodGroup", (object)s.BloodGroupCode ?? DBNull.Value),
                new SqlParameter("@Nationality", (object)s.NationalityCode ?? DBNull.Value),
                new SqlParameter("@MotherTongue", (object)s.MotherTongueCode ?? DBNull.Value),
                new SqlParameter("@Religion", (object)s.ReligionCode ?? DBNull.Value),
                new SqlParameter("@Caste_Category", (object)s.Caste_Category ?? DBNull.Value),
                new SqlParameter("@AadhaarNumber", (object)s.AadhaarNumber ?? DBNull.Value),
                new SqlParameter("@PreviousSchool", (object)s.PreviousSchool ?? DBNull.Value),
                new SqlParameter("@TcNumber", (object)s.TcNumber ?? DBNull.Value),
                new SqlParameter("@AddressLine1", (object)s.AddressLine1 ?? DBNull.Value),
                new SqlParameter("@AddressLine2", (object)s.AddressLine2 ?? DBNull.Value),
                new SqlParameter("@City", (object)s.City ?? DBNull.Value),
                new SqlParameter("@State", (object)s.State ?? DBNull.Value),
                new SqlParameter("@PinCode", (object)s.PinCode ?? DBNull.Value),
                new SqlParameter("@Country", (object)s.Country ?? DBNull.Value),
                new SqlParameter("@Phone", (object)s.Phone ?? DBNull.Value),
                new SqlParameter("@AlternatePhone", (object)s.AlternatePhone ?? DBNull.Value),
                new SqlParameter("@Email", (object)s.Email ?? DBNull.Value),
                new SqlParameter("@RfidTag", (object)s.RfidTag ?? DBNull.Value),
                new SqlParameter("@PortalAccess", s.PortalAccess),
                new SqlParameter("@PhotoUrl", (object)s.PhotoUrl ?? DBNull.Value),
                new SqlParameter("@StudentPhoto", SqlDbType.VarBinary, -1)
                {
                    Value = (object)photoBytes ?? DBNull.Value
                },
                new SqlParameter("@Remarks", (object)s.Remarks ?? DBNull.Value),
                new SqlParameter("@User", (object)user ?? DBNull.Value)
            };

            var (returnCode, returnMessage) = DatabaseHelper.ExecuteNonQueryWithReturn("dbo.usp_SaveStudentMaster", p.ToArray());
            return (returnCode == 0, returnMessage);
        }

        public DataTable GetStudents(string branch, string searchTerm = "")
        {
            return DatabaseHelper.ExecuteQuery(
                "usp_GetStudentsList",
                new SqlParameter("@Branch", branch ?? "CAP"),
                new SqlParameter("@SearchTerm", (object)searchTerm ?? DBNull.Value)
            );
        }

    }
}