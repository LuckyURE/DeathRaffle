using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
//        [HttpPost]
//        [Route("login")]
//        public IActionResult Login([FromBody] User user)
//        {
//            try
//            {
////                var poolRepository = new LoginRepository();
////                return Ok(poolRepository.Login(user));
//            }
//            catch (Exception e)
//            {
//                AppSettings.Logger.Error($"Unable to connect to the database at: {AppSettings.Logger}");
//                AppSettings.Logger.Error($"The error was: {e.Message}");
//                return StatusCode(500);
//            }
//        }
    }
}