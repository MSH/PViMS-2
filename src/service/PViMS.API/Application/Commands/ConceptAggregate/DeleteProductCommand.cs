using MediatR;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.ConceptAggregate
{
    [DataContract]
    public class DeleteProductCommand
        : IRequest<bool>
    {
        [DataMember]
        public int ConceptId { get; private set; }

        [DataMember]
        public int ProductId { get; private set; }

        public DeleteProductCommand()
        {
        }

        public DeleteProductCommand(int conceptId, int productId): this()
        {
            ConceptId = conceptId;
            ProductId = productId;
        }
    }
}
