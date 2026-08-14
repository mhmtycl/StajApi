using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;
using StajApi.CQRS;
using StajApi.Data;
using StajApi.Features.Auth;
using StajApi.Features.Calisanlar;
using StajApi.Features.Departmanlar;
using StajApi.Features.Projeler;
using StajApi.Models;
using StajApi.Repositories;
using StajApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IDbConnectionFactory, OracleConnectionFactory>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddScoped<
    IQueryHandler<GetCalisanlarQuery, List<Calisan>>,
    GetCalisanlarQueryHandler>();
builder.Services.AddScoped<
    ICommandHandler<LoginCommand, AuthResult>,
    LoginCommandHandler>();
builder.Services.AddScoped<
    ICommandHandler<RefreshTokenCommand, AuthResult>,
    RefreshTokenCommandHandler>();

builder.Services.AddScoped<
    ICommandHandler<CreateCalisanCommand, CalisanIslemSonucu>,
    CreateCalisanCommandHandler>();
builder.Services.AddScoped<
    ICommandHandler<UpdateCalisanCommand, CalisanIslemSonucu>,
    UpdateCalisanCommandHandler>();
builder.Services.AddScoped<
    ICommandHandler<DeleteCalisanCommand, CalisanIslemSonucu>,
    DeleteCalisanCommandHandler>();

builder.Services.AddScoped<
    IQueryHandler<GetDepartmanlarQuery, List<Departman>>,
    GetDepartmanlarQueryHandler>();
builder.Services.AddScoped<
    ICommandHandler<CreateDepartmanCommand, DepartmanIslemSonucu>,
    CreateDepartmanCommandHandler>();
builder.Services.AddScoped<
    ICommandHandler<UpdateDepartmanCommand, DepartmanIslemSonucu>,
    UpdateDepartmanCommandHandler>();
builder.Services.AddScoped<
    ICommandHandler<DeleteDepartmanCommand, DepartmanIslemSonucu>,
    DeleteDepartmanCommandHandler>();

builder.Services.AddScoped<
    IQueryHandler<GetProjelerQuery, List<Proje>>,
    GetProjelerQueryHandler>();
builder.Services.AddScoped<
    ICommandHandler<CreateProjeCommand, ProjeIslemSonucu>,
    CreateProjeCommandHandler>();
builder.Services.AddScoped<
    ICommandHandler<UpdateProjeCommand, ProjeIslemSonucu>,
    UpdateProjeCommandHandler>();
builder.Services.AddScoped<
    ICommandHandler<DeleteProjeCommand, ProjeIslemSonucu>,
    DeleteProjeCommandHandler>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "StajApi",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Token giriniz."
    });

    options.AddSecurityRequirement(document =>
{
    var schemeReference =
        new OpenApiSecuritySchemeReference("Bearer", document);

    return new OpenApiSecurityRequirement
    {
        [schemeReference] = []
    };
});
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("FrontendPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();