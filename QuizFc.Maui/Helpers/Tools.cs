namespace QuizFc.Helpers
{
    public static class Tools
    {
        public static string MapPlayerAgeToTxt(int age)
        {
            if (age <= 21) return "lat";
            if (age <= 24) return "lata";
            if (age <= 31) return "lat";
            if (age <= 34) return "lata";
            if (age <= 41) return "lat";
            if (age <= 44) return "lata";
            return "lat";
        }
    }
}
