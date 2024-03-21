using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using users_api_dotnet.context;
using users_api_dotnet.Models;

namespace users_api_dotnet.Controllers
{   
    [Authorize(Roles = "admin")]
    [Route("/[controller]")]
    [ApiController]
    public class AdminController :ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("/api/admin/users-stats")]
        
        public async Task<IActionResult> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {


            var users = await _context.Users
                                            .OrderBy(u => u.Id)
                                            .Skip((pageNumber - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToListAsync();

            
                                                
            return Ok(users);
        }   
    }
}