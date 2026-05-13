using System.Text.Json.Serialization;
using Demo.API.Handlers;
using Demo.Application.Common.Extensions;
using Demo.Application.Common.Interfaces;
using Demo.Application.Common.Mappings;
using Demo.Infrastructure;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails(opt =>
{
    opt.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.Add("requestId", context.HttpContext.TraceIdentifier);
    };
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddEndpoints();
builder.Services.AddEndpointsApiExplorer();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddOpenApi(options =>
{
    options.CreateSchemaReferenceId = type =>
    {
        var schemaId = OpenApiOptions.CreateDefaultSchemaReferenceId(type);
        
        if (schemaId is null) 
            return null;

        if (!type.Type.IsNested) 
            return schemaId;
        
        var nameParts = new List<string> { schemaId };
        var declaringType = type.Type.DeclaringType;
            
        while (declaringType is not null)
        {
            nameParts.Insert(0, declaringType.Name);
                
            if (declaringType is { Name: "Endpoint", Namespace: not null })
            {
                var featureName = declaringType.Namespace.Split('.').Last();
                nameParts.Insert(0, featureName);
                break;
            }
                
            declaringType = declaringType.DeclaringType;
        }

        schemaId = string.Join("", nameParts);

        return schemaId;
    };
});

builder.Services.AddMapster();
builder.Services.RegisterMappingConfiguration();
builder.Services.AddValidatorsFromAssembly(typeof(IEndpoint).Assembly);

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.MapEndpoints();

app.Run();