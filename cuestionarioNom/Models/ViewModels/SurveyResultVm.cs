using System.Collections.Generic;

namespace cuestionarioNom.Models.ViewModels
{
    public class SurveyResultVm
    {
        public System.Guid SurveyId { get; set; }
        public string QuestionnaireTitle { get; set; } = "";
        public string RespondentId { get; set; } = "";
        public ScoreVm Total { get; set; } = new();
        public List<ScoreVm> BySection { get; set; } = new();
        public List<ScoreVm> ByTag { get; set; } = new();
    }

    public class ScoreVm
    {
        public string Scope { get; set; } = "";
        public string? SectionName { get; set; }
        public string? Tag { get; set; }
        public int Raw { get; set; }
        public string Level { get; set; } = "";
    }
}
