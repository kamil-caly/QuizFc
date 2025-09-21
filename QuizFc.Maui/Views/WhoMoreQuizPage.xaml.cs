using QuizFc.Enums;
using QuizFc.FootballWebScraper;
using QuizFc.FootballWebScrapper.Models;

namespace QuizFc.Views;


[QueryProperty(nameof(Life), "Life")]
[QueryProperty(nameof(SelectedCategory), "SelectedCategory")]
[QueryProperty(nameof(SelectedLeague), "SelectedLeague")]
public partial class WhoMoreQuizPage : ContentPage
{
    // Query parameters at first
    private int lifePriv;
    public int Life
    {
        get => lifePriv;
        set
        {
            lifePriv = value;
            LifeLabel.Text = lifePriv.ToString();
        }
    }

    private WhoMoreCategory selectedCategoryPriv;
    public WhoMoreCategory SelectedCategory
    {
        get => selectedCategoryPriv;
        set
        {
            selectedCategoryPriv = value;
        }
    }

    private League selectedLeaguePriv;
    public League SelectedLeague
    {
        get => selectedLeaguePriv;
        set
        {
            selectedLeaguePriv = value;
        }
    }

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
        SetNewPlayers();
        HeighScore = 0;
        ScoreLabel.Text = HeighScore.ToString();
    }

    private async Task SetNewPlayers()
    {
        FirstPlayerCardImg.Source = CardColorPurple;
        SecondPlayerCardImg.Source = CardColorPurple;

        (Player player, Player player2) = await PlayerScraper.Get2PlayersWithDifferendMarketValue();

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

        MarketValueP1.Text = MarketValueP2.Text = "?";

        CanCardClick = true;
        FirstCardFrame.IsVisible = SecondCardFrame.IsVisible = true;
    }

    private void OnCardClickedLogic(int card)
    {
        if (currentPlayer1 == null || currentPlayer2 == null) return;

        MarketValueP1.Text = $"{currentPlayer1.MarketValue}M";
        MarketValueP2.Text = $"{currentPlayer2.MarketValue}M";

        if (card == 1)
        {
            if (currentPlayer1.MarketValue > currentPlayer2.MarketValue)
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
            if (currentPlayer2.MarketValue > currentPlayer1.MarketValue)
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
        await SetNewPlayers();
        CanCardClick = true;
    }
}