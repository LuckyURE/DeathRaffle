using System;
using System.Globalization;
using System.Linq;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Repository.Models;
using Repository.Repositories;
using stellar_dotnetcore_sdk;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    public class CelebrityController : Controller
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var celebrityRepository = new CelebrityRepository();
                return Ok(celebrityRepository.GetAll());
            }
            catch (Exception e)
            {
                AppSettings.Logger.Error($"Unable to connect to the database at: {AppSettings.Logger}");
                AppSettings.Logger.Error($"The error was: {e.Message}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("getLiving")]
        public IActionResult GetLiving()
        {
            try
            {
                var celebrityRepository = new CelebrityRepository();
                return Ok(celebrityRepository.GetLiving());
            }
            catch (Exception e)
            {
                AppSettings.Logger.Error($"Unable to connect to the database at: {AppSettings.Logger}");
                AppSettings.Logger.Error($"The error was: {e.Message}");
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var celebrityRepository = new CelebrityRepository();
                return Ok(celebrityRepository.GetById(id));
            }
            catch (Exception e)
            {
                AppSettings.Logger.Error($"Unable to connect to the database at: {AppSettings.Logger}");
                AppSettings.Logger.Error($"The error was: {e.Message}");
                return StatusCode(500);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] Celebrity celebrity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                var celebrityRepository = new CelebrityRepository();
                var duplicate = celebrityRepository
                    .GetByLastNameAndBirthYear(celebrity.LastName, celebrity.BirthYear);

                if (duplicate == null)
                {
                    return Ok(celebrityRepository.Add(celebrity));
                }

                return BadRequest("Celebrity already exists.");
            }
            catch (Exception e)
            {
                AppSettings.Logger.Error($"Unable to connect to the database at: {AppSettings.Logger}");
                AppSettings.Logger.Error($"The error was: {e.Message}");
                return StatusCode(500);
            }
        }

        [HttpPut]
        [Route("updateCeleb")]
        public IActionResult Update([FromBody] Celebrity celebrity)
        {
            try
            {
                var celebrityRepository = new CelebrityRepository();
                celebrityRepository.Update(celebrity);
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
        [Route("markDead")]
        public IActionResult MarkDead([FromBody] Celebrity celebrity)
        {
            try
            {
                var celebrityRepository = new CelebrityRepository();
                var appInfoRepository = new AppInfoRepository();
                var numLumensToPay = appInfoRepository.GetAppInfo().LumensRequired;
                var winnersToBePaid = celebrityRepository.MarkDead(celebrity).ToList();

                if (!winnersToBePaid.Any()) return Ok();
                
                foreach (var address in winnersToBePaid)
                {
                    SendPayment(
                        address,
                        numLumensToPay.ToString(CultureInfo.InvariantCulture),
                        "You won in DeathRaffle!");
                }

                return Ok();
            }
            catch (Exception e)
            {
                AppSettings.Logger.Error($"Unable to connect to the database at: {AppSettings.Logger}");
                AppSettings.Logger.Error($"The error was: {e.Message}");
                return StatusCode(500);
            }
        }

        [HttpDelete]
        public IActionResult Delete([FromBody] Celebrity celebrity)
        {
            try
            {
                var celebrityRepository = new CelebrityRepository();
                celebrityRepository.Delete(celebrity);
                return Ok();
            }
            catch (Exception e)
            {
                AppSettings.Logger.Error($"Unable to connect to the database at: {AppSettings.Logger}");
                AppSettings.Logger.Error($"The error was: {e.Message}");
                return StatusCode(500);
            }
        }
        
        private static async void SendPayment(string accountId, string amount, string memo)
        {
            Server server;
            var keyPair = KeyPair.FromSecretSeed(AppSettings.WalletPrivateKey);
            
            if (AppSettings.IsProduction)
            {
                AppSettings.Logger.Information("Connected to the public Stellar network.");
                Network.UsePublicNetwork();
                server = new Server("https://horizon.stellar.org");
                Console.WriteLine("Server is: PROD");
            }
            else
            {
                AppSettings.Logger.Information("Connected to the TEST Stellar network.");
                Network.UseTestNetwork();
                server = new Server("https://horizon-testnet.stellar.org");
                Console.WriteLine("Server is: DEV");
            }
            
            var toAccount = KeyPair.FromAccountId(accountId);
            var accountInfo = await server.Accounts.Account(keyPair);
            var fromAccount = new Account(keyPair, accountInfo.SequenceNumber);

            if (toAccount.AccountId == fromAccount.KeyPair.AccountId) return;

            var transaction = new Transaction.Builder(fromAccount)
                .AddOperation(new PaymentOperation.Builder(
                    toAccount,
                    new AssetTypeNative(),
                    amount).Build())
                .AddMemo(Memo.Text(memo))
                .Build();
            transaction.Sign(keyPair);

            try
            {
                var response = await server.SubmitTransaction(transaction);
                AppSettings.Logger.Information($"Payment was sent successfully to {accountId}. Ledger: {response.Ledger}");
            }
            catch (Exception e)
            {
                AppSettings.Logger.Error($"Payment was not sent successfully to {accountId}. Error was: \n {e.Message}");
                try
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("DeathRaffle.com Error Notification", "datsure@gmail.com"));
                    message.To.Add(new MailboxAddress("DeathRaffle Support", "datsure@gmail.com"));
                    message.Subject = "Message From DeathRaffle.com Payment Error Notification!";
                    message.Body = new TextPart("plain")
                    {
                        Text = $@" There was an error sending a winning payment to:
                        Customer Address: {accountId}
                        Amount to Send: {amount}
                        Error was: {e.Message}
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
                }
                catch (Exception ex)
                {
                    var message = $@" There was an error sending a winning payment to:
                        Customer Address: {accountId}
                        Amount to Send: {amount}
                        Error was: {e.Message}";
                    
                    AppSettings.Logger.Error($"Could not email about error: {message} {Environment.NewLine} Error was: {ex.Message}");
                }
            }
        }
    }
}