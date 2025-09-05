using System.Collections.Generic;

namespace cuestionarioNom.Domain.Entities
{
    public class Questionnaire
    {
        public int Id { get; set; }
        public string Code { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsActive { get; set; } = true;

        public ICollection<Section> Sections { get; set; } = new List<Section>();
        public ICollection<ScoringRule> ScoringRules { get; set; } = new List<ScoringRule>();
    }
}
