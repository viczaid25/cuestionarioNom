using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cuestionarioNom.Data;
using cuestionarioNom.Models.ViewModels;

namespace cuestionarioNom.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QuestionnairesController : Controller
    {
        private readonly ApplicationDbContext _db;
        public QuestionnairesController(ApplicationDbContext db) => _db = db;

        // /Admin/Questionnaires
        public async Task<IActionResult> Index()
        {
            var list = await _db.Questionnaires.AsNoTracking()
                .OrderBy(x => x.Title).ToListAsync();
            return View(list); // esta vista puede seguir usando la Entity
        }

        // /Admin/Questionnaires/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var q = await _db.Questionnaires
                .Include(x => x.Sections)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (q is null) return NotFound();

            var vm = new QuestionnaireEditVm
            {
                Id = q.Id,
                Code = q.Code,
                Title = q.Title,
                Description = q.Description,
                IsActive = q.IsActive,
                Sections = q.Sections
                    .OrderBy(s => s.Order)
                    .Select(s => new SectionEditVm
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Order = s.Order
                    })
                    .ToList()
            };

            return View(vm); // ← ahora la vista Edit espera el VM
        }
    }
}
