﻿using BCM.Api.BusinessLayer;

namespace BCM.Api.Apis;

public static class BooksBulkImportApi
{
    const string BaseRoute = "/api/books";

    public static void MapBooksBulkImportApi(this WebApplication app)
    {
        app.MapPost("/bulkimport", async (IBookBulkImportService bookBulkImportService, IFormFile file, CancellationToken cancellationToken = default) =>
        {
            await bookBulkImportService.ImportBooksAsync(file, cancellationToken);
            return Results.Ok();
        })
        .WithName("BulkImport").WithDescription("Import books from csv file.")
        .Produces(StatusCodes.Status200OK);
    }
}