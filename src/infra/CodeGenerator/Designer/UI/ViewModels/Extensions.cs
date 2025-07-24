using DataLib.SqlServer;
﻿namespace CodeGenerator.Designer.UI.ViewModels;

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
            Properties = [.. @this.Properties.Select(p => new Property
            {
                Comment = p.Comment,
                DbObjectId = p.DbObjectId,
                DtoId = p.DtoId,
                Guid = p.Guid,
                HasGetter = p.HasGetter,
                HasSetter = p.HasSetter,
                Id = p.Id,
                IsList = p.IsList,
                IsNullable = p.IsNullable,
                Name = p.Name,
                ParentEntityId = p.ParentEntityId,
                PropertyType = p.PropertyType,
                TypeFullName = SqlTypeUtils.ToNetTypeName(p.TypeFullName ?? string.Empty),
            })],
        };
    }
}
