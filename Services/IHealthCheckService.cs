

using Tabasco.Models;

namespace Tabasco.Services
{
    public interface IHealthCheckService
    {
        Task<KavenegarHealthCheckResult> CheckKavenegarHealthAsync();
    }
}