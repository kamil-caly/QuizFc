using HtmlAgilityPack;
using QuizFc.Enums;
using QuizFc.FootballWebScrapper.Enums;
using QuizFc.FootballWebScrapper.Models;

namespace QuizFc.FootballWebScraper.Scrapers
{
    public class PremierLeaguePlayerScraper : PlayerScraperBase
    {
        // max 4 pages
        private const string TopValuableAttackers = "https://www.transfermarkt.pl/premier-league/marktwerte/wettbewerb/GB1/ajax/yw1/pos/Sturm/detailpos//altersklasse/alle/land_id/0/plus//galerie/0/page/";
        // max 4 pages
        private const string TopValuableMidfielders = "https://www.transfermarkt.pl/premier-league/marktwerte/wettbewerb/GB1/pos/Mittelfeld/detailpos//altersklasse/alle/land_id/0/plus//galerie/0/page/";
        // max 4 pages
        private const string TopValuableDefenders = "https://www.transfermarkt.pl/premier-league/marktwerte/wettbewerb/GB1/pos/Abwehr/detailpos//altersklasse/alle/land_id/0/plus//galerie/0/page/";
        // max 3 pages but 2 and 3 page players are not so valuable (we have to decrease chance to get player from these pages)
        private const string TopValuableGoalKeepers = "https://www.transfermarkt.pl/premier-league/marktwerte/wettbewerb/GB1/pos/Torwart/detailpos//altersklasse/alle/land_id/0/plus//galerie/0/page/";

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
                    position == PitchPosition.GoalKeeper ? 2 : null
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
    }
}
