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
        private static readonly SerieAPlayerScraper serieAPlayerScraper = default!;
        private static readonly LaLigaPlayerScraper laLigaPlayerScraper = default!;
        private static readonly BundesligaPlayerScraper bundesligaPlayerScraper = default!;
        private static readonly Ligue1PlayerScraper ligue1PlayerScraper = default!;
        static PlayerScraper()
        {
            allLeaguesPlayerScrapper = new AllLeaguesPlayerScrapper();
            premierLeaguePlayerScraper = new PremierLeaguePlayerScraper();
            serieAPlayerScraper = new SerieAPlayerScraper();
            laLigaPlayerScraper = new LaLigaPlayerScraper();
            bundesligaPlayerScraper = new BundesligaPlayerScraper();
            ligue1PlayerScraper = new Ligue1PlayerScraper();
        }
        public async static Task<(Player, Player)> Get2PlayersWithDifferendCategoryValue(League league, WhoMoreCategory category)
        {
            Player player1;
            Player player2;

            switch (league)
            {
                case League.All:
                    player1 = await allLeaguesPlayerScrapper.GetRandomPlayer(category);
                    player2 = await allLeaguesPlayerScrapper.GetRandomPlayer(category, player1);
                    break;
                case League.PremierLeague:
                    player1 = await premierLeaguePlayerScraper.GetRandomPlayer(category);
                    player2 = await premierLeaguePlayerScraper.GetRandomPlayer(category, player1);
                    break;
                case League.SerieA:
                    player1 = await serieAPlayerScraper.GetRandomPlayer(category);
                    player2 = await serieAPlayerScraper.GetRandomPlayer(category, player1);
                    break;
                case League.LaLiga:
                    player1 = await laLigaPlayerScraper.GetRandomPlayer(category);
                    player2 = await laLigaPlayerScraper.GetRandomPlayer(category, player1);
                    break;
                case League.Bundesliga:
                    player1 = await bundesligaPlayerScraper.GetRandomPlayer(category);
                    player2 = await bundesligaPlayerScraper.GetRandomPlayer(category, player1);
                    break;
                case League.Ligue1:
                    player1 = await ligue1PlayerScraper.GetRandomPlayer(category);
                    player2 = await ligue1PlayerScraper.GetRandomPlayer(category, player1);
                    break;
                default:
                    player1 = await allLeaguesPlayerScrapper.GetRandomPlayer(category);
                    player2 = await allLeaguesPlayerScrapper.GetRandomPlayer(category, player1);
                    break;
            }

            return (player1, player2);
        }
    }
}
