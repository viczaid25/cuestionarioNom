using System;
using System.Threading.Tasks;

namespace cuestionarioNom.Services.Interfaces
{
    public interface IReportService
    {
        Task<byte[]> ExportSurveySummaryAsync(Guid surveyId); // opcional
    }
}
