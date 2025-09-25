using QuizFc.Enums;
using System.Windows.Input;

namespace QuizFc.Views;

public partial class WhoMoreConfigPage : ContentPage
{
    private int ChanceCounter = 1;
    private WhoMoreCategory SelectedCategory = WhoMoreCategory.MarketValue;
    private League SelectedLeague = League.PremierLeague;
    public WhoMoreConfigPage()
	{
		InitializeComponent();
        BindingContext = this;
        ChanceCounterBtn.Text = ChanceCounter.ToString();
        SetActiveCategoryButton(MarketValueBtn);
        SetActiveLeagueButton(PLBtn);
    }

    public ICommand ChanceCounterCommand => new Command<string>(OnChanceCounterClick);
    public ICommand CategoryCommand => new Command<string>(OnCategoryClick);
    public ICommand LeagueCommand => new Command<string>(OnLeagueClick);

    private void OnChanceCounterClick(string parameter)
    {
        if (parameter == "left" && ChanceCounter > 1)
        {
            ChanceCounter--;
        }
        else if (parameter == "right" && ChanceCounter < 3)
        {
            ChanceCounter++;
        }

        ChanceCounterBtn.Text = ChanceCounter.ToString();
    }

    private void OnCategoryClick(string parameter)
    {
        var map = new Dictionary<string, (Button Button, WhoMoreCategory Category)>
        {
            { "marketValue", (MarketValueBtn, WhoMoreCategory.MarketValue) },
            { "age", (AgeBtn, WhoMoreCategory.Age) },
            { "height", (HeightBtn, WhoMoreCategory.Height) },
            { "random", (RandomCategoryBtn, WhoMoreCategory.Random) }
        };

        if (!map.TryGetValue(parameter, out var selected))
        {
            // fallback jeœli parametru brak lub jest b³êdny
            selected = (MarketValueBtn, WhoMoreCategory.MarketValue);
        }

        SetActiveCategoryButton(selected.Button);
        SelectedCategory = selected.Category;
    }

    private void SetActiveCategoryButton(Button activeButton)
    {
        var activeColor = (Color)Application.Current!.Resources["ButtonActive"];
        var inactiveColor = (Color)Application.Current!.Resources["ButtonInactive"];

        foreach (var btn in new[] { MarketValueBtn, AgeBtn, HeightBtn, RandomCategoryBtn })
        {
            btn.BackgroundColor = inactiveColor;
            btn.BorderWidth = 0;
        }

        activeButton.BackgroundColor = activeColor;
        activeButton.BorderWidth = 2;
    }

    private void OnLeagueClick(string parameter)
    {
        var map = new Dictionary<string, (Button Button, League SelectedLeague)>
        {
            { "pl", (PLBtn, League.PremierLeague) },
            { "bl", (BLBtn, League.Bundesliga) },
            { "ll", (LLBtn, League.LaLiga) },
            { "sa", (SABtn, League.SerieA) },
            { "l1", (L1Btn, League.Ligue1) },
            { "all", (WholeWorldBtn, League.All) }
        };

        if (!map.TryGetValue(parameter, out var selected))
        {
            // fallback jeœli parametru brak lub jest b³êdny
            selected = (PLBtn, League.PremierLeague);
        }

        SetActiveLeagueButton(selected.Button);
        SelectedLeague = selected.SelectedLeague;
    }

    private void SetActiveLeagueButton(Button activeButton)
    {
        var activeColor = (Color)Application.Current!.Resources["ButtonActive"];
        var inactiveColor = (Color)Application.Current!.Resources["ButtonInactive"];

        foreach (var btn in new[] { PLBtn, BLBtn, LLBtn, SABtn, L1Btn, WholeWorldBtn })
        {
            btn.BackgroundColor = inactiveColor;
            btn.BorderWidth = 0;
        }

        activeButton.BackgroundColor = activeColor;
        activeButton.BorderWidth = 2;
    }

    private async void PlayQuizClick(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(WhoMoreQuizPage)}?" +
            $"Life={ChanceCounter}&SelectedCategory={SelectedCategory}&SelectedLeague={SelectedLeague}");
    }
}