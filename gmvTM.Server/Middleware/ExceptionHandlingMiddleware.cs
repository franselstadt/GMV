using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using gmvTM.Application.Classes.Exceptions;
using gmvTM.Domain;

namespace gmvTM.Server.Middleware
{

    //i usually also add slack or email notification 
    public sealed class ExceptionHandlingMiddleware
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                await WriteErrorAsync(context, StatusCodes.Status400BadRequest, gmvServer.Messages.ErrorTitleValidation, [ex.Message]);
            }
            catch (NotFoundException ex)
            {
                await WriteErrorAsync(context, StatusCodes.Status404NotFound, gmvServer.Messages.ErrorTitleNotFound, [ex.Message]);
            }
            catch (InvalidOperationException ex)
            {
                await WriteErrorAsync(context, StatusCodes.Status400BadRequest, gmvServer.Messages.ErrorTitleBadRequest, [ex.Message]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, gmvServer.Messages.LogUnhandledException, context.TraceIdentifier);

                await WriteErrorAsync(
                    context,
                    StatusCodes.Status500InternalServerError,
                    gmvServer.Messages.ErrorTitleUnexpected,
                    [gmvServer.Messages.UnexpectedError]);
            }
        }

        private static async Task WriteErrorAsync(HttpContext context, int status, string title, string[] errors)
        {
            if (context.Response.HasStarted)
                throw new InvalidOperationException(gmvServer.Messages.ResponseAlreadyStarted);

            context.Response.Clear();
            context.Response.StatusCode = status;
            context.Response.ContentType = ContentTypes.ApplicationJson;

            ErrorEnvelope envelope = ItemFactory.CreateItem<ErrorEnvelope>(new
            {
                TraceID = context.TraceIdentifier,
                Title = title,
                Status = status,
                Messages = errors.ToList()
            });


            await context.Response.WriteAsync(JsonSerializer.Serialize(envelope, JsonOptions));
        }
    }
}
