using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using cuestionarioNom.Data;
using cuestionarioNom.Domain.Entities;
using cuestionarioNom.Models.Dtos;
using cuestionarioNom.Domain.Enums;

namespace cuestionarioNom.Services
{
    public class ImportService : Interfaces.IImportService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public ImportService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db; _mapper = mapper;
        }

        public async Task<int> ImportAsync(ImportQuestionnaireDto dto)
        {
            // Si ya existe por Code, crea uno nuevo con otro Code o, si prefieres, actúa como "update".
            if (await _db.Questionnaires.AnyAsync(x => x.Code == dto.Code))
            {
                dto.Code = $"{dto.Code}-{System.DateTime.UtcNow:yyyyMMddHHmmss}";
            }

            var q = new Questionnaire
            {
                Code = dto.Code,
                Title = dto.Title,
                Description = dto.Description ?? "",
                IsActive = true
            };

            // Secciones, preguntas, opciones y tags
            foreach (var s in dto.Sections.OrderBy(x => x.Order))
            {
                var sec = new Section { Name = s.Name, Order = s.Order };
                foreach (var qu in s.Questions.OrderBy(x => x.Order))
                {
                    var question = new Question
                    {
                        Code = qu.Code,
                        Text = qu.Text,
                        Type = qu.Type?.ToLowerInvariant() switch
                        {
                            "multiplechoice" => QuestionType.MultipleChoice,
                            "text" => QuestionType.Text,
                            "number" => QuestionType.Number,
                            _ => QuestionType.SingleChoiceLikert
                        },
                        Order = qu.Order,
                        ReverseScore = qu.ReverseScore
                    };

                    foreach (var opt in qu.Options.OrderBy(x => x.Order))
                        question.Options.Add(new Option { Label = opt.Label, Value = opt.Value, Order = opt.Order });

                    foreach (var tag in qu.Tags.Distinct())
                        question.Tags.Add(new QuestionTag { Tag = tag });

                    sec.Questions.Add(question);
                }
                q.Sections.Add(sec);
            }

            // Reglas de calificación
            foreach (var r in dto.ScoringRules)
            {
                q.ScoringRules.Add(new ScoringRule
                {
                    Scope = r.Scope?.ToLowerInvariant() switch
                    {
                        "section" => ScoreScope.Section,
                        "tag" => ScoreScope.Tag,
                        _ => ScoreScope.Questionnaire
                    },
                    SectionName = r.SectionName,
                    Tag = r.Tag,
                    Min = r.Min,
                    Max = r.Max,
                    Level = r.Level,
                    Color = r.Color,
                    Priority = r.Priority
                });
            }

            _db.Questionnaires.Add(q);
            await _db.SaveChangesAsync();
            return q.Id;
        }
    }
}
