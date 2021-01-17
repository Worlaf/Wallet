using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;

namespace Wallet.API.App
{
    public static class HttpContextExtensions
    {
        public static async Task ExecuteResultAsync<TActionResult>(this HttpContext self, TActionResult result) where TActionResult : IActionResult
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            if (result == null)
                throw new ArgumentNullException(nameof(result));

            var routeData = self.GetRouteData() ?? new RouteData();
            var actionContext = new ActionContext(self, routeData, new ActionDescriptor());
            await result.ExecuteResultAsync(actionContext);
        }
    }
}
