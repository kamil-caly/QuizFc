namespace QuizFc.Components;

public partial class Navbar : ContentView
{
	public Navbar()
	{
		InitializeComponent();
	}

    private async void LogoOnClick(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}