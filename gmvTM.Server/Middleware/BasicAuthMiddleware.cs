using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace gmvTM.Server.Middleware
{
    // For this small exam project we deliberately keep security simple: plain HTTP Basic
    // authentication with a single username and password, no claims, roles, or tokens.
    // The password is never stored in plain text; appsettings.json holds its SHA-256 hash
    // and incoming passwords are hashed and compared in constant time.
    public sealed class BasicAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _username;
        private readonly byte[] _passwordHash;

        public BasicAuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _username = configuration[gmvServer.Security.AuthUsernameSetting] ?? string.Empty;
            _passwordHash = Convert.FromHexString(configuration[gmvServer.Security.AuthPasswordHashSetting] ?? string.Empty);
        }

        public Task InvokeAsync(HttpContext context)
        {
            if (!RequiresAuthentication(context.Request.Path))
                return _next(context);

            if (IsAuthorized(context.Request.Headers.Authorization.ToString()))
                return _next(context);

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.Headers.WWWAuthenticate = gmvServer.Security.WwwAuthenticateValue;
            return context.Response.WriteAsync(gmvServer.Messages.ErrorTitleUnauthorized);
        }

        private static bool RequiresAuthentication(PathString path)
        {
            return path.StartsWithSegments(gmvServer.Security.ApiPathPrefix, StringComparison.OrdinalIgnoreCase)
                || path.StartsWithSegments(gmvServer.Security.ODataPathPrefix, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsAuthorized(string authorizationHeader)
        {
            if (!authorizationHeader.StartsWith(gmvServer.Security.BasicSchemePrefix, StringComparison.OrdinalIgnoreCase))
                return false;

            string decoded;
            try
            {
                decoded = Encoding.UTF8.GetString(Convert.FromBase64String(authorizationHeader.Substring(gmvServer.Security.BasicSchemePrefix.Length).Trim()));
            }
            catch (FormatException)
            {
                return false;
            }

            int separator = decoded.IndexOf(':');
            if (separator < 0)
                return false;

            string username = decoded.Substring(0, separator);
            string password = decoded.Substring(separator + 1);

            if (!string.Equals(username, _username, StringComparison.Ordinal))
                return false;

            byte[] providedHash = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return CryptographicOperations.FixedTimeEquals(providedHash, _passwordHash);
        }
    }
}
