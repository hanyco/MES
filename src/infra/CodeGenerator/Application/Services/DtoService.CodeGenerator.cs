using System;
using CodeGenerator.Application.Domain;
﻿using CodeGenerator.Application.Domain;
using Library.CodeGenLib.Back;
using Library.CodeGenLib.CodeGenerators;
using Library.CodeGenLib.Models;
using Library.Coding;
using Library.Resulting;

namespace CodeGenerator.Application.Services;

public partial class DtoService
{
    public IResult<Code> GenerateCode(Dto dto)
    {
        if (dto is null)
        {
            return Result.Fail<Code>(new ArgumentNullException(nameof(dto)));
        }

        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Namespace))
        {
            return Result.Fail<Code>("invalid dto name or namespace");
        }

        try
        {
            var ns = INamespace.New(dto.Namespace);
            var cls = IClass.New(dto.Name);
            foreach (var field in dto.Properties)
            {
                var prop = IProperty.New(field.Name, TypePath.New(field.TypeFullName ?? "object"));
                _ = cls.AddProperty(prop);
            }
            _ = ns.AddType(cls);
            var codeResult = ns.GenerateCode<RoslynCodeGenerator>();
            var code = new Code(dto.Name, Languages.CSharp, codeResult.Value!, isPartial: true);
            return Result.From(codeResult, code);
        }
        catch (Exception ex)
        {
            return Result.Fail<Code>(ex);
        }
    }
}
