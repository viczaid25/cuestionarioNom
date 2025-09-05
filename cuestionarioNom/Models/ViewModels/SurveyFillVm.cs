using System.Collections.Generic;

namespace cuestionarioNom.Models.ViewModels
{
    public class SurveyFillVm
    {
        public System.Guid SurveyId { get; set; }
        public string QuestionnaireTitle { get; set; } = "";
        public IEnumerable<SectionVm> Sections { get; set; } = new List<SectionVm>();
    }

    public class SectionVm
    {
        public string Name { get; set; } = "";
        public int Order { get; set; }
        public IEnumerable<QuestionVm> Questions { get; set; } = new List<QuestionVm>();
    }

    public class QuestionVm
    {
        public int Id { get; set; }
        public string Code { get; set; } = "";
        public string Text { get; set; } = "";
        public int? SelectedOptionId { get; set; }
        public IEnumerable<OptionVm> Options { get; set; } = new List<OptionVm>();
    }

    public class OptionVm
    {
        public int Id { get; set; }
        public string Label { get; set; } = "";
        public int Order { get; set; }
    }
}
