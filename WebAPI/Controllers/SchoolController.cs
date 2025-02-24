using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/schools")]
    public class SchoolController : Controller
    {
        private readonly DatabaseHelper _db;

        public SchoolController(DatabaseHelper db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetSchoolList()
        {
            string sql = "";
            sql += "SELECT * FROM Schools ";
            var schools = await _db.QueryAsync<School_OutputType>(sql);
            return Ok(schools);
        }

        [HttpPost]
        public async Task<IActionResult> AddSchool([FromBody] School_InputType postObj)
        {
            var errList = new List<string>();

            if (string.IsNullOrWhiteSpace(postObj.SchoolName))
            {
                errList.Add("School Name is required and can not be empty");
            }

            if (string.IsNullOrWhiteSpace(postObj.AddressLine1))
            {
                errList.Add("Address Line 1 is required and can not be empty");
            }

            if (string.IsNullOrWhiteSpace(postObj.AddressLine2))
            {
                postObj.AddressLine2 = "";
            }

            if (string.IsNullOrWhiteSpace(postObj.City))
            {
                errList.Add("City is required and cannot be empty.");
            }
         
            if (string.IsNullOrWhiteSpace(postObj.State))
            {
                errList.Add("State is required and can not be empty");
            }

            if (string.IsNullOrWhiteSpace(postObj.ZipCode))
            {
                errList.Add("Zip code is required and can not be empty");
            }
            else if (!int.TryParse(postObj.ZipCode, out _) || postObj.ZipCode.Length != 5)
            {
                errList.Add("Zip code must be a valid integer and must have a length of 5");
            }

            if (errList.Count > 0)
            {
                return BadRequest(new { errors = errList });
            }

            try
            {
                postObj.SchoolId = Guid.NewGuid().ToString();
                postObj.DateCreated = DateTime.UtcNow;
                postObj.DateModified = DateTime.UtcNow;

                string sql = @"INSERT INTO Schools (SchoolId, SchoolName, AddressLine1, AddressLine2, City, State, ZipCode, DateCreated, DateModified) 
                             VALUES (@SchoolId, @SchoolName, @AddressLine1, @AddressLine2, @City, @State, @ZipCode, @DateCreated, @DateModified)";

                int rowsAffected = await _db.ExecuteAsync(sql, postObj);

                if (rowsAffected == 0)
                {
                    errList.Add("Failed to add school");
                    return BadRequest(new { errors = errList });
                }
                else
                {
                    return Ok(new { message = "School added successfully" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        public class School_OutputType
        {
            public string? SchoolId { get; set; }
            public string? SchoolName { get; set; }
            public string? AddressLine1 { get; set; }
            public string? AddressLine2{ get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
            public string? ZipCode { get; set; }
            public DateTime? DateCreated { get; set; }
            public DateTime? DateModified { get; set; }

        }

        public class School_InputType
        {
            public string? SchoolId { get; set; }
            public string? SchoolName { get; set; }
            public string? AddressLine1 { get; set; }
            public string? AddressLine2 { get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
            public string? ZipCode { get; set; }
            public DateTime? DateCreated { get; set; }
            public DateTime? DateModified { get; set; }
        }
    }
}
