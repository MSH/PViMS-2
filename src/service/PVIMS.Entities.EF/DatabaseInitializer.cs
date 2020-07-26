using System.Data.Entity;
using PVIMS.Entities.EF.Migrations;

namespace PVIMS.Entities.EF
{
    public  class DatabaseInitializer : MigrateDatabaseToLatestVersion<PVIMSDbContext, Configuration>
    {
    }
}
