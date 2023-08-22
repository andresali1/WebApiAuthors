using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebApiAuthors.Utilities
{
    public class SwaggerGroupByVersion : IControllerModelConvention
    {
        /// <summary>
        /// Method to control the controllers according to the version
        /// </summary>
        /// <param name="controller"></param>
        public void Apply(ControllerModel controller)
        {
            var controllerNamespace = controller.ControllerType.Namespace; //Controllers.v1
            var ApiVersion = controllerNamespace.Split(".").Last().ToLower(); //v1
            controller.ApiExplorer.GroupName = ApiVersion;
        }
    }
}
