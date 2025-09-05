using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using cuestionarioNom.Data;
using cuestionarioNom.Domain.Entities;
using cuestionarioNom.Domain.Enums;
using cuestionarioNom.Services.Interfaces;

namespace cuestionarioNom.Services
{
    public class ScoringService : IScoringService
    {
        private readonly ApplicationDbContext _db;
        public ScoringService(ApplicationDbContext db) => _db = db;

        public async Task RecalculateAsync(Guid surveyId, CancellationToken ct = default)
        {
            var survey = await _db.Surveys
                .Include(s => s.Questionnaire).ThenInclude(q => q.Sections).ThenInclude(sec => sec.Questions).ThenInclude(q => q.Options)
                .Include(s => s.Responses).ThenInclude(r => r.SelectedOption)
                .Include(s => s.Questionnaire).ThenInclude(q => q.ScoringRules)
                .FirstAsync(s => s.Id == surveyId, ct);

            _db.Scores.RemoveRange(_db.Scores.Where(x => x.SurveyId == surveyId));

            int MaxLikert(Question q) => q.Options.Any() ? q.Options.Max(o => o.Value) : 0;
            int ScoreOf(Question q, Response? r)
            {
                var v = r?.SelectedOption?.Value;
                if (v is null) return 0;
                return q.ReverseScore ? (MaxLikert(q) - v.Value) : v.Value;
            }

            // Por sección
            foreach (var sec in survey.Questionnaire.Sections.OrderBy(s => s.Order))
            {
                var qIds = sec.Questions.Select(x => x.Id).ToHashSet();
                var raw = survey.Responses.Where(r => qIds.Contains(r.QuestionId))
                    .Sum(r => ScoreOf(sec.Questions.First(q => q.Id == r.QuestionId), r));
                var lvl = ResolveLevel(survey.Questionnaire.ScoringRules, ScoreScope.Section, sec.Name, null, raw);
                _db.Scores.Add(new Score { SurveyId = surveyId, Scope = ScoreScope.Section, SectionName = sec.Name, Raw = raw, Level = lvl });
            }

            // Por etiqueta (dominio)
            var tags = survey.Questionnaire.Sections
                .SelectMany(s => s.Questions).SelectMany(q => q.Tags.Select(t => t.Tag)).Distinct();
            foreach (var tag in tags)
            {
                var qForTag = survey.Questionnaire.Sections.SelectMany(s => s.Questions)
                    .Where(q => q.Tags.Any(t => t.Tag == tag)).ToList();
                var raw = qForTag.Sum(q =>
                {
                    var r = survey.Responses.FirstOrDefault(x => x.QuestionId == q.Id);
                    return ScoreOf(q, r);
                });
                var lvl = ResolveLevel(survey.Questionnaire.ScoringRules, ScoreScope.Tag, null, tag, raw);
                _db.Scores.Add(new Score { SurveyId = surveyId, Scope = ScoreScope.Tag, Tag = tag, Raw = raw, Level = lvl });
            }

            // Total
            var allQ = survey.Questionnaire.Sections.SelectMany(s => s.Questions).ToList();
            var totalRaw = allQ.Sum(q =>
            {
                var r = survey.Responses.FirstOrDefault(x => x.QuestionId == q.Id);
                return ScoreOf(q, r);
            });
            var totalLvl = ResolveLevel(survey.Questionnaire.ScoringRules, ScoreScope.Questionnaire, null, null, totalRaw);
            _db.Scores.Add(new Score { SurveyId = surveyId, Scope = ScoreScope.Questionnaire, Raw = totalRaw, Level = totalLvl });

            await _db.SaveChangesAsync(ct);
            survey.CompletedAt ??= DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        private static string ResolveLevel(
            System.Collections.Generic.IEnumerable<ScoringRule> rules,
            ScoreScope scope, string? section, string? tag, int raw)
            => rules.Where(r => r.Scope == scope
                             && (scope != ScoreScope.Section || r.SectionName == section)
                             && (scope != ScoreScope.Tag || r.Tag == tag)
                             && raw >= r.Min && raw <= r.Max)
                    .OrderByDescending(r => r.Priority)
                    .Select(r => r.Level)
                    .FirstOrDefault() ?? "N/A";
    }
}
