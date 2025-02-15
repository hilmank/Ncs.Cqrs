using Ncs.Cqrs.Application.Common.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ncs.Cqrs.Api.Helpers
{
    public class GenericResponseSchemaFilter : ISchemaFilter, IOperationFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsGenericType && context.Type.GetGenericTypeDefinition() == typeof(ResponseDto<>))
            {
                var itemType = context.Type.GenericTypeArguments[0];

                schema.Properties["data"] = GetOpenApiSchemaForType(itemType);
                schema.Properties["success"] = new OpenApiSchema { Type = "boolean" };
                schema.Properties["message"] = new OpenApiSchema { Type = "string" };
            }

            if (schema.Properties == null) return;

            var pascalCaseProps = new Dictionary<string, OpenApiSchema>();
            foreach (var prop in schema.Properties)
            {
                var pascalCaseKey = char.ToUpper(prop.Key[0]) + prop.Key.Substring(1);
                pascalCaseProps[pascalCaseKey] = prop.Value;
            }
            schema.Properties = pascalCaseProps;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Add standardized response codes for all endpoints returning ResponseDto<T>
            if (context.MethodInfo.ReturnType.IsGenericType &&
                context.MethodInfo.ReturnType.GetGenericTypeDefinition() == typeof(ResponseDto<>))
            {
                var responses = new Dictionary<int, string>
                    {
                        { 200, "Returns the requested data successfully." },
                        { 400, "Invalid input. Missing or incorrect parameters." },
                        { 401, "Unauthorized. Invalid or missing authentication token." },
                        { 403, "Forbidden. The user does not have the necessary permissions." },
                        { 404, "The requested resource was not found." },
                        { 500, "An error occurred while processing the request." }
                    };
                foreach (var response in responses)
                {
                    operation.Responses[response.Key.ToString()] = new OpenApiResponse { Description = response.Value };
                }
            }
        }

        private static bool IsCollectionType(Type type)
        {
            if (!type.IsGenericType) return false;

            var genericTypeDefinition = type.GetGenericTypeDefinition();
            return genericTypeDefinition == typeof(List<>) ||
                   genericTypeDefinition == typeof(IEnumerable<>) ||
                   genericTypeDefinition == typeof(ICollection<>) ||
                   genericTypeDefinition == typeof(IReadOnlyCollection<>);
        }

        /// <summary>
        /// Generates the correct OpenApiSchema for a given type, handling nullable and collection types.
        /// </summary>
        private OpenApiSchema GetOpenApiSchemaForType(Type itemType)
        {
            if (itemType == typeof(string))
            {
                return new OpenApiSchema { Type = "string", Nullable = true };
            }

            if (itemType == typeof(object))
            {
                return new OpenApiSchema { Type = "object", Nullable = true };
            }

            if (itemType.IsGenericType && itemType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = Nullable.GetUnderlyingType(itemType);
                return new OpenApiSchema
                {
                    Type = underlyingType != null ? GetOpenApiType(underlyingType) : "string",
                    Nullable = true
                };
            }

            if (IsCollectionType(itemType))
            {
                var elementType = itemType.GetGenericArguments()[0];
                return new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = elementType.Name }
                    }
                };
            }

            if (itemType.IsPrimitive || itemType == typeof(decimal))
            {
                return new OpenApiSchema { Type = GetOpenApiType(itemType) };
            }

            return new OpenApiSchema
            {
                Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = itemType.Name }
            };
        }

        /// <summary>
        /// Maps C# types to OpenAPI primitive types.
        /// </summary>
        private string GetOpenApiType(Type type)
        {
            return type switch
            {
                _ when type == typeof(int) => "integer",
                _ when type == typeof(long) => "integer",
                _ when type == typeof(float) => "number",
                _ when type == typeof(double) => "number",
                _ when type == typeof(decimal) => "number",
                _ when type == typeof(bool) => "boolean",
                _ => "string"
            };
        }
    }
}
