namespace PVIMS.Core.Entities.Accounts
{
    public class UserFacility : EntityBase
    {
        public int FacilityId { get; set; }
        public int UserId { get; set; }

        public User User { get; set; }
        public Facility Facility { get; set; }
    }
}
