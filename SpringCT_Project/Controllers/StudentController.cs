using Microsoft.AspNetCore.Mvc;
using SpringCT_Project.Model.AddStudents;
using SpringCT_Project.Model.AssignCourseToStudent;
using SpringCT_Project.Model.ListStudents;
using SpringCT_Project.Services.Interfaces;

namespace SpringCT_Project.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public List<ListStudentResponse> Index()
        {
            return _studentService.ListStudents();
        }

        [HttpPost]
        public string AddStudent([FromBody] AddStudentRequest studentRequest)
        {
            return _studentService.AddStudent(studentRequest);
        }

        [HttpPost]
        [Route("AssignCourses")]
        public string AssignCourseToStudent([FromBody] AssignCourseRequest assignCourseRequest)
        {
            return _studentService.AssignCourseToStudent(assignCourseRequest);
        }
    }
}
