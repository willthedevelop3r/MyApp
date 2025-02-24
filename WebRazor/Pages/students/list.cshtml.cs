using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using WebClient.Pages;

public class StudentsModel : PageModel
{
    public void OnGet()
    {
        // Skipped direct database access to maintain RESTful principles.
        // Instead of fetching data here, I used JavaScript (fetch API) to retrieve data from a separate RESTful API.
    }
}