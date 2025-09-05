namespace cuestionarioNom.Domain.Entities
{
    public class Option
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;
        public string Label { get; set; } = "";
        public int Value { get; set; }
        public int Order { get; set; }
    }
}
