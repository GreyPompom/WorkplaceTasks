//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc;
//using Workplace.Tasks.Api.Models;
//using System.Security.Claims;

//namespace Workplace.Tasks.Api.Controllers
//{
//    [Authorize]
//    [Route("api/[controller]")]
//    public class UsersController : BaseController
//    {
//        [HttpGet("me")]
//        public IActionResult GetProfile()
//        {
//            var (id, role, email, name) = GetUserInfo();
//            return Ok(new { id, name, email, role });
//        }
//    }
//}
