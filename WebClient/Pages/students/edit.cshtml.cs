using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebRazor.Pages.students
{
    public class editModel : PageModel
    {
        public void OnGet()
        {
            // Skipped direct database access to maintain RESTful principles.
            // Instead of fetching data here, I used JavaScript (fetch API) to retrieve data from a separate RESTful API.
        }
    }
}
