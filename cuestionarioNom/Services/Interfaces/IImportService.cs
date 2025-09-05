using System.Threading.Tasks;
using cuestionarioNom.Models.Dtos;

namespace cuestionarioNom.Services.Interfaces
{
    public interface IImportService
    {
        Task<int> ImportAsync(ImportQuestionnaireDto dto);
    }
}
