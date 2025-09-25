using HtmlAgilityPack;
using QuizFc.Enums;
using QuizFc.FootballWebScraper.Models;
using QuizFc.FootballWebScrapper.Enums;
using QuizFc.FootballWebScrapper.Models;
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

        protected int GetRandomPage(PitchPosition position, int maxPageNumber, int? lastTopValuablePage)
        {
            switch (position)
            {
                case PitchPosition.Attacker:
                case PitchPosition.Midfielder:
                case PitchPosition.Defender:
                    return GetRandomPageNumber(maxPageNumber, lastTopValuablePage);
                case PitchPosition.GoalKeeper:
                    return GetRandomPageNumber(maxPageNumber, lastTopValuablePage);
                default:
                    throw new NotImplementedException();
            }
        }

        public async Task<Player> GetRandomPlayer(WhoMoreCategory category, WhoMoreTop5LeagueScraperRequest request)
        {
            Random random = new Random();
            // losujemy pozycję na boisku
            PitchPosition position = (PitchPosition)random.Next(0, 4);
            // tworzymy url do losowej strony z zawodnikami na wylosowanej pozycji
            string url = GetPlayerUrl(position, request.TopValuableAttackers,
                request.TopValuableMidfielders, request.TopValuableDefenders, request.TopValuableGoalKeepers);

            int maxPageNumber = 0;
            switch (position)
            {
                case PitchPosition.Attacker:
                    maxPageNumber = request.MaxAttackersPage;
                    break;
                case PitchPosition.Midfielder:
                    maxPageNumber = request.MaxMidfieldersPage;
                    break;
                case PitchPosition.Defender:
                    maxPageNumber = request.MaxDefendersPage;
                    break;
                case PitchPosition.GoalKeeper:
                    maxPageNumber = request.MaxGoalKeepersPage;
                    break;
            }

            // dodajemy losowy numer strony
            int pageNumber =
                GetRandomPage(
                    position,
                    maxPageNumber,
                    position == PitchPosition.GoalKeeper ? request.LastTopValuableGolaKeepersPage : null
                );

            url += pageNumber.ToString();

            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = await web.LoadFromWebAsync(url);

            // losujemy zawodnika ze strony
            int playersCount = htmlDoc.QuerySelectorAll("div#yw1 table.items > tbody > tr").Count();
            int playerIdx = random.Next(0, playersCount);
            var randomPlayerTR = htmlDoc.QuerySelectorAll("div#yw1 table.items > tbody > tr")[playerIdx];

            return await GetPlayerFromHtmlNode(randomPlayerTR, category);
        }

        public async Task<Player> GetRandomPlayer(WhoMoreCategory category, Player player, WhoMoreTop5LeagueScraperRequest request)
        {
            Random random = new Random();
            while (true)
            {
                // losujemy pozycję na boisku
                PitchPosition position = (PitchPosition)random.Next(0, 4);
                // tworzymy url do losowej strony z zawodnikami na wylosowanej pozycji
                string url = GetPlayerUrl(position, request.TopValuableAttackers, request.TopValuableMidfielders,
                    request.TopValuableDefenders, request.TopValuableGoalKeepers);

                int maxPageNumber = 0;
                switch (position)
                {
                    case PitchPosition.Attacker:
                        maxPageNumber = request.MaxAttackersPage;
                        break;
                    case PitchPosition.Midfielder:
                        maxPageNumber = request.MaxMidfieldersPage;
                        break;
                    case PitchPosition.Defender:
                        maxPageNumber = request.MaxDefendersPage;
                        break;
                    case PitchPosition.GoalKeeper:
                        maxPageNumber = request.MaxGoalKeepersPage;
                        break;
                }

                // dodajemy losowy numer strony
                int pageNumber =
                    GetRandomPage(
                        position,
                        maxPageNumber,
                        position == PitchPosition.GoalKeeper ? request.LastTopValuableGolaKeepersPage : null
                    );

                url += pageNumber.ToString();

                HtmlWeb web = new HtmlWeb();
                HtmlDocument htmlDoc = await web.LoadFromWebAsync(url);

                // pobieramy wszystkich zawodników z danej strony
                List<HtmlNode> randomPlayerTRs = htmlDoc.QuerySelectorAll("div#yw1 table.items > tbody > tr").ToList();

                if (category == WhoMoreCategory.MarketValue)
                {
                    // Robymy słownik <indeks, wartość rynkowa> z zawodników z danej strony dla wartości różnych od blockedMarketValue
                    Dictionary<int, decimal> playersWithMarketValue = new Dictionary<int, decimal>();
                    for (int i = 0; i < randomPlayerTRs.Count; i++)
                    {
                        decimal marketValue = decimal.Parse(randomPlayerTRs[i].ChildNodes[6].QuerySelector("a").InnerHtml.Split(" ")[0]);
                        if (marketValue != player.MarketValue)
                        {
                            playersWithMarketValue.Add(i, marketValue);
                        }
                    }

                    // jeżeli żaden z zawodników nie ma innej wartości rynkowej niż blockedMarketValue to 'continue'
                    if (playersWithMarketValue.Count == 0) continue;

                    // losujemy zawodnika ze strony, który ma inną wartość rynkową niż blockedMarketValue
                    HtmlNode htmlPlayerTR = randomPlayerTRs[playersWithMarketValue.ElementAt(random.Next(0, playersWithMarketValue.Count)).Key];
                    return await GetPlayerFromHtmlNode(htmlPlayerTR, category);
                }
                else if (category == WhoMoreCategory.Age)
                {
                    // Robymy słownik <indeks, wiek> z zawodników z danej strony dla wartości różnych od wieku player.Age
                    Dictionary<int, int> playersWithAge = new Dictionary<int, int>();
                    for (int i = 0; i < randomPlayerTRs.Count; i++)
                    {
                        int age = int.Parse(randomPlayerTRs[i].ChildNodes[4].InnerHtml);
                        if (age != player.Age)
                        {
                            playersWithAge.Add(i, age);
                        }
                    }

                    // jeżeli żaden z zawodników nie ma innego wieku niż player.Age to 'continue'
                    if (playersWithAge.Count == 0) continue;

                    // losujemy zawodnika ze strony, który ma inny wiek niż player.Age
                    HtmlNode htmlPlayerTR = randomPlayerTRs[playersWithAge.ElementAt(random.Next(0, playersWithAge.Count)).Key];
                    return await GetPlayerFromHtmlNode(htmlPlayerTR, category);
                }
                else
                {
                    // Losujemy zawodnika ze strony
                    int playerIdx = random.Next(0, randomPlayerTRs.Count);
                    // sprawdzamy czy jego wzrost jest różny od wzrostu player.Height
                    var playerUrl = transfermarktUrlBase + Regex.Match(randomPlayerTRs[playerIdx].ChildNodes[2].QuerySelector("a").OuterHtml, "href=\"([^\"]+)\"").Groups[1].Value;
                    web = new HtmlWeb();
                    Thread.Sleep(500); // w celu zapobiegnięcia błędu: 403
                    int height = 0;
                    try
                    {
                        htmlDoc = await web.LoadFromWebAsync(playerUrl);
                        height = int.Parse(htmlDoc.QuerySelector("span[itemprop=height]").InnerHtml.Trim().Replace(",", "").Split(" ")[0]);
                    }
                    catch (Exception)
                    {
                        // w przypadku błędu próbujemy ponownie
                        Thread.Sleep(500);
                        try
                        {
                            htmlDoc = await web.LoadFromWebAsync(playerUrl);
                            height = int.Parse(htmlDoc.QuerySelector("span[itemprop=height]").InnerHtml.Trim().Replace(",", "").Split(" ")[0]);
                        }
                        catch (Exception)
                        {
                            height = 0;
                        }
                    }

                    // jeżeli wzrost jest różny to zwracamy zawodnika
                    if (height != player.Height || (height == 0 && player.Height == 0))
                    {
                        return await GetPlayerFromHtmlNode(randomPlayerTRs[playerIdx], category);
                    }
                }
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
            string marketValueTxt = randomPlayerTR.ChildNodes[6].QuerySelector("a").InnerHtml;
            if (marketValueTxt.Contains("tys"))
            {
                player.MarketValue = decimal.Parse("0," + marketValueTxt.Split(" ")[0]);
            }
            else
            {
                player.MarketValue = decimal.Parse(marketValueTxt.Split(" ")[0]);
            }
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

        private int GetRandomPageNumber(int maxPageNumber, int? lastTopValuablePage = null)
        {
            Random random = new Random();

            if (lastTopValuablePage == null)
            {
                return random.Next(1, maxPageNumber + 1);
            }

            int randomValue = random.Next(1, 101);
            if (randomValue <= 95)
            {
                return random.Next(1, (int)lastTopValuablePage + 1);
            }
            else
            {
                return random.Next((int)lastTopValuablePage + 1, maxPageNumber + 1);
            }
        }
    }
}
