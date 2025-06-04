
using Microsoft.CodeAnalysis.CSharp;

namespace Library.CodeGenLib.Helpers;

internal static class GeneratorHelper
{
    internal static IEnumerable<SyntaxKind> ToModifiers(AccessModifier? access, InheritanceModifier? inheritance)
    {
        var result = new List<SyntaxKind>();

        var modifier = access == null ? AccessModifier.None : access.Value;
        updateModifier(modifier, result, AccessModifier.Public, SyntaxKind.PublicKeyword);
        updateModifier(modifier, result, AccessModifier.Private, SyntaxKind.PrivateKeyword);
        updateModifier(modifier, result, AccessModifier.Internal, SyntaxKind.InternalKeyword);
        updateModifier(modifier, result, AccessModifier.Protected, SyntaxKind.ProtectedKeyword);
        updateModifier(modifier, result, AccessModifier.ReadOnly, SyntaxKind.ReadOnlyKeyword);

        var inherit = inheritance == null ? InheritanceModifier.None : inheritance.Value;
        updateInheritance(inherit, result, InheritanceModifier.New, SyntaxKind.NewKeyword);
        updateInheritance(inherit, result, InheritanceModifier.Static, SyntaxKind.StaticKeyword);
        updateInheritance(inherit, result, InheritanceModifier.Sealed, SyntaxKind.SealedKeyword);
        updateInheritance(inherit, result, InheritanceModifier.Override, SyntaxKind.OverrideKeyword);
        updateInheritance(inherit, result, InheritanceModifier.Abstract, SyntaxKind.AbstractKeyword);
        updateInheritance(inherit, result, InheritanceModifier.Const, SyntaxKind.ConstKeyword);
        updateInheritance(inherit, result, InheritanceModifier.Virtual, SyntaxKind.VirtualKeyword);
        updateInheritance(inherit, result, InheritanceModifier.Partial, SyntaxKind.PartialKeyword);

        return result;

        static void updateModifier(AccessModifier access, List<SyntaxKind> result, AccessModifier referral, SyntaxKind kind)
        {
            if (access.Contains(referral))
            {
                result.Add(kind);
            }
        }

        static void updateInheritance(InheritanceModifier inheritance, List<SyntaxKind> result, InheritanceModifier referral, SyntaxKind kind)
        {
            if (inheritance.Contains(referral))
            {
                result.Add(kind);
            }
        }
    }
}