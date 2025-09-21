namespace QuizFc.FootballWebScrapper.Models
{
    public class Player
    {
        public string Name { get; set; } = default!;
        public string Position { get; set; } = default!;
        public string FaceImgUrl { get; set; } = default!;
        public int Age { get; set; }
        public string Nationality { get; set; } = default!;
        public string NationalityImgUrl { get; set; } = default!;
        public string Club { get; set; } = default!;
        public string ClubImgUrl { get; set; } = default!;
        public decimal MarketValue { get; set; }
        /// <summary>
        /// Wysokość w centymetrach
        /// </summary>
        public int Height { get; set; }
    }
}
