using System.Collections.Generic;
using cuestionarioNom.Domain.Enums;

namespace cuestionarioNom.Domain.Entities
{
    public class Question
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public Section Section { get; set; } = null!;
        public string Code { get; set; } = "";
        public string Text { get; set; } = "";
        public QuestionType Type { get; set; } = QuestionType.SingleChoiceLikert;
        public int Order { get; set; }
        public bool ReverseScore { get; set; } = false;

        public ICollection<Option> Options { get; set; } = new List<Option>();
        public ICollection<QuestionTag> Tags { get; set; } = new List<QuestionTag>();
    }
}
