using QuizFc.Views;

namespace QuizFc
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(WhoMoreQuizPage), typeof(WhoMoreQuizPage));
            Routing.RegisterRoute(nameof(GameSummaryPage), typeof(GameSummaryPage));
        }
    }
}
