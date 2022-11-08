using SpringCT_Project.Model.AddStudents;
using SpringCT_Project.Model.AssignCourseToStudent;
using SpringCT_Project.Model.ListStudents;

namespace SpringCT_Project.Services.Interfaces
{
    public interface IStudentService
    {
        public string AddStudent(AddStudentRequest studentRequest);
        public string AssignCourseToStudent(AssignCourseRequest assignCourseRequest);
        public List<ListStudentResponse> ListStudents();
    }
}
