using System;
using System.Threading;
using System.Threading.Tasks;

namespace cuestionarioNom.Services.Interfaces
{
    public interface IScoringService
    {
        Task RecalculateAsync(Guid surveyId, CancellationToken ct = default);
    }
}
