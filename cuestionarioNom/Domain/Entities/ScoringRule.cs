using cuestionarioNom.Domain.Enums;

namespace cuestionarioNom.Domain.Entities
{
    public class ScoringRule
    {
        public int Id { get; set; }
        public int QuestionnaireId { get; set; }
        public Questionnaire Questionnaire { get; set; } = null!;
        public ScoreScope Scope { get; set; }
        public string? SectionName { get; set; }
        public string? Tag { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public string Level { get; set; } = "";
        public string? Color { get; set; }
        public int Priority { get; set; } = 0;
    }
}
