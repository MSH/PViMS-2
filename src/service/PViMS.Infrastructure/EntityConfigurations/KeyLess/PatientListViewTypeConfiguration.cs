using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities.Keyless;

namespace PVIMS.Infrastructure.EntityConfigurations.KeyLess
{
    class PatientListViewTypeConfiguration : IEntityTypeConfiguration<PatientList>
    {
        public void Configure(EntityTypeBuilder<PatientList> configuration)
        {
            configuration.ToView("vwPatientVisitList");

            configuration.HasNoKey();
        }
    }
}
