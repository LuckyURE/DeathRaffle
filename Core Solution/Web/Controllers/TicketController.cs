using System;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Repository.Repositories;
using Web.Controllers.Models;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    public class TicketController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var ticketRepository = new TicketRepository();
                return Ok(ticketRepository.GetAll());
            }
            catch (Exception e)
            {
                AppSettings.Logger.Error($"Unable to connect to the database at: {AppSettings.Logger}");
                AppSettings.Logger.Error($"The error was: {e.Message}");
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                var ticketRepository = new TicketRepository();
                var ticket = ticketRepository.GetById(id);

                if (ticket == null) return NotFound();
            
                var celebrityRepository = new CelebrityRepository();
                var celebrity = celebrityRepository.GetById(ticket.CelebrityId);

                if (celebrity == null) return NotFound();

                return Ok(new TicketSearchResult
                {
                    TicketId = ticket.Id,
                    PoolId = ticket.PoolId,
                    PlayerAddress = ticket.PlayerAddress,
                    Celebrity = celebrity
                });
            }
            catch (Exception e)
            {
                AppSettings.Logger.Error($"Unable to connect to the database at: {AppSettings.Logger}");
                AppSettings.Logger.Error($"The error was: {e.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] Ticket ticket)
        {
            try
            {
                var ticketRepository = new TicketRepository();
                ticketRepository.Add(ticket);
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
        public IActionResult Put([FromBody] Ticket ticket)
        {
            try
            {
                var ticketRepository = new TicketRepository();
                ticketRepository.Update(ticket);
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
        public IActionResult Delete(int id)
        {
            try
            {
                var ticketRepository = new TicketRepository();
                ticketRepository.Delete(id);
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