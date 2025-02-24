using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebClient.Pages.Students
{
    public class StudentAddModel : PageModel
    {
        public void OnGet()
        {
            // Skipped direct database access to maintain RESTful principles.
            // Instead of fetching data here, I used JavaScript (fetch API) to retrieve data from a separate RESTful API.
        }
    }
}
