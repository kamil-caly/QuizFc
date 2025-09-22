using HtmlAgilityPack;
using QuizFc.Enums;
using QuizFc.FootballWebScrapper.Enums;
using QuizFc.FootballWebScrapper.Models;
using System;
using System.Text.RegularExpressions;

namespace QuizFc.FootballWebScraper.Scrapers
{
    public class PlayerScraperBase
    {
        protected readonly string transfermarktUrlBase = "https://www.transfermarkt.pl";
        protected string GetPlayerUrl(PitchPosition position, string attackers, 
            string midfielders, string defenders, string goalKeepers)
        {
            return position switch
            {
                PitchPosition.Attacker => attackers,
                PitchPosition.Midfielder => midfielders,
                PitchPosition.Defender => defenders,
                PitchPosition.GoalKeeper => goalKeepers,
                _ => throw new NotImplementedException()
            };
        }

        protected int GetRandomPage(PitchPosition position, int maxPageNumber, int? firstStep)
        {
            switch (position)
            {
                case PitchPosition.Attacker:
                case PitchPosition.Midfielder:
                case PitchPosition.Defender:
                    return GetRandomPageNumber(maxPageNumber);
                case PitchPosition.GoalKeeper:
                    return GetRandomPageNumber(maxPageNumber, firstStep);
                default:
                    throw new NotImplementedException();
            }
        }

        virtual async protected Task<Player> GetPlayerFromHtmlNode(HtmlNode randomPlayerTR, WhoMoreCategory category)
        {
            Player player = new Player();
            // Imie i nazwisko
            player.Name = randomPlayerTR.ChildNodes[2].QuerySelector("a").InnerHtml;
            // Zdjęcie twarzy
            player.FaceImgUrl = Regex.Match(randomPlayerTR.ChildNodes[2].QuerySelector("img").OuterHtml, "data-src=\"([^\"]+)\"").Groups[1].Value.Replace("small", "big").Replace("medium", "big");
            // Zdjęcie klubu
            player.ClubImgUrl = randomPlayerTR.ChildNodes[5].QuerySelector("img").GetAttributeValue("src", "").Replace("verysmall", "medium");
            // Zdjęcie narodowości
            player.NationalityImgUrl = randomPlayerTR.ChildNodes[3].QuerySelector("img").GetAttributeValue("src", "").Replace("verysmall", "medium");
            // Wartość rynkowa
            player.MarketValue = decimal.Parse(randomPlayerTR.ChildNodes[6].QuerySelector("a").InnerHtml.Split(" ")[0]);
            // Wiek
            player.Age = int.Parse(randomPlayerTR.ChildNodes[4].InnerHtml);
            
            if (category == WhoMoreCategory.Height)
            {
                // Wzrost
                var playerUrl = transfermarktUrlBase + Regex.Match(randomPlayerTR.ChildNodes[2].QuerySelector("a").OuterHtml, "href=\"([^\"]+)\"").Groups[1].Value;
                HtmlWeb web = new HtmlWeb();
                Thread.Sleep(500); // w celu zapobiegnięcia błędu: 403
                HtmlDocument htmlDoc;
                try
                {
                    htmlDoc = await web.LoadFromWebAsync(playerUrl);
                    player.Height = int.Parse(htmlDoc.QuerySelector("span[itemprop=height]").InnerHtml.Trim().Replace(",", "").Split(" ")[0]);
                }
                catch (Exception)
                {
                    // w przypadku błędu próbujemy ponownie
                    Thread.Sleep(500);
                    try
                    {
                        htmlDoc = await web.LoadFromWebAsync(playerUrl);
                        player.Height = int.Parse(htmlDoc.QuerySelector("span[itemprop=height]").InnerHtml.Trim().Replace(",", "").Split(" ")[0]);
                    }
                    catch (Exception)
                    {
                        player.Height = 0;
                    }
                }
            }

            return player;
        }

        private int GetRandomPageNumber(int maxPageNumber, int? firstStep = null)
        {
            Random random = new Random();

            if (firstStep == null)
            {
                return random.Next(1, maxPageNumber + 1);
            }

            int randomValue = random.Next(1, 101);
            if (randomValue <= 95)
            {
                return random.Next(1, (int)firstStep + 1);
            }
            else
            {
                return random.Next((int)firstStep + 1, maxPageNumber + 1);
            }
        }
    }
}
