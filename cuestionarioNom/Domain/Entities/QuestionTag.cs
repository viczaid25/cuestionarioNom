namespace cuestionarioNom.Domain.Entities
{
    public class QuestionTag
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;
        public string Tag { get; set; } = "";
    }
}
