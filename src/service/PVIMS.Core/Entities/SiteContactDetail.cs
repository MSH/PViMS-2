using PVIMS.Core.ValueTypes;

namespace PVIMS.Core.Entities
{
    public class SiteContactDetail : AuditedEntityBase
    {
        public ContactType ContactType { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactSurname { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string ContactNumber { get; set; }
        public string ContactEmail { get; set; }
        public string CountryCode { get; set; }
        public string OrganisationName { get; set; }
    }
}