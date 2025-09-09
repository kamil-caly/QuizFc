using QuizFc.FootballWebScraper;
using QuizFc.FootballWebScrapper.Models;

for (int i = 0; i < 20; i++)
{
    Thread.Sleep(2000);
    (Player player1, Player player2) = PlayerScraper.Get2PlayersWithDifferendMarketValue();
    Console.WriteLine($"{player1.Name} ({player1.MarketValue}M) vs {player2.Name} ({player2.MarketValue}M)");
}

