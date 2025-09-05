using System.Collections.Generic;

namespace cuestionarioNom.Models.Dtos
{
    public class ImportQuestionnaireDto
    {
        public string Code { get; set; } = "";
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public List<SectionDto> Sections { get; set; } = new();
        public List<ScoringRuleDto> ScoringRules { get; set; } = new();
    }

    public class SectionDto
    {
        public string Name { get; set; } = "";
        public int Order { get; set; }
        public List<QuestionDto> Questions { get; set; } = new();
    }

    public class QuestionDto
    {
        public string Code { get; set; } = "";
        public string Text { get; set; } = "";
        public string Type { get; set; } = "SingleChoiceLikert";
        public int Order { get; set; }
        public bool ReverseScore { get; set; } = false;
        public List<OptionDto> Options { get; set; } = new();
        public List<string> Tags { get; set; } = new();
    }

    public class OptionDto
    {
        public string Label { get; set; } = "";
        public int Value { get; set; }
        public int Order { get; set; }
    }

    public class ScoringRuleDto
    {
        public string Scope { get; set; } = "Questionnaire";
        public string? SectionName { get; set; }
        public string? Tag { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public string Level { get; set; } = "";
        public string? Color { get; set; }
        public int Priority { get; set; } = 0;
    }
}
