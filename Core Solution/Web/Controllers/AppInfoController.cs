using System;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Repository.Repositories;
using Web.Controllers.Models;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    public class AppInfoController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var appInfoRepository = new AppInfoRepository();
                return Ok(appInfoRepository.GetAppInfo());
            }
            catch (Exception e)
            {
                AppSettings.Logger.Error("Unable to connect to the database.");
                AppSettings.Logger.Error($"The error was: {e.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("sendMessage")]
        public IActionResult SendMessage([FromBody] CustomerMessage customerMessage)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(customerMessage.Name, customerMessage.Email));
                message.ReplyTo.Add(new MailboxAddress(customerMessage.Name, customerMessage.Email));
                message.To.Add(new MailboxAddress("DeathRaffle Support", "datsure@gmail.com"));
                message.Subject = "Message From DeathRaffle.com Customer";
                message.Body = new TextPart("plain")
                {
                    Text = $@"
                        Customer Name: {customerMessage.Name}
                        Customer Email: {customerMessage.Email}
                        {Environment.NewLine}
                        Customer Said: {customerMessage.Message} 
                    "
                };

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 465, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.Authenticate("datsure@gmail.com", "ownlraesbvstthqb");
                    client.Send(message);
                    client.Disconnect(true);
                }

                return Ok();
            }
            catch (Exception e)
            {
                AppSettings.Logger.Error(e.Message);
                return StatusCode(500, "There was a problem on the server.  Please try again later.");
            }
        }
    }
}