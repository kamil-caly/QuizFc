namespace QuizFc.FootballWebScraper.Models
{
    public class WhoMoreTop5LeagueScraperRequest
    {
        public string TopValuableAttackers { get; set; } = default!;
        public string TopValuableMidfielders { get; set; } = default!;
        public string TopValuableDefenders { get; set; } = default!;
        public string TopValuableGoalKeepers { get; set; } = default!;
        public int MaxAttackersPage { get; set; }
        public int MaxMidfieldersPage { get; set; }
        public int MaxDefendersPage { get; set; }
        public int MaxGoalKeepersPage { get; set; }
        public int LastTopValuableGolaKeepersPage { get; set; }
    }
}
