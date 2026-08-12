using System;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Application.Classes.Exceptions;
using gmvTM.Domain;
using gmvTM.Domain.Extensions.Collections;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Workers.Interfaces;
using gmvTM.Domain.Items;

namespace gmvTM.Application.Classes.Tools
{
    public static class RouteCodeLookup
    {
        public static string Normalize(string? routeCode)
        {
            if (string.IsNullOrWhiteSpace(routeCode))
                throw new ValidationException(gmvDomain.Messages.RouteCodeRequired);

            return routeCode.Trim().ToUpperInvariant();
        }

        public static async Task<RouteItem> RequireAsync(IRouteCollection routes, string? routeCode, CancellationToken ct)
        {
            string code = Normalize(routeCode);
            RouteItem? route = await routes.GetByCodeAsync(code, ct);

            if (route is null)
                throw new NotFoundException(string.Format(gmvDomain.Messages.RouteNotFound, code));

            return route;
        }
    }
}
