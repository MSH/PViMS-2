using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using PVIMS.API.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PVIMS.API.OperationFilters
{
    public class GetProductOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //if (operation.OperationId == "GetProductsByIdentifier")
            //{
            //    operation.Responses[StatusCodes.Status200OK.ToString()].Content.Add(
            //        "application/vnd.pvims.detail.v1+json",
            //        new OpenApiMediaType()
            //        {
            //            Schema = context.SchemaRegistry.GetOrRegister(typeof(LinkedCollectionResourceWrapperDto<ProductDetailDto>))
            //        });
            //    operation.Responses[StatusCodes.Status200OK.ToString()].Content.Add(
            //        "application/vnd.pvims.detail.v1+xml",
            //        new OpenApiMediaType()
            //        {
            //            Schema = context.SchemaRegistry.GetOrRegister(typeof(LinkedCollectionResourceWrapperDto<ProductDetailDto>))
            //        });
            //}

            //if (operation.OperationId == "GetProductByIdentifier")
            //{
            //    operation.Responses[StatusCodes.Status200OK.ToString()].Content.Add(
            //        "application/vnd.pvims.detail.v1+json",
            //        new OpenApiMediaType()
            //        {
            //            Schema = context.SchemaRegistry.GetOrRegister(typeof(ProductDetailDto))
            //        });
            //    operation.Responses[StatusCodes.Status200OK.ToString()].Content.Add(
            //        "application/vnd.pvims.detail.v1+xml",
            //        new OpenApiMediaType()
            //        {
            //            Schema = context.SchemaRegistry.GetOrRegister(typeof(ProductDetailDto))
            //        });
            //}

        }
    }
}
