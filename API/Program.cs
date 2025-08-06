using Application;
using Application.Common.Interfaces;
using Application.Meetings.Create;
using Application.Users.Commands.Create;
using Application.Users.Queries.GetUserMettings;
using Domain.Entities;
using Domain.ValueObjects;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddPersistence();
builder.Services.AddApplicationServices();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();

app.MapPost("/users", async (CreateUserCommand request, ICommandHandler<CreateUserCommand, User> commandHandler) =>
{
    var result = await commandHandler.Handle(request);
    if (!result.IsSuccess)
    {
        return Results.BadRequest(result.Error);
    }

    return Results.Created($"/users/{result.Value.Id}", result.Value);
});

app.MapPost("/meetings", async (CreateMeetingCommand request, ICommandHandler<CreateMeetingCommand, Interval> commandHandler) =>
{
    var result = await commandHandler.Handle(request);
    if (!result.IsSuccess)
    {
        return Results.BadRequest(result.Error);
    }

    return Results.Ok<Interval>(result.Value);
});

app.MapGet("/users/{userId}/meetings", async (Guid userId, IQueryHandler<GetUserMeetingsQuery, IReadOnlyCollection<Meeting>> queryHandler) => 
{
    var query = new GetUserMeetingsQuery { UserId = userId };
    var result = await queryHandler.Handle(query);
    if (!result.IsSuccess)
    {
        return Results.BadRequest(result.Error);
    }

    return Results.Ok(result.Value);
});

app.Run();
