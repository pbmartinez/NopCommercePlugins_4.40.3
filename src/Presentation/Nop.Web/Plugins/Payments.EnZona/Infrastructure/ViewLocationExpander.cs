﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Nop.Plugin.Payments.EnZona.Infrastructure
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
                    $"/Plugins/Payments.EnZona/Areas/Admin/Views/{context.ControllerName}/{context.ViewName}.cshtml" ,
                    $"/Plugins/Payments.EnZona/Areas/Admin/Views/{context.ViewName}.cshtml"
                }.Concat(viewLocations);
            }
            else
            {
                viewLocations = new[]
                {
                    $"/Plugins/Payments.EnZona/Views/{context.ControllerName}/{context.ViewName}.cshtml" ,
                    $"/Plugins/Payments.EnZona/Views/{context.ViewName}.cshtml"

                }.Concat(viewLocations);
            }

            return viewLocations;
        }
    }
}
