using Application.DTOs.Dashboard;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatisticsAsync();
    }
}