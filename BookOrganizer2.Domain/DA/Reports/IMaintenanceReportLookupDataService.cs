using System.Threading.Tasks;

namespace BookOrganizer2.Domain.DA.Reports
{
    public interface IMaintenanceReportLookupDataService
    {
        Task<MaintenanceReportItems> GetMaintenanceData();
    }
}
