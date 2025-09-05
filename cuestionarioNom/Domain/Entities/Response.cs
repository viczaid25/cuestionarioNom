namespace cuestionarioNom.Domain.Entities
{
    public class Response
    {
        public int Id { get; set; }
        public System.Guid SurveyId { get; set; }
        public Survey Survey { get; set; } = null!;
        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;
        public string? TextAnswer { get; set; }
        public decimal? NumericAnswer { get; set; }
        public int? SelectedOptionId { get; set; }
        public Option? SelectedOption { get; set; }
    }
}
