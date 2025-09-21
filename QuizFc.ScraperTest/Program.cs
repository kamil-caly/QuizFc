using QuizFc.Enums;
using QuizFc.FootballWebScraper;
using QuizFc.FootballWebScrapper.Models;

for (int i = 0; i < 20; i++)
{
    Thread.Sleep(10000);
    (Player player1, Player player2) = await PlayerScraper.Get2PlayersWithDifferendCategoryValue(League.PremierLeague, WhoMoreCategory.Height);
    //Console.WriteLine($"{player1.Name} ({player1.MarketValue}M) vs {player2.Name} ({player2.MarketValue}M)");
    // wypisz w konsoli wszystkie atrybuty player 1 w kolejnych liniach
    Console.WriteLine("Player 1:");
    Console.WriteLine($"Name: {player1.Name}");
    Console.WriteLine($"FaceImgUrl: {player1.FaceImgUrl}");
    Console.WriteLine($"ClubImgUrl: {player1.ClubImgUrl}");
    Console.WriteLine($"NationalityImgUrl: {player1.NationalityImgUrl}");
    Console.WriteLine($"MarketValue: {player1.MarketValue}M");
    Console.WriteLine($"Age: {player1.Age}");
    Console.WriteLine($"Height: {player1.Height}cm");
}

