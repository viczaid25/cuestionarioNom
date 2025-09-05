using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cuestionarioNom.Data;
using cuestionarioNom.Domain.Entities;
using cuestionarioNom.Models.ViewModels;
using cuestionarioNom.Services.Interfaces;

namespace cuestionarioNom.Controllers
{
    public class SurveysController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IScoringService _scoring;
        private readonly IMapper _mapper;

        public SurveysController(ApplicationDbContext db, IScoringService scoring, IMapper mapper)
        {
            _db = db; _scoring = scoring; _mapper = mapper;
        }

        // GET: /Surveys/Start?questionnaireId=1
        public async Task<IActionResult> Start(int questionnaireId, string? respondentId)
        {
            var q = await _db.Questionnaires.AsNoTracking().FirstOrDefaultAsync(x => x.Id == questionnaireId);
            if (q is null) return NotFound();

            if (string.IsNullOrWhiteSpace(respondentId))
            {
                ViewBag.Questionnaire = q;
                return View(); // muestra input de respondentId
            }

            var s = new Survey { QuestionnaireId = questionnaireId, RespondentId = respondentId };
            _db.Surveys.Add(s);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Fill), new { id = s.Id });
        }

        // GET: /Surveys/Fill/{id}
        public async Task<IActionResult> Fill(Guid id)
        {
            var survey = await _db.Surveys
                .Include(s => s.Questionnaire).ThenInclude(q => q.Sections).ThenInclude(sec => sec.Questions).ThenInclude(q => q.Options)
                .Include(s => s.Responses)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (survey is null) return NotFound();

            var vm = _mapper.Map<SurveyFillVm>(survey.Questionnaire);
            vm.SurveyId = survey.Id;
            vm.Sections = survey.Questionnaire.Sections
                .OrderBy(s => s.Order)
                .Select(sec => new SectionVm
                {
                    Name = sec.Name,
                    Order = sec.Order,
                    Questions = sec.Questions.OrderBy(q => q.Order).Select(qu =>
                    {
                        var r = survey.Responses.FirstOrDefault(x => x.QuestionId == qu.Id);
                        return new QuestionVm
                        {
                            Id = qu.Id,
                            Code = qu.Code,
                            Text = qu.Text,
                            SelectedOptionId = r?.SelectedOptionId,
                            Options = qu.Options.OrderBy(o => o.Order).Select(o => new OptionVm
                            {
                                Id = o.Id,
                                Label = o.Label,
                                Order = o.Order
                            }).ToList()
                        };
                    }).ToList()
                }).ToList();

            return View(vm);
        }

        // POST: /Surveys/Answer
        [HttpPost]
        public async Task<IActionResult> Answer(Guid surveyId, int questionId, int selectedOptionId)
        {
            var surveyExists = await _db.Surveys.AnyAsync(s => s.Id == surveyId);
            if (!surveyExists) return NotFound();

            var resp = await _db.Responses.FirstOrDefaultAsync(r => r.SurveyId == surveyId && r.QuestionId == questionId);
            if (resp is null)
            {
                resp = new Response { SurveyId = surveyId, QuestionId = questionId, SelectedOptionId = selectedOptionId };
                _db.Responses.Add(resp);
            }
            else
            {
                resp.SelectedOptionId = selectedOptionId;
            }
            await _db.SaveChangesAsync();
            return Ok();
        }

        // POST: /Surveys/Finish/{id}
        [HttpPost]
        public async Task<IActionResult> Finish(Guid id)
        {
            var exists = await _db.Surveys.AnyAsync(s => s.Id == id);
            if (!exists) return NotFound();

            await _scoring.RecalculateAsync(id);
            return RedirectToAction(nameof(Result), new { id });
        }

        // GET: /Surveys/Result/{id}
        public async Task<IActionResult> Result(Guid id)
        {
            var survey = await _db.Surveys
                .Include(s => s.Questionnaire)
                .Include(s => s.Scores)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (survey is null) return NotFound();

            var vm = new SurveyResultVm
            {
                SurveyId = survey.Id,
                QuestionnaireTitle = survey.Questionnaire.Title,
                RespondentId = survey.RespondentId,
                Total = _mapper.Map<ScoreVm>(survey.Scores.First(x => x.Scope == Domain.Enums.ScoreScope.Questionnaire)),
                BySection = survey.Scores.Where(x => x.Scope == Domain.Enums.ScoreScope.Section).Select(_mapper.Map<ScoreVm>).ToList(),
                ByTag = survey.Scores.Where(x => x.Scope == Domain.Enums.ScoreScope.Tag).Select(_mapper.Map<ScoreVm>).ToList()
            };
            return View(vm);
        }
    }
}
