using System.Collections.Generic;

namespace cuestionarioNom.Models.ViewModels
{
    public class QuestionnaireEditVm
    {
        public int? Id { get; set; }
        public string Code { get; set; } = "";
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public List<SectionEditVm> Sections { get; set; } = new();
    }

    public class SectionEditVm
    {
        public int? Id { get; set; }
        public string Name { get; set; } = "";
        public int Order { get; set; }
    }
}
