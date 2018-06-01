using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Repository;
using Repository.Repositories;

namespace CelebScraper
{
    internal static class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static void Main()
        {
            DatabaseUtils.ConfigureDatabase();
            var celebrityRepository = new CelebrityRepository();
            //var celebs = celebrityRepository.GetAll().Reverse();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
            var celebs = File.ReadLines("/home/joshua/Desktop/DeathRaffle/not_found.txt");
            Console.WriteLine(celebs);
            

            foreach (var celeb in celebs)
            {
                if(string.IsNullOrEmpty(celeb)) continue;
                var item = celeb.Trim();
                var stringTask = client.GetStringAsync($"https://en.wikipedia.org/w/api.php?action=query&prop=revisions&rvprop=content&rvsection=0&titles={item}");

                var msg = stringTask.Result;
                Console.WriteLine(msg);

                if (msg.Contains("death_date={{") || msg.Contains("{{Death date and"))
                {
                    var split = item.Split(" ");
                    var entity = celebrityRepository
                        .GetAll()
                        .Where(p => p.LastName == split[1] && p.FirstName == split[0])
                        .Select(p => p)
                        .First();

                    entity.Dead = true;
                    celebrityRepository.Update(entity);
                    Console.WriteLine($"{celeb} has been marked dead");
                }
                else if (msg.Contains("\"-1\":"))
                {
                    using(var writetext = new StreamWriter("/home/joshua/Desktop/DeathRaffle/not_found2.txt", true))
                    {
                        writetext.WriteLine($"{celeb}");
                        writetext.WriteLine();
                        Console.WriteLine($"No record found for ID: {celeb}");
                    }
                }
                else if (msg.Contains("|death_date  ") || msg.Contains("|death_date =\\n|"))
                {
                    Console.WriteLine($"{celeb} is ALIVE!");
                }
                else if (msg.Contains("{{Birth date") && !msg.Contains("death"))
                {
                    Console.WriteLine($"{celeb} is ALIVE!");
                }
                else if (msg.Contains("| birth_date ") && !msg.Contains("death_date={{"))
                {
                    Console.WriteLine($"{celeb} is ALIVE!");
                }
                else
                {
                    using(var writetext = new StreamWriter("/home/joshua/Desktop/DeathRaffle/not_found2.txt", true))
                    {
                        writetext.WriteLine($"{celeb}");
                        writetext.WriteLine();
                        Console.WriteLine($"Something else happened!?: {celeb}");
                    }
                    
                }
               
                
//                var b = new BrowserSession();
//                b.Get("http://www.deadoraliveinfo.com/dead.nsf/Search?OpenForm&x=nf");
//                b.FormElements["Query"] = $"{celeb.FirstName} {celeb.LastName}";
//                b.FormElements["Sex"] = "b";
//                var response = b.Post("http://www.deadoraliveinfo.com/dead.nsf/44ed9b7d682bef2e8525706a0061508d!CreateDocument");
//                if (response.Contains("No documents match your search criteria"))
//                {
//                    Console.WriteLine($"No record found for ID: {celeb.Id} - {celeb.FirstName} {celeb.LastName}");
//                    using(var writetext = new StreamWriter("/var/www/not_found.txt", true))
//                    {
//                        writetext.WriteLine($"No record found for ID: {celeb.Id} - {celeb.FirstName} {celeb.LastName}");
//                        writetext.WriteLine();
//                    }
//                }
//                else if (response.Contains("<img src=\"/images/alive-record.gif\""))
//                {
//                    Console.WriteLine($"{celeb.FirstName} {celeb.LastName} is alive!");
//                    celeb.Dead = false;
//                    celebrityRepository.Update(celeb);
//                }
//                else if (response.Contains("<img src=\"/images/dead-record.gif\""))
//                {
//                    Console.WriteLine($"{celeb.FirstName} {celeb.LastName} is DEAD!");
//                    celeb.Dead = true;
//                    celebrityRepository.Update(celeb);
//                }
//                else
//                {
//                    Console.WriteLine(response);
//                }
//
//                Console.WriteLine("");

//                var doc = new HtmlDocument();
//                try
//                {
//                    var web = new HtmlWeb();
//                    doc = web.Load(url);
//                }
//                catch (Exception e)
//                {
//                    Console.WriteLine("Web call timed out, continuing anyway.");
//                }


//                try
//                {
//                    var deadNode = doc.DocumentNode
//                        .Descendants("div")
//                        .SingleOrDefault(d => d.Attributes.Contains("class")
//                                              && d.Attributes["class"].Value.Contains("row main-stats"));
//
//                    var isDead = deadNode != null && deadNode.InnerText.Contains("DEATH DATE");
//
//                    if (deadNode != null && isDead == false)
//                    {
//                        var birthNode = doc.DocumentNode
//                            .Descendants("div")
//                            .First(d => d.Attributes.Contains("class")
//                                        && d.Attributes["class"].Value.Contains("stat box"));
//
//                        var birthFrom = birthNode.InnerText.IndexOf(",", StringComparison.Ordinal) + 2;
//                        var birthTo = birthNode.InnerText.IndexOf(",", StringComparison.Ordinal) + 6;
//                        var birthText = birthNode.InnerText.Substring(birthFrom, birthTo - birthFrom);
//
//                        var aboutNode = doc.DocumentNode
//                            .Descendants("div")
//                            .Single(d => d.Attributes.Contains("class")
//                                         && d.Attributes["class"].Value.Contains("bio col-sm-7 col-md-8 col-lg-6"));
//
//                        var aboutFrom = aboutNode.InnerText.IndexOf(".html\">", StringComparison.Ordinal) +
//                                        ".html\">".Length;
//                        var aboutTo = aboutNode.InnerText.IndexOf("Before Fame", StringComparison.Ordinal);
//                        var aboutText = aboutNode.InnerText.Substring(aboutFrom, aboutTo - aboutFrom)
//                            .Replace("\n", string.Empty);
//
//                        var humanName = new FullNameParser(celeb.Name);
//                        humanName.Parse();
//
//                        celebrityRepository.Add(new Celebrity()
//                        {
//                            BirthYear = int.Parse(birthText),
//                            Dead = false,
//                            FirstName = humanName.FirstName,
//                            LastName = humanName.LastName,
//                            MiddleName = humanName.MiddleName,
//                            Suffix = humanName.Suffix,
//                            Country = celeb.Country.ToUpper(),
//                            Description = aboutText
//                        });
//                        Console.WriteLine($"Added {celeb.Name}");
//                    }
//                }
//                catch
//                {
//                    Console.WriteLine($"{celeb.Name} NOT FOUND");
//                }
            }
        }
    }
}