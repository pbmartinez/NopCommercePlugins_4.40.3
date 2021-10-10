using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Widgets.HelloWorld.Infrastructure
{
    public class ViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.AreaName == "Admin")
            {
                viewLocations = new[] 
                { 
                    $"/Plugins/Widgets.HelloWorld/Areas/Admin/Views/{context.ControllerName}/{context.ViewName}.cshtml" ,
                    $"/Plugins/Widgets.HelloWorld/Areas/Admin/Views/{context.ViewName}.cshtml" ,
                }.Concat(viewLocations);
            }
            else
            {
                viewLocations = new[] 
                {
                    $"/Plugins/Widgets.HelloWorld/Views/{context.ControllerName}/{context.ViewName}.cshtml" ,
                    $"/Plugins/Widgets.HelloWorld/Views/{context.ViewName}.cshtml"
                }.Concat(viewLocations);
            }

            return viewLocations;
        }
    }
}
