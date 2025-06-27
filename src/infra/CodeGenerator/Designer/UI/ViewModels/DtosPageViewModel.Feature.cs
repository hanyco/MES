using System;
using CodeGenerator.Application.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CodeGenerator.Designer.UI.ViewModels;

[Feature("مدیریت DTO")]
public partial class DtosPageViewModel
{
    public DtosPageViewModel()
        : this(new DtoService(new SqlConnection(
            new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build()
                .GetConnectionString("DefaultConnection")!)))
    {
    }
}
