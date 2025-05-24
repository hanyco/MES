namespace Library.CodeGenLib.Back;

/// <summary>
/// Defines various access levels for class members, including None, Private, Protected, Internal, Public,
/// InternalProtected, and ReadOnly.
///Each access level specifies the visibility and accessibility of class members.
/// </summary>
[Flags]
public enum AccessModifier
{
    /// <summary>
    /// Represents a value of zero, typically used to indicate the absence of a value or a null state.
    /// </summary>
    None = 0,
    /// <summary>
    /// Represents a private access level, typically indicating that a member is accessible only within its own class.
    /// </summary>
    Private = 1,
    /// <summary>
    /// Represents a protected access level, typically allowing access within its own class and by derived class
    /// instances. It is assigned a value of 2.
    /// </summary>
    Protected = 2,
    /// <summary>
    /// Represents an internal access level with a value of 4. Used to indicate that a member is accessible only within
    /// its own assembly.
    /// </summary>
    Internal = 4,
    /// <summary>
    /// Represents a public access level with a value of 8.
    /// </summary>
    Public = 8,
    /// <summary>
    /// Represents a combination of protected and internal access modifiers. It allows access within the same assembly
    /// and derived classes.
    /// </summary>
    InternalProtected = Protected | Internal,
    /// <summary>
    /// Represents a read-only access level, typically used to indicate that data cannot be modified. It is assigned a
    /// value of 32.
    /// </summary>
    ReadOnly = 32
}
/// <summary>
/// Defines flags for inheritance modifiers in a class or method. Each flag represents a specific behavior related to
/// inheritance.
/// </summary>
[Flags]
public enum InheritanceModifier
{
    None = 0,
    Virtual = 1,
    Abstract = 2,
    Override = 4,
    /// <summary>
    /// Represents a new state or option, assigned the value of 8.
    /// </summary>
    New = 8,
    /// <summary>
    /// Represents a sealed access modifier with a value of 16. It restricts the inheritance of classes.
    /// </summary>
    Sealed = 16,
    /// <summary>
    /// Represents a static value, set to 32. It is likely used as a constant in the context of the code.
    /// </summary>
    Static = 32,
    /// <summary>
    /// Represents a flag indicating that a member is partial. It is typically used in contexts where a member's
    /// implementation is incomplete or only partially defined.
    /// </summary>
    Partial = 64,
    /// <summary>
    /// Defines a constant value of 128. This value is immutable and can be used throughout the code.
    /// </summary>
    Const = 128
}