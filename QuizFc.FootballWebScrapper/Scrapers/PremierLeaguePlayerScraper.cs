using QuizFc.Enums;
using QuizFc.FootballWebScraper.Models;
using QuizFc.FootballWebScrapper.Models;

namespace QuizFc.FootballWebScraper.Scrapers
{
    public class PremierLeaguePlayerScraper : PlayerScraperBase
    {
        private WhoMoreTop5LeagueScraperRequest request = new()
        {
            TopValuableAttackers = "https://www.transfermarkt.pl/premier-league/marktwerte/wettbewerb/GB1/ajax/yw1/pos/Sturm/detailpos//altersklasse/alle/land_id/0/plus//galerie/0/page/",
            TopValuableMidfielders = "https://www.transfermarkt.pl/premier-league/marktwerte/wettbewerb/GB1/pos/Mittelfeld/detailpos//altersklasse/alle/land_id/0/plus//galerie/0/page/",
            TopValuableDefenders = "https://www.transfermarkt.pl/premier-league/marktwerte/wettbewerb/GB1/pos/Abwehr/detailpos//altersklasse/alle/land_id/0/plus//galerie/0/page/",
            TopValuableGoalKeepers = "https://www.transfermarkt.pl/premier-league/marktwerte/wettbewerb/GB1/pos/Torwart/detailpos//altersklasse/alle/land_id/0/plus//galerie/0/page/",
            MaxAttackersPage = 4,
            MaxMidfieldersPage = 4,
            MaxDefendersPage = 4,
            MaxGoalKeepersPage = 3,
            LastTopValuableGolaKeepersPage = 2
        };

        public async Task<Player> GetRandomPlayer(WhoMoreCategory category)
        {
            return await base.GetRandomPlayer(category, request);
        }

        public async Task<Player> GetRandomPlayer(WhoMoreCategory category, Player player)
        {
            return await base.GetRandomPlayer(category, player, request);
        }
    }
}
