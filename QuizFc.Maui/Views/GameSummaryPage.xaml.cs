namespace QuizFc.Views;

[QueryProperty(nameof(Score), "Score")]
public partial class GameSummaryPage : ContentPage
{
	public GameSummaryPage()
	{
		InitializeComponent();
	}

    private string score = default!;
    public string Score
    {
        get => score;
        set
        {
            score = value;
            ScoreLabel.Text = score.ToString();
        }
    }

    private async void PlayAgainOnClick(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(WhoMoreQuizPage));
    }

    private async void ChangeGameOnClick(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}