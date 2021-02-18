using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities.Keyless;

namespace PVIMS.Infrastructure.EntityConfigurations.KeyLess
{
    class AppointmentListViewTypeConfiguration : IEntityTypeConfiguration<AppointmentList>
    {
        public void Configure(EntityTypeBuilder<AppointmentList> configuration)
        {
            configuration.ToView("vwAppointmentList");

            configuration.HasNoKey();
        }
    }
}
