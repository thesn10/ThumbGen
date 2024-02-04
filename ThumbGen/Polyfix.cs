#if !NET5_0_OR_GREATER
// ref: https://stackoverflow.com/questions/62648189/testing-c-sharp-9-0-in-vs2019-cs0518-isexternalinit-is-not-defined-or-imported
// ref: https://github.com/dotnet/core/issues/8016

using System.ComponentModel;

namespace System.Runtime.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
internal static class IsExternalInit { }

internal class RequiredMemberAttribute : Attribute { }
internal class CompilerFeatureRequiredAttribute : Attribute
{
    internal CompilerFeatureRequiredAttribute(string name) { }
}
#endif