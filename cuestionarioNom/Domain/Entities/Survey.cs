using System;
using System.Collections.Generic;

namespace cuestionarioNom.Domain.Entities
{
    public class Survey
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int QuestionnaireId { get; set; }
        public Questionnaire Questionnaire { get; set; } = null!;
        public string RespondentId { get; set; } = "";
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        public ICollection<Response> Responses { get; set; } = new List<Response>();
        public ICollection<Score> Scores { get; set; } = new List<Score>();
    }
}
