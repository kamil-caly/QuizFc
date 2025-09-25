using QuizFc.Enums;
using QuizFc.FootballWebScraper;
using QuizFc.FootballWebScrapper.Models;
using QuizFc.Helpers;

namespace QuizFc.Views;


[QueryProperty(nameof(LifeParam), "Life")]
[QueryProperty(nameof(CategoryParam), "Category")]
[QueryProperty(nameof(LeagueParam), "League")]
public partial class WhoMoreQuizPage : ContentPage
{
    public string LifeParam { get; set; } = default!;
    public string CategoryParam { get; set; } = default!;
    public string LeagueParam { get; set; } = default!;

    public int Life { get; private set; }
    public WhoMoreCategory SelectedCategory { get; private set; }
    public League SelectedLeague { get; private set; }

    private WhoMoreCategory CurrentQuestionCategory;
    private int HeighScore;
    private const string CardColorGreen = "player_card_green.png";
    private const string CardColorPurple = "player_card_purple.png";
    private const string CardColorRed = "player_card_red.png";
    private const int CardClickDelay = 1200;
    private bool CanCardClick = false;
    private Player? currentPlayer1 = null;
    private Player? currentPlayer2 = null;

    public WhoMoreQuizPage()
    {
		InitializeComponent();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        Life = int.TryParse(LifeParam, out int life) ? life : 1;
        SelectedCategory = int.TryParse(CategoryParam, out int cat) ? (WhoMoreCategory)cat : WhoMoreCategory.MarketValue;
        SelectedLeague = int.TryParse(LeagueParam, out int league) ? (League)league : League.PremierLeague;

        LifeLabel.Text = Life.ToString();
        await SetNewPlayers();
    }

    private async Task SetNewPlayers()
    {
        FirstPlayerCardImg.Source = CardColorPurple;
        SecondPlayerCardImg.Source = CardColorPurple;

        WhoMoreCategory category = SelectedCategory;
        if (SelectedCategory == WhoMoreCategory.Random)
        {
            Random rand = new Random();
            switch (rand.Next(1, 4))
            {
                case 1:
                    category = WhoMoreCategory.MarketValue;
                    break;
                case 2:
                    category = WhoMoreCategory.Age;
                    break;
                case 3:
                    category = WhoMoreCategory.Height;
                    break;
            }
        }

        CurrentQuestionCategory = category;
        (Player player, Player player2) = await PlayerScraper.Get2PlayersWithDifferendCategoryValue(SelectedLeague, category);

        switch (category)
        {
            case WhoMoreCategory.MarketValue:
                CategoryLbl.Text = "dro¿szy";
                break;
            case WhoMoreCategory.Age:
                CategoryLbl.Text = "starszy";
                break;
            case WhoMoreCategory.Height:
                CategoryLbl.Text = "wy¿szy";
                break;
        }

        currentPlayer1 = player;
        currentPlayer2 = player2;

        FaceImgP1.Source = player.FaceImgUrl;
        FaceImgP2.Source = player2.FaceImgUrl;
        ClubImgP1.Source = player.ClubImgUrl;
        ClubImgP2.Source = player2.ClubImgUrl;
        NationalityImgP1.Source = player.NationalityImgUrl;
        NationalityImgP2.Source = player2.NationalityImgUrl;
        NameP1.Text = player.Name;
        NameP2.Text = player2.Name;

        CategoryValueP1.Text = CategoryValueP2.Text = "?";

        CanCardClick = true;
        FirstCardFrame.IsVisible = SecondCardFrame.IsVisible = true;
    }

    private void OnCardClickedLogic(int card)
    {
        if (currentPlayer1 == null || currentPlayer2 == null) return;
        decimal value = 0m;
        decimal value2 = 0m;

        switch (CurrentQuestionCategory)
        {
            case WhoMoreCategory.MarketValue:
                CategoryValueP1.Text = $"{currentPlayer1.MarketValue}M";
                CategoryValueP2.Text = $"{currentPlayer2.MarketValue}M";
                value = currentPlayer1.MarketValue;
                value2 = currentPlayer2.MarketValue;
                break;
            case WhoMoreCategory.Age:
                CategoryValueP1.Text = $"{currentPlayer1.Age} {Tools.MapPlayerAgeToTxt(currentPlayer1.Age)}";
                CategoryValueP2.Text = $"{currentPlayer2.Age} {Tools.MapPlayerAgeToTxt(currentPlayer2.Age)}";
                value = (decimal)currentPlayer1.Age;
                value2 = (decimal)currentPlayer2.Age;
                break;
            case WhoMoreCategory.Height:
                CategoryValueP1.Text = $"{currentPlayer1.Height}cm";
                CategoryValueP2.Text = $"{currentPlayer2.Height}cm";
                value = (decimal)currentPlayer1.Height;
                value2 = (decimal)currentPlayer2.Height;
                break;
        }

        if (card == 1)
        {
            if (value > value2)
            {
                FirstPlayerCardImg.Source = CardColorGreen;
                HeighScore++;
            }
            else
            {
                FirstPlayerCardImg.Source = CardColorRed;
                Life--;
            }
        }
        else
        {
            if (value2 > value)
            {
                SecondPlayerCardImg.Source = CardColorGreen;
                HeighScore++;
            }
            else
            {
                SecondPlayerCardImg.Source = CardColorRed;
                Life--;
            }
        }

        ScoreLabel.Text = HeighScore.ToString();
        LifeLabel.Text = Life.ToString();
    }

    private async void OnBackArrowClick(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(GameSummaryPage)}?Score={HeighScore}");
    }

    private async void FirstPlayerOnClick(object sender, TappedEventArgs e)
    {
        if (!CanCardClick) return;

        CanCardClick = false;
        OnCardClickedLogic(1);
        await Task.Delay(CardClickDelay);
        if (Life <= 0)
        {
            await Shell.Current.GoToAsync($"{nameof(GameSummaryPage)}?Score={HeighScore}");
            return;
        }
        FirstCardFrame.IsVisible = SecondCardFrame.IsVisible = false;
        CategoryLbl.Text = "...";
        await SetNewPlayers();
        CanCardClick = true;
    }

    private async void SecondPlayerOnClick(object sender, TappedEventArgs e)
    {
        if (!CanCardClick) return;

        CanCardClick = false;
        OnCardClickedLogic(2);
        await Task.Delay(CardClickDelay);
        if (Life <= 0)
        {
            await Shell.Current.GoToAsync($"{nameof(GameSummaryPage)}?Score={HeighScore}");
            return;
        }
        FirstCardFrame.IsVisible = SecondCardFrame.IsVisible = false;
        CategoryLbl.Text = "...";
        await SetNewPlayers();
        CanCardClick = true;
    }
}