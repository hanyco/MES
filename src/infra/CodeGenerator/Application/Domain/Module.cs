using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGenerator.Application.Domain;
public class Module
{
    public long Id { get; set; }
    public Guid Guid { get; set; }
    public string Name { get; set; } = string.Empty;
    public long ParentId { get; set; }
}
