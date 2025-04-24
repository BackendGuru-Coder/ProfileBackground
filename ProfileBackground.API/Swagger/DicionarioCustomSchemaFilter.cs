using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProfileBackground.API.Swagger
{
    public class DicionarioCustomSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(Dictionary<string, string>))
            {
                schema.Example = OpenApiAnyFactory.CreateFromJson(
                    "{\"parameter1\": \"true\", \"parameter2\": \"false\", \"parameter3\": \"true\"}"
                );

                schema.Description = "Um dicionário com chaves string e string de parâmetros.";
            }
        }
    }
}
