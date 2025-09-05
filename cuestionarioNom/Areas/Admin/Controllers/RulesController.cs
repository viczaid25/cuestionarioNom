using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cuestionarioNom.Data;
using cuestionarioNom.Domain.Entities;
using cuestionarioNom.Domain.Enums;
using cuestionarioNom.Models.ViewModels;

namespace cuestionarioNom.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RulesController : Controller
    {
        private readonly ApplicationDbContext _db;
        public RulesController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index(int questionnaireId)
        {
            var q = await _db.Questionnaires.Include(x => x.ScoringRules).FirstOrDefaultAsync(x => x.Id == questionnaireId);
            if (q is null) return NotFound();
            ViewBag.Questionnaire = q;
            return View(q.ScoringRules.OrderBy(r => r.Scope).ThenBy(r => r.Priority).ToList());
        }

        public IActionResult Create(int questionnaireId)
        {
            return View(new RuleEditVm { QuestionnaireId = questionnaireId });
        }

        [HttpPost]
        public async Task<IActionResult> Create(RuleEditVm vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var rule = new ScoringRule
            {
                QuestionnaireId = vm.QuestionnaireId,
                Scope = vm.Scope switch
                {
                    "Section" => ScoreScope.Section,
                    "Tag" => ScoreScope.Tag,
                    _ => ScoreScope.Questionnaire
                },
                SectionName = vm.SectionName,
                Tag = vm.Tag,
                Min = vm.Min,
                Max = vm.Max,
                Level = vm.Level,
                Color = vm.Color,
                Priority = vm.Priority
            };
            _db.ScoringRules.Add(rule);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { questionnaireId = vm.QuestionnaireId });
        }
    }
}
