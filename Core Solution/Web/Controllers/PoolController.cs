using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Repository.Repositories;
using Web.Controllers.Models;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    public class PoolController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var poolRepository = new PoolRepository();
                return Ok(poolRepository.GetAll());
            }
            catch (Exception e)
            {
                AppSettings.Logger.Error($"Unable to connect to the database at: {AppSettings.Logger}");
                AppSettings.Logger.Error($"The error was: {e.Message}");
                return StatusCode(500);
            }
        }

        [HttpGet("id")]
        public IActionResult Get(int id)
        {
            try
            {
                var poolRepository = new PoolRepository();
                return Ok(poolRepository.GetById(id));
            }
            catch (Exception e)
            {
                AppSettings.Logger.Error($"Unable to connect to the database at: {AppSettings.Logger}");
                AppSettings.Logger.Error($"The error was: {e.Message}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("poolDashboard")]
        public IActionResult GetPoolDashboard()
        {
            try
            {
                var poolRepository = new PoolRepository();
                var results = poolRepository.GetDashboardPools();

                return Ok(results.Select(p => new PoolDashboardResult()
                {
                    PoolId = p.PoolId,
                    TicketCount = p.TicketCount,
                    CreateDate = p.CreateDate,
                    GameStarted = p.GameStarted != DateTime.Parse("0001-01-01T00:00:00")
                }));
            }
            catch (Exception e)
            {
                AppSettings.Logger.Error($"Unable to connect to the database at: {AppSettings.Logger}");
                AppSettings.Logger.Error($"The error was: {e.Message}");
                return StatusCode(500);
            }
        }
        
        [HttpGet]
        [Route("adminPools")]
        public IActionResult GetClosedPools([FromQuery] bool active)
        {
            try
            {
                var poolRepository = new PoolRepository();
                var results = poolRepository.GetAdminDashboardPools(active);

                return Ok(results.Select(p => new AdminDashboardPoolsResult()
                {
                    Id = p.Id,
                    TicketCount = p.TicketCount,
                    CreateDate = p.CreateDate,
                    GameStarted = p.GameStarted,
                    GameEnded = p.GameEnded
                }));
            }
            catch (Exception e)
            {
                AppSettings.Logger.Error($"Unable to connect to the database at: {AppSettings.Logger}");
                AppSettings.Logger.Error($"The error was: {e.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] Pool pool)
        {
            try
            {
                var poolRepository = new PoolRepository();
                poolRepository.Add(pool);
                return Ok();
            }
            catch (Exception e)
            {
                AppSettings.Logger.Error($"Unable to connect to the database at: {AppSettings.Logger}");
                AppSettings.Logger.Error($"The error was: {e.Message}");
                return StatusCode(500);
            }
        }

        [HttpPut]
        public IActionResult Put([FromBody] Pool pool)
        {
            try
            {
                var poolRepository = new PoolRepository();
                poolRepository.Update(pool);
                return Ok();
            }
            catch (Exception e)
            {
                AppSettings.Logger.Error($"Unable to connect to the database at: {AppSettings.Logger}");
                AppSettings.Logger.Error($"The error was: {e.Message}");
                return StatusCode(500);
            }
        }

        [HttpDelete("id")]
        public IActionResult Delete(Pool pool)
        {
            try
            {
                var poolRepository = new PoolRepository();
                poolRepository.Delete(pool);
                return Ok();
            }
            catch (Exception e)
            {
                AppSettings.Logger.Error($"Unable to connect to the database at: {AppSettings.Logger}");
                AppSettings.Logger.Error($"The error was: {e.Message}");
                return StatusCode(500);
            }
        }
    }
}