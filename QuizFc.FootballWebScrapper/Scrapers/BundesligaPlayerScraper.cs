using HtmlAgilityPack;
using QuizFc.Enums;
using QuizFc.FootballWebScrapper.Enums;
using QuizFc.FootballWebScrapper.Models;
using System.Text.RegularExpressions;

namespace QuizFc.FootballWebScraper.Scrapers
{
    public class BundesligaPlayerScraper : PlayerScraperBase
    {
        // max 4 pages
        private const string TopValuableAttackers = "https://www.transfermarkt.pl/bundesliga/marktwerte/wettbewerb/L1/ajax/yw1/pos/Sturm/detailpos//altersklasse/alle/land_id/0/plus//galerie/0/page/";
        // max 4 pages
        private const string TopValuableMidfielders = "https://www.transfermarkt.pl/bundesliga/marktwerte/wettbewerb/L1/ajax/yw1/pos/Mittelfeld/detailpos//altersklasse/alle/land_id/0/plus//galerie/0/page/";
        // max 4 pages
        private const string TopValuableDefenders = "https://www.transfermarkt.pl/bundesliga/marktwerte/wettbewerb/L1/ajax/yw1/pos/Abwehr/detailpos//altersklasse/alle/land_id/0/plus//galerie/0/page/";
        // max 3 pages
        private const string TopValuableGoalKeepers = "https://www.transfermarkt.pl/bundesliga/marktwerte/wettbewerb/L1/ajax/yw1/pos/Torwart/detailpos//altersklasse/alle/land_id/0/plus//galerie/0/page/";

        public async Task<Player> GetRandomPlayer(WhoMoreCategory category)
        {
            Random random = new Random();
            // losujemy pozycję na boisku
            PitchPosition position = (PitchPosition)random.Next(0, 4);
            // tworzymy url do losowej strony z zawodnikami na wylosowanej pozycji
            string url = base.GetPlayerUrl(position, TopValuableAttackers, 
                TopValuableMidfielders, TopValuableDefenders, TopValuableGoalKeepers);
            // dodajemy losowy numer strony
            int pageNumber = 
                GetRandomPage(
                    position, 
                    position == PitchPosition.GoalKeeper ? 3 : 4,
                    position == PitchPosition.GoalKeeper ? 1 : null
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

        public async Task<Player> GetRandomPlayer(WhoMoreCategory category, Player player)
        {
            Random random = new Random();
            while (true)
            {
                // losujemy pozycję na boisku
                PitchPosition position = (PitchPosition)random.Next(0, 4);
                // tworzymy url do losowej strony z zawodnikami na wylosowanej pozycji
                string url = base.GetPlayerUrl(position, TopValuableAttackers, TopValuableMidfielders,
                    TopValuableDefenders, TopValuableGoalKeepers);
                // dodajemy losowy numer strony
                int pageNumber =
                    GetRandomPage(
                        position,
                        position == PitchPosition.GoalKeeper ? 3 : 4,
                        position == PitchPosition.GoalKeeper ? 2 : null
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
    }
}
