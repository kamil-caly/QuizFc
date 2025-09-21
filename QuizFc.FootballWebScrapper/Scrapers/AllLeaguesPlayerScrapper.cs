using HtmlAgilityPack;
using QuizFc.Enums;
using QuizFc.FootballWebScrapper.Enums;
using QuizFc.FootballWebScrapper.Models;
using System.Text.RegularExpressions;

namespace QuizFc.FootballWebScraper.Scrapers
{
    public class AllLeaguesPlayerScrapper : PlayerScraperBase
    {
        private const int MaxPageNumber = 20;

        // Najbardziej wartościowi napastnicy występują do strony nr 6
        private const string TopValuableAttackers = "https://www.transfermarkt.pl/spieler-statistik/wertvollstespieler/marktwertetop/plus/ajax/yw1/ausrichtung/Sturm/spielerposition_id/alle/altersklasse/alle/jahrgang/0/land_id/0/kontinent_id/0/yt0/Poka%C5%BC/0//page/";
        // Najbardziej wartościowi pomocnicy występują do strony nr 6
        private const string TopValuableMidfielders = "https://www.transfermarkt.pl/spieler-statistik/wertvollstespieler/marktwertetop/plus/ausrichtung/Mittelfeld/spielerposition_id/alle/altersklasse/alle/jahrgang/0/land_id/0/kontinent_id/0/yt0/Poka%C5%BC/0//page/";
        // Najbardziej wartościowi obrońcy występują do strony nr 6
        private const string TopValuableDefenders = "https://www.transfermarkt.pl/spieler-statistik/wertvollstespieler/marktwertetop/plus/ajax/yw1/ausrichtung/Abwehr/spielerposition_id/alle/altersklasse/alle/jahrgang/0/land_id/0/kontinent_id/0/yt0/Poka%C5%BC/0//page/";
        // Najbardziej wartościowi bramkarze występują do strony nr 3
        private const string TopValuableGoalKeepers = "https://www.transfermarkt.pl/spieler-statistik/wertvollstespieler/marktwertetop/plus/ajax/yw1/ausrichtung/Torwart/spielerposition_id/alle/altersklasse/alle/jahrgang/0/land_id/0/kontinent_id/0/yt0/Poka%C5%BC/0//page/";

        public async Task<Player> GetRandomPlayer(WhoMoreCategory category)
        {
            Random random = new Random();
            // losujemy pozycję na boisku
            PitchPosition position = (PitchPosition)random.Next(0, 4);
            // tworzymy url do losowej strony z zawodnikami na wylosowanej pozycji
            string url = base.GetPlayerUrl(position, TopValuableAttackers, TopValuableMidfielders,
                TopValuableDefenders, TopValuableGoalKeepers);
            // dodajemy losowy numer strony (większa szansa na wartościowszych zawodników)
            int pageNumber = GetRandomPage(position);
            url += pageNumber.ToString();

            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = await web.LoadFromWebAsync(url);

            // losujemy zawodnika ze strony
            int playersCount = htmlDoc.QuerySelectorAll("div#yw1 table.items > tbody > tr").Count();
            int playerIdx = random.Next(0, playersCount);
            var randomPlayerTR = htmlDoc.QuerySelectorAll("div#yw1 table.items > tbody > tr")[playerIdx];

            return await GetPlayerFromHtmlNode(randomPlayerTR, category);
        }

        private static int GetRandomPage(PitchPosition position)
        {
            switch (position)
            {
                case PitchPosition.Attacker:
                case PitchPosition.Midfielder:
                case PitchPosition.Defender:
                    return GetRandomPageNumber(6, 10);
                case PitchPosition.GoalKeeper:
                    return GetRandomPageNumber(3, 6);
                default:
                    throw new NotImplementedException();
            }
        }

        private static int GetRandomPageNumber(int firstStep, int secondStep)
        {
            Random random = new Random();

            // 70% szans na stronę 1-{firstStep},
            // 20% szans na stronę {firstStep+1}-{secondStep},
            // 10% szans na stronę {secondStep+1}-{MaxPageNumber}
            int randomValue = random.Next(1, 11);
            if (randomValue <= 7)
            {
                return random.Next(1, firstStep + 1);
            }
            else if (randomValue <= 9)
            {
                return random.Next(firstStep + 1, secondStep + 1);
            }
            else
            {
                return random.Next(secondStep + 1, MaxPageNumber);
            }
        }

        override async protected Task<Player> GetPlayerFromHtmlNode(HtmlNode randomPlayerTR, WhoMoreCategory category)
        {
            Player player = new Player();
            // Imie i nazwisko
            player.Name = Regex.Match(randomPlayerTR.ChildNodes[2].QuerySelector("a").InnerHtml, "title=\"([^\"]+)\"").Groups[1].Value;
            // Zdjęcie twarzy
            player.FaceImgUrl = Regex.Match(randomPlayerTR.ChildNodes[2].QuerySelector("img").OuterHtml, "src=\"([^\"]+)\"").Groups[1].Value.Replace("small", "big");
            // Zdjęcie klubu
            player.ClubImgUrl = randomPlayerTR.ChildNodes[5].QuerySelector("img").GetAttributeValue("src", "").Replace("verysmall", "medium");
            // Zdjęcie narodowości
            player.NationalityImgUrl = randomPlayerTR.ChildNodes[4].QuerySelector("img").GetAttributeValue("src", "").Replace("verysmall", "medium");
            // Wartość rynkowa
            player.MarketValue = decimal.Parse(randomPlayerTR.ChildNodes[6].QuerySelector("a").InnerHtml.Split(" ")[0]);

            return player;
        }
    }
}
