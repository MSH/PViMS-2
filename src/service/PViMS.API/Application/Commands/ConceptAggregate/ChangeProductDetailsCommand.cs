using MediatR;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.ConceptAggregate
{
    [DataContract]
    public class ChangeProductDetailsCommand
        : IRequest<bool>
    {
        [DataMember]
        public int ConceptId { get; private set; }

        [DataMember]
        public int ProductId { get; private set; }

        [DataMember]
        public string ProductName { get; private set; }

        [DataMember]
        public string Manufacturer { get; private set; }

        [DataMember]
        public string Description { get; private set; }

        [DataMember]
        public bool Active { get; private set; }

        public ChangeProductDetailsCommand()
        {
        }

        public ChangeProductDetailsCommand(int conceptId, int productId, string productName, string manufacturer, string description, bool active): this()
        {
            ConceptId = conceptId;
            ProductId = productId;
            ProductName = productName;
            Manufacturer = manufacturer;
            Description = description;
            Active = active;
        }
    }
}