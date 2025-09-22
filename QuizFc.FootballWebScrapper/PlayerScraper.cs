using HtmlAgilityPack;
using QuizFc.Enums;
using QuizFc.FootballWebScraper.Scrapers;
using QuizFc.FootballWebScrapper.Enums;
using QuizFc.FootballWebScrapper.Models;
using System.Text.RegularExpressions;

namespace QuizFc.FootballWebScraper
{
    public static class PlayerScraper
    {
        private static readonly AllLeaguesPlayerScrapper allLeaguesPlayerScrapper = default!;
        private static readonly PremierLeaguePlayerScraper premierLeaguePlayerScraper = default!;
        static PlayerScraper()
        {
            allLeaguesPlayerScrapper = new AllLeaguesPlayerScrapper();
            premierLeaguePlayerScraper = new PremierLeaguePlayerScraper();
        }

        //private const int MaxPageNumber = 20;

        //// Najbardziej wartościowi napastnicy występują do strony nr 6
        //private const string TopValuableAttackers = "https://www.transfermarkt.pl/spieler-statistik/wertvollstespieler/marktwertetop/plus/ajax/yw1/ausrichtung/Sturm/spielerposition_id/alle/altersklasse/alle/jahrgang/0/land_id/0/kontinent_id/0/yt0/Poka%C5%BC/0//page/";
        //// Najbardziej wartościowi pomocnicy występują do strony nr 6
        //private const string TopValuableMidfielders = "https://www.transfermarkt.pl/spieler-statistik/wertvollstespieler/marktwertetop/plus/ausrichtung/Mittelfeld/spielerposition_id/alle/altersklasse/alle/jahrgang/0/land_id/0/kontinent_id/0/yt0/Poka%C5%BC/0//page/";
        //// Najbardziej wartościowi obrońcy występują do strony nr 6
        //private const string TopValuableDefenders = "https://www.transfermarkt.pl/spieler-statistik/wertvollstespieler/marktwertetop/plus/ajax/yw1/ausrichtung/Abwehr/spielerposition_id/alle/altersklasse/alle/jahrgang/0/land_id/0/kontinent_id/0/yt0/Poka%C5%BC/0//page/";
        //// Najbardziej wartościowi bramkarze występują do strony nr 3
        //private const string TopValuableGoalKeepers = "https://www.transfermarkt.pl/spieler-statistik/wertvollstespieler/marktwertetop/plus/ajax/yw1/ausrichtung/Torwart/spielerposition_id/alle/altersklasse/alle/jahrgang/0/land_id/0/kontinent_id/0/yt0/Poka%C5%BC/0//page/";


        //private static string GetRandomPlayerUrl(PitchPosition position)
        //{
        //    return position switch
        //    {
        //        PitchPosition.Attacker => TopValuableAttackers,
        //        PitchPosition.Midfielder => TopValuableMidfielders,
        //        PitchPosition.Defender => TopValuableDefenders,
        //        PitchPosition.GoalKeeper => TopValuableGoalKeepers,
        //        _ => throw new NotImplementedException()
        //    };
        //}

        //private static int GetRandomPageNumber(int firstStep, int secondStep)
        //{
        //    Random random = new Random();

        //    // 70% szans na stronę 1-{firstStep},
        //    // 20% szans na stronę {firstStep+1}-{secondStep},
        //    // 10% szans na stronę {secondStep+1}-{MaxPageNumber}
        //    int randomValue = random.Next(1, 11);
        //    if (randomValue <= 7)
        //    {
        //        return random.Next(1, firstStep + 1);
        //    }
        //    else if (randomValue <= 9)
        //    {
        //        return random.Next(firstStep + 1, secondStep + 1);
        //    }
        //    else
        //    {
        //        return random.Next(secondStep + 1, MaxPageNumber);
        //    }
        //}
        //private static int GetRandomPage(PitchPosition position)
        //{
        //    switch (position)
        //    {
        //        case PitchPosition.Attacker:
        //        case PitchPosition.Midfielder:
        //        case PitchPosition.Defender:
        //            return GetRandomPageNumber(6, 10);
        //        case PitchPosition.GoalKeeper:
        //            return GetRandomPageNumber(3, 6);
        //        default:
        //            throw new NotImplementedException();
        //    }
        //}
        //private static Player GetPlayerFromHtmlNode(HtmlNode randomPlayerTR)
        //{
        //    Player player = new Player();
        //    // Imie i nazwisko
        //    player.Name = Regex.Match(randomPlayerTR.ChildNodes[2].QuerySelector("a").InnerHtml, "title=\"([^\"]+)\"").Groups[1].Value;
        //    // Zdjęcie twarzy
        //    player.FaceImgUrl = Regex.Match(randomPlayerTR.ChildNodes[2].QuerySelector("img").OuterHtml, "src=\"([^\"]+)\"").Groups[1].Value.Replace("small", "big");
        //    // Zdjęcie klubu
        //    player.ClubImgUrl = randomPlayerTR.ChildNodes[5].QuerySelector("img").GetAttributeValue("src", "").Replace("verysmall", "medium");
        //    // Zdjęcie narodowości
        //    player.NationalityImgUrl = randomPlayerTR.ChildNodes[4].QuerySelector("img").GetAttributeValue("src", "").Replace("verysmall", "medium");
        //    // Wartość rynkowa
        //    player.MarketValue = decimal.Parse(randomPlayerTR.ChildNodes[6].QuerySelector("a").InnerHtml.Split(" ")[0]);

