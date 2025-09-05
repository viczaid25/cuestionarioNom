using System.Collections.Generic;

namespace cuestionarioNom.Domain.Entities
{
    public class Section
    {
        public int Id { get; set; }
        public int QuestionnaireId { get; set; }
        public Questionnaire Questionnaire { get; set; } = null!;
        public string Name { get; set; } = "";
        public int Order { get; set; }

        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
