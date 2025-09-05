using Microsoft.EntityFrameworkCore;
using cuestionarioNom.Domain.Entities;

namespace cuestionarioNom.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Questionnaire> Questionnaires => Set<Questionnaire>();
        public DbSet<Section> Sections => Set<Section>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<Option> Options => Set<Option>();
        public DbSet<QuestionTag> QuestionTags => Set<QuestionTag>();
        public DbSet<Survey> Surveys => Set<Survey>();
        public DbSet<Response> Responses => Set<Response>();
        public DbSet<ScoringRule> ScoringRules => Set<ScoringRule>();
        public DbSet<Score> Scores => Set<Score>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            mb.Entity<Questionnaire>(e =>
            {
                e.Property(x => x.Code).HasMaxLength(100);
                e.Property(x => x.Title).HasMaxLength(300);
            });

            mb.Entity<Section>(e =>
            {
                e.HasIndex(x => new { x.QuestionnaireId, x.Order }).IsUnique(false);
                e.Property(x => x.Name).HasMaxLength(200);
            });

            mb.Entity<Question>(e =>
            {
                e.HasIndex(x => new { x.SectionId, x.Order }).IsUnique(false);
                e.Property(x => x.Code).HasMaxLength(50);
                e.Property(x => x.Text).HasMaxLength(2000);
            });

            mb.Entity<Option>(e =>
            {
                e.HasIndex(x => new { x.QuestionId, x.Order }).IsUnique(false);
                e.Property(x => x.Label).HasMaxLength(200);
            });

            mb.Entity<QuestionTag>(e =>
            {
                e.Property(x => x.Tag).HasMaxLength(200);
                e.HasIndex(x => new { x.QuestionId, x.Tag }).IsUnique();
            });

            mb.Entity<ScoringRule>(e =>
            {
                e.Property(x => x.Level).HasMaxLength(50);
                e.Property(x => x.SectionName).HasMaxLength(200);
                e.Property(x => x.Tag).HasMaxLength(200);
            });

            mb.Entity<Survey>(e =>
            {
                e.Property(x => x.RespondentId).HasMaxLength(200);
            });

            // ------------ Parche específico: Responses / Scores ------------
            mb.Entity<Response>(e =>
            {
                // 1) Precisión explícita para evitar truncamientos silenciosos
                e.Property(x => x.NumericAnswer)
                 .HasPrecision(18, 2);

                // 2) Relaciones y comportamientos de borrado
                //    a) Survey -> Responses : CASCADE (borra respuestas del levantamiento)
                e.HasOne(r => r.Survey)
                 .WithMany(s => s.Responses)
                 .HasForeignKey(r => r.SurveyId)
                 .OnDelete(DeleteBehavior.Cascade);

                //    b) Question -> Responses : RESTRICT (rompe múltiples rutas de cascada)
                e.HasOne(r => r.Question)
                 .WithMany()
                 .HasForeignKey(r => r.QuestionId)
                 .OnDelete(DeleteBehavior.Restrict);

                //    c) SelectedOption -> Responses : NO ACTION (opcional, evita cascada)
                e.HasOne(r => r.SelectedOption)
                 .WithMany()
                 .HasForeignKey(r => r.SelectedOptionId)
                 .OnDelete(DeleteBehavior.NoAction);
            });

            mb.Entity<Score>(e =>
            {
                // Survey -> Scores : CASCADE (borra scores del levantamiento)
                e.HasOne(s => s.Survey)
                 .WithMany(su => su.Scores)
                 .HasForeignKey(s => s.SurveyId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
            // ---------------------------------------------------------------
        }
    }
}
