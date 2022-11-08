using SpringCT_Project.Model.AddStudents;
using SpringCT_Project.Model.AssignCourseToStudent;
using SpringCT_Project.Model.ListStudents;
using SpringCT_Project.Services.Interfaces;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SpringCT_Project.Services
{
    public class StudentService : IStudentService
    {
        private readonly string ConnectionString;
        public StudentService(IConfiguration config)
        {
            ConnectionString = config.GetConnectionString("SpringCT");
        }
        public string AddStudent(AddStudentRequest studentRequest)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    string query = "INSERT INTO STUDENT (Name, Phone, Email) VALUES (@Name, @Phone, @Email)";
                    using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                    {
                        cmd.Parameters.Add("@Name", SqlDbType.VarChar, 1000).Value = studentRequest.Name;
                        cmd.Parameters.Add("@Phone", SqlDbType.VarChar, 1000).Value = studentRequest.Phone;
                        cmd.Parameters.Add("@Email", SqlDbType.VarChar, 1000).Value = studentRequest.Email;
                        sqlConnection.Open();
                        cmd.ExecuteNonQuery();
                        sqlConnection.Close();
                    }
                }
                return "SUCCESS";
            }
            catch (Exception ex)
            {
                return "FAILURE";
            }
        }

        public string AssignCourseToStudent(AssignCourseRequest assignCourseRequest)
        {
            var courses = assignCourseRequest.CourseId.Split(',');
             
            try
            {
                foreach (var course in courses)
                {
                    using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                    {
                        string query = "INSERT INTO COURSES_ENROLLED (StudentId, CourseId) VALUES (@StudentId, @CourseId)";
                        using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                        {
                            cmd.Parameters.Add("@StudentId", SqlDbType.Int).Value = assignCourseRequest.StudentId;
                            cmd.Parameters.Add("@CourseId", SqlDbType.Int).Value = Convert.ToInt32(course.Trim());
                            sqlConnection.Open();
                            cmd.ExecuteNonQuery();
                            sqlConnection.Close();
                        }
                    }
                }
                return "SUCCESS";
            }
            catch (Exception ex)
            {
                return "FAILURE";
            }
        }

        public List<ListStudentResponse> ListStudents()
        {
            //SqlDataReader reader = null;
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    string query = "SELECT st.ID, st.NAME, st.EMAIL, st.PHONE, c.NAME as COURSENAME FROM STUDENT st INNER JOIN COURSES_ENROLLED cr ON ST.Id = cr.StudentId " +
                                   "INNER JOIN COURSES c ON cr.CourseId = c.Id";
                    using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                    {
                        sqlConnection.Open();
                        var reader = cmd.ExecuteReader();// .ExecuteReader();
                        if (reader != null)
                        {
                            return ParseDataOutput(reader);
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<ListStudentResponse> ParseDataOutput(SqlDataReader result)
        {
            List<ListStudentResponse> studentDetails = new List<ListStudentResponse>();
            List<ListStudentResponse> studentData = new List<ListStudentResponse>();

            while (result.Read())
            {
                var student = new ListStudentResponse();
                student.StudentId = (result.GetValue(result.GetOrdinal("ID"))).ToString();
                student.StudentName = (string)result.GetValue(result.GetOrdinal("NAME")).ToString();
                student.StudentEmail = (string)result.GetValue(result.GetOrdinal("EMAIL")).ToString();
                student.StudentPhoneNumber = (string)result.GetValue(result.GetOrdinal("PHONE")).ToString();
                student.CourseEnrolled = (string)result.GetValue(result.GetOrdinal("COURSENAME")).ToString();
                studentDetails.Add(student);
            }

            var distinctStudentIds = studentDetails.Select(x => x.StudentId).Distinct();

            foreach (var studentId in distinctStudentIds)
            {
                var st = new ListStudentResponse();

                var student = studentDetails.FindAll(x => x.StudentId == studentId);     //.All(x => x.StudentId == studentId);

                st.StudentId = studentId;
                st.StudentName = student.FirstOrDefault(x => x.StudentPhoneNumber != null).StudentName;
                st.StudentEmail = student.FirstOrDefault(x => x.StudentPhoneNumber != null).StudentEmail;
                st.StudentPhoneNumber = student.FirstOrDefault(x => x.StudentPhoneNumber != null).StudentPhoneNumber;

                var courses = studentDetails.Where(x => x.StudentId == studentId).Select(x => x.CourseEnrolled);
                string coursesEnrolled = "";
                foreach (var course in courses)
                {
                    coursesEnrolled = coursesEnrolled + course.ToString() + ", ";
                }
                st.CourseEnrolled = coursesEnrolled.Trim().Remove(coursesEnrolled.Trim().Length - 1, 1);
                studentData.Add(st);
            }
            return studentData;
        }
    }
}
