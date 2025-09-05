namespace cuestionarioNom.Models.ViewModels
{
    public class RuleEditVm
    {
        public int? Id { get; set; }
        public int QuestionnaireId { get; set; }
        public string Scope { get; set; } = "Questionnaire";
        public string? SectionName { get; set; }
        public string? Tag { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public string Level { get; set; } = "";
        public string? Color { get; set; }
        public int Priority { get; set; }
    }
}
