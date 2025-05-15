﻿namespace GitIssueTracker.Api.Middlewares;

public static class ExceptionHandlingExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
