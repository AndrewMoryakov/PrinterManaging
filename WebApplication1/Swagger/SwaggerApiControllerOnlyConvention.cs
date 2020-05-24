using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebApplication1.Swagger
{
    public class SwaggerApiControllerOnlyConvention : IActionModelConvention
    {
        public void Apply(ActionModel action) =>
            action.ApiExplorer.IsVisible = action.Attributes.OfType<ApiControllerAttribute>().Any();
    }
}