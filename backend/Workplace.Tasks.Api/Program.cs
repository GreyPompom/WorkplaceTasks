using Workplace.Tasks.Api.Data;
using Workplace.Tasks.Api.Repositories;
using Workplace.Tasks.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Workplace.Tasks.Api.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Workplace.Tasks.Api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Workplace.Tasks.Api.Authorization;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
        policy.RequireRole("Member"));

    //  usada junto com outros policies
    options.AddPolicy("OwnsTask", policy =>
        policy.Requirements.Add(new OwnsTaskRequirement()));
});

//Melhoria: alterar de RBAC (Controle de Acesso Baseado em Função) e para ABAC (Controle de Acesso Baseado em Atributo) 
//RBAC é binario e estatico — ele responde apenas tem ou nao tem.
//e na pratica e em projetos escalaveis as autorizações dependem do contexto

var app = builder.Build();

//tratar erros
app.UseMiddleware<ModelStateValidationMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

// configure the http request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseModelStateValidation();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
