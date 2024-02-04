#if !NET5_0_OR_GREATER || !NET6_0_OR_GREATER
using System.ComponentModel;

namespace System.Runtime.CompilerServices;
#endif
#if !NET5_0_OR_GREATER
// ref: https://stackoverflow.com/questions/62648189/testing-c-sharp-9-0-in-vs2019-cs0518-isexternalinit-is-not-defined-or-imported

[EditorBrowsable(EditorBrowsableState.Never)]
internal static class IsExternalInit { }
#endif
#if !NET6_0_OR_GREATER
// ref: https://github.com/dotnet/core/issues/8016

#pragma warning disable IDE0079
#pragma warning disable CA1018
internal class RequiredMemberAttribute : Attribute { }
internal class CompilerFeatureRequiredAttribute : Attribute
{
#pragma warning disable IDE0060
    internal CompilerFeatureRequiredAttribute(string name) { }
#pragma warning restore IDE0060
}
#pragma warning restore CA1018
#pragma warning restore IDE0079
#endif