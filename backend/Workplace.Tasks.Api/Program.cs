using Workplace.Tasks.Api.Authorization;
using Workplace.Tasks.Api.Data;
using Workplace.Tasks.Api.Repositories;
using Workplace.Tasks.Api.Services;
using Workplace.Tasks.Api.Middlewares;
using Workplace.Tasks.Api.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Microsoft.EntityFrameworkCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy
            .WithOrigins("http://localhost:4200") // front
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "BIM Workplace Tasks API - teste tecnico",
            Version = "v1",
            Description = "API para gerenciamento de tarefas com autenticação JWT e RBAC"
        });

        var jwtSecurityScheme = new OpenApiSecurityScheme
        {
            Scheme = "bearer",
            BearerFormat = "JWT",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Description = @"API de gerenciamento de tarefas com autenticação JWT e RBAC.<br/><br/>
            <b>Usuários padrão (seed):</b><br/>
             <b>Admin:</b> admin@example.com / Password123!<br/>
             <b>Manager:</b> manager@example.com / Password123!<br/>
             <b>Member:</b> member@example.com / Password123!",

            Reference = new OpenApiReference
            {
                Id = JwtBearerDefaults.AuthenticationScheme,
                Type = ReferenceType.SecurityScheme
            }
        };

        options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                jwtSecurityScheme,
                Array.Empty<string>()
            }
        });
    });



//conexão banco
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));


// Injeção de depencia
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IAuthorizationHandler, OwnsTaskHandler>();

// Configuração JWT
var jwtKey = builder.Configuration["Jwt:Key"];
var keyBytes = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("CanManageAll", policy =>
//        policy.RequireRole("Admin"));

//    options.AddPolicy("CanManageTasks", policy =>
//        policy.RequireRole("Admin", "Manager"));

//    options.AddPolicy("CanDeleteOwn", policy =>
//        policy.Requirements.Add(new OwnsTaskRequirement()));

//    options.AddPolicy("CanEditOwn", policy =>
//        policy.Requirements.Add(new OwnsTaskRequirement()));
//});
builder.Services.AddAuthorization(options =>
{
    // adm tudo liberado
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));

    // mamager, exceto deletar alheios
    options.AddPolicy("ManagerPolicy", policy =>
        policy.RequireRole("Admin", "Manager"));

    // member crud apenas próprios
    options.AddPolicy("MemberPolicy", policy =>
        policy.RequireRole("Admin", "Manager", "Member"));

    //  usada junto com outros policies
    options.AddPolicy("OwnsTask", policy =>
        policy.Requirements.Add(new OwnsTaskRequirement()));
});

//Melhoria: alterar de RBAC (Controle de Acesso Baseado em Função) e para ABAC (Controle de Acesso Baseado em Atributo) 
//RBAC é binario e estatico — ele responde apenas tem ou nao tem.
//e na pratica e em projetos escalaveis as autorizações dependem do contexto

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated(); // Cria DB se não existir
    DbInitializer.Seed(context);
}

//tratar erros
app.UseMiddleware<ModelStateValidationMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

// configure the http request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Workplace Tasks API v1");
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        options.DisplayRequestDuration();
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseModelStateValidation();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
