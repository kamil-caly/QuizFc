namespace QuizFc.Views;

[QueryProperty(nameof(Score), "Score")]
public partial class GameSummaryPage : ContentPage
{
	public GameSummaryPage()
	{
		InitializeComponent();
	}

    private string score;
    public string Score
    {
        get => score;
        set
        {
            score = value;
            // tutaj masz ju¿ wynik
            //ScoreLabel.Text = $"Twój wynik: {score}";
        }
    }
}