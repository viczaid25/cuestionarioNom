using cuestionarioNom.Domain.Enums;

namespace cuestionarioNom.Domain.Entities
{
    public class Score
    {
        public int Id { get; set; }
        public System.Guid SurveyId { get; set; }
        public Survey Survey { get; set; } = null!;
        public ScoreScope Scope { get; set; }
        public string? SectionName { get; set; }
        public string? Tag { get; set; }
        public int Raw { get; set; }
        public string Level { get; set; } = "";
    }
}
