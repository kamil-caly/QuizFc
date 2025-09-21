using QuizFc.Views;

namespace QuizFc
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnWhoMoreQuizClicked(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(WhoMoreConfigPage));
        }
    }
}
