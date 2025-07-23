namespace CodeGenerator.Designer.UI.ViewModels;

public static class Extensions
{
    extension(DtoViewModel? @this)
    {
        public Dto? ToEntity() => @this is null ? null : new()
        {
            Id = @this.Id ?? 0,
            Name = @this.Name ?? string.Empty,
            Namespace = @this.NameSpace,
            Comment = @this.Comments,
            IsList = @this.IsList,
            IsParamsDto = @this.IsParams,
            IsResultDto = @this.IsResult,
            IsViewModel = @this.IsViewModel,
            DbObjectId = @this.ObjectId.ToString(),
            ModuleId = @this.Module?.Id,
            Properties = [.. @this.Properties],
        };
    }
}