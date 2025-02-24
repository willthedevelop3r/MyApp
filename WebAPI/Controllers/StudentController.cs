using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly DatabaseHelper _db;

        public StudentsController(DatabaseHelper db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentDetails()
        {
            string sql = "";
            sql += "SELECT s.StudentId, s.FirstName, s.LastName, s.Major, s.DateModified, s.IsActive, ss.SchoolId, ss.SchoolName ";
            sql += "FROM Students s ";
            sql += "INNER JOIN Schools ss ON s.SchoolId = ss.SchoolId ";

            var students = await _db.QueryAsync<StudentDetails_OutputType>(sql);
            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(string id)
        {
            var errList = new List<string>();

            if (string.IsNullOrWhiteSpace(id))
            {
                errList.Add("Id is required and can not be empty");
            }
            else if (!Guid.TryParse(id, out _))
            {
                errList.Add("Id must be a valid Guid");
            }

            if (errList.Count > 0)
            {
                return BadRequest(new { errors = errList });
            }

            string sql = "SELECT * FROM Students WHERE StudentId = @Id";
            var student = await _db.QuerySingleAsync<StudentDetails_OutputType>(sql, new { Id = id });
            if (student == null) return NotFound("Student not found");
            return Ok(student);
        }

        [HttpPost]
        public async Task<IActionResult> AddStudent([FromBody] StudentDetails_InputType postObj)
        {
            var errList = new List<string>();

            if (string.IsNullOrWhiteSpace(postObj.FirstName))
            {
                errList.Add("First Name is required and can not be empty");
            }

            if (string.IsNullOrWhiteSpace(postObj.LastName))
            {
                errList.Add("Last Name is required and can not be empty");
            }

            if (string.IsNullOrWhiteSpace(postObj.SchoolId))
            {
                errList.Add("SchoolId is required and cannot be empty.");
            }
            else if (!Guid.TryParse(postObj.SchoolId.ToString(), out _))
            {
                errList.Add("SchoolId must be a valid GUID.");
            }

            if (string.IsNullOrWhiteSpace(postObj.Major))
            {
                errList.Add("Major is required and can not be empty");
            }

            if (errList.Count > 0)
            {
                return BadRequest(new { errors = errList });
            }

            try
            {
                postObj.StudentId = Guid.NewGuid().ToString();
                postObj.DateCreated = DateTime.UtcNow;
                postObj.DateModified = DateTime.UtcNow;

                string sql = @"INSERT INTO Students (StudentId, FirstName, LastName, SchoolId, Major, DateCreated, DateModified, IsActive) 
                             VALUES (@StudentId, @FirstName, @LastName, @SchoolId, @Major, @DateCreated, @DateModified, @IsActive)";

                int rowsAffected = await _db.ExecuteAsync(sql, postObj);

                if (rowsAffected == 0)
                {
                    errList.Add("Failed to add student");
                    return BadRequest(new { errors = errList });
                }
                else
                {
                    return Ok(new { message = "Student added successfully" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(string id, [FromBody] StudentDetails_InputType postObj)
        {
            var errList = new List<string>();

            if (string.IsNullOrWhiteSpace(id))
            {
                errList.Add("Id is required and can not be empty");
            }
            else if (!Guid.TryParse(id, out _))
            {
                errList.Add("Id must be a valid Guid");
            }

            if (string.IsNullOrWhiteSpace(postObj.FirstName))
            {
                errList.Add("First Name is required and can not be empty");
            }

            if (string.IsNullOrWhiteSpace(postObj.LastName))
            {
                errList.Add("Last Name is required and can not be empty");
            }

            if (string.IsNullOrWhiteSpace(postObj.SchoolId))
            {
                errList.Add("SchoolId is required and cannot be empty.");
            }
            else if (!Guid.TryParse(postObj.SchoolId, out _))
            {
                errList.Add("SchoolId must be a valid GUID.");
            }

            if (string.IsNullOrWhiteSpace(postObj.Major))
            {
                errList.Add("Major is required and can not be empty");
            }

            if (errList.Count > 0)
            {
                return BadRequest(new { errors = errList });
            }

            string sql = @"UPDATE Students SET 
                            FirstName = @FirstName, 
                            LastName = @LastName, 
                            SchoolId = @SchoolId,
                            Major = @Major, 
                            IsActive = @IsActive, 
                            DateModified = CURRENT_TIMESTAMP
                            WHERE StudentId = @Id";

            int rowsAffected = await _db.ExecuteAsync(sql, new
            {
                postObj.FirstName,
                postObj.LastName,
                postObj.SchoolId,
                postObj.Major,
                postObj.IsActive,
                Id = id
            });

            if (rowsAffected == 0) 
            {
                errList.Add("Failed to update student");
                return BadRequest(new { errors = errList });
            }
            else
            {
                return Ok(new { message = "Student updated successfully" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteStudents([FromBody] StudentIds studentIds)
        {
            var errList = new List<string>();

            if (studentIds == null || studentIds.Ids == null || studentIds.Ids.Count == 0)
            {
                errList.Add("No students IDs provided");
                return BadRequest(new { errors = errList});
            }

            try
            {
                string sql = "DELETE FROM Students WHERE StudentId IN @Ids";
                int rowsAffected = await _db.ExecuteAsync(sql, new { Ids = studentIds.Ids });

                if (rowsAffected > 0)
                    return Ok(new { message = $"{rowsAffected} students deleted successfully." });

                errList.Add("An error occured");
                return NotFound(new { errors = errList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        public class StudentIds
        {
            public List<string> Ids { get; set; }
        }
        public class StudentDetails_OutputType
        {
            public string? StudentId { get; set; }

            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? SchoolId { get; set; }
            public string? SchoolName { get; set; }
            public string? Major { get; set; }
            public bool IsActive { get; set; }
            public string? DateCreated { get; set; }
            public string? DateModified { get; set; }
        }

        public class StudentDetails_InputType
        {
            public string? StudentId { get; set; }

            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? SchoolId { get; set; }
            public string? SchoolName { get; set; }
            public string? Major { get; set; }
            public bool IsActive { get; set; }
            public DateTime? DateCreated { get; set; }
            public DateTime? DateModified { get; set; }
        }
    }
}