        //    return player;
        //}
        //private async static Task<Player> GetRandomPlayer(League league, WhoMoreCategory category)
        //{
        //    Random random = new Random();
        //    // losujemy pozycję na boisku
        //    PitchPosition position = (PitchPosition)random.Next(0, 4);
        //    // tworzymy url do losowej strony z zawodnikami na wylosowanej pozycji
        //    string url = GetRandomPlayerUrl(position);
        //    // dodajemy losowy numer strony (większa szansa na wartościowszych zawodników)
        //    int pageNumber = GetRandomPage(position);
        //    url += pageNumber.ToString();

        //    HtmlWeb web = new HtmlWeb();
        //    HtmlDocument htmlDoc = await web.LoadFromWebAsync(url);

        //    // losujemy zawodnika ze strony
        //    int playersCount = htmlDoc.QuerySelectorAll("table.items > tbody > tr").Count();
        //    int playerIdx = random.Next(0, playersCount);
        //    var randomPlayerTR = htmlDoc.QuerySelectorAll("table.items > tbody > tr")[playerIdx];

        //    return GetPlayerFromHtmlNode(randomPlayerTR);
        //}

        //private async static Task<Player> GetRandomPlayer(decimal blockedMarketValue)
        //{
        //    Random random = new Random();

        //    while (true)
        //    {
        //        // losujemy pozycję na boisku
        //        PitchPosition position = (PitchPosition)random.Next(0, 4);
        //        // tworzymy url do losowej strony z zawodnikami na wylosowanej pozycji
        //        string url = GetRandomPlayerUrl(position);
        //        // dodajemy losowy numer strony (większa szansa na wartościowszych zawodników)
        //        int pageNumber = GetRandomPage(position);
        //        url += pageNumber.ToString();

        //        HtmlWeb web = new HtmlWeb();
        //        HtmlDocument htmlDoc = await web.LoadFromWebAsync(url);

        //        // pobieramy wszystkich zawodników z danej strony
        //        List<HtmlNode> randomPlayerTRs = htmlDoc.QuerySelectorAll("table.items > tbody > tr").ToList();
        //        // Robymy słownik <indeks, wartość rynkowa> z zawodników z danej strony dla wartości różnych od blockedMarketValue
        //        Dictionary<int, decimal> playersWithMarketValue = new Dictionary<int, decimal>();
        //        for (int i = 0; i < randomPlayerTRs.Count; i++)
        //        {
        //            decimal marketValue = decimal.Parse(randomPlayerTRs[i].ChildNodes[6].QuerySelector("a").InnerHtml.Split(" ")[0]);
        //            if (marketValue != blockedMarketValue)
        //            {
        //                playersWithMarketValue.Add(i, marketValue);
        //            }
        //        }

        //        // jeżeli żaden z zawodników nie ma innej wartości rynkowej niż blockedMarketValue to 'continue'
        //        if (playersWithMarketValue.Count == 0) continue;

        //        // losujemy zawodnika ze strony, który ma inną wartość rynkową niż blockedMarketValue
        //        HtmlNode htmlPlayerTR = randomPlayerTRs[playersWithMarketValue.ElementAt(random.Next(0, playersWithMarketValue.Count)).Key];
        //        return GetPlayerFromHtmlNode(htmlPlayerTR);
        //    }
        //}
        public async static Task<(Player, Player)> Get2PlayersWithDifferendCategoryValue(League league, WhoMoreCategory category)
        {
            Player player1;
            Player player2 = new Player() { Name = "Test" };

            switch (league)
            {
                case League.All:
                    player1 = await allLeaguesPlayerScrapper.GetRandomPlayer(category);
                    player2 = await allLeaguesPlayerScrapper.GetRandomPlayer(category, player1);
                    break;
                case League.PremierLeague:
                    player1 = await premierLeaguePlayerScraper.GetRandomPlayer(category);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return (player1, player2);
        }
    }
}
