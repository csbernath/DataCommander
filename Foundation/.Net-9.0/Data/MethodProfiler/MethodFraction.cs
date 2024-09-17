using System;
using System.Globalization;
using System.Reflection;

namespace Foundation.Data.MethodProfiler;

internal sealed class MethodFraction(MethodBase method, string name) : MethodBase
{
    public override Type DeclaringType => method.DeclaringType;

    public static string GetKey(MethodBase method, string name)
    {
        string fullName = method.MethodHandle.Value.ToInt32().ToString("x") + "[" + name + "]";
        return fullName;
    }

    public override object[] GetCustomAttributes(bool inherit)
    {
        return method.GetCustomAttributes(inherit);
    }

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
        return method.GetCustomAttributes(attributeType, inherit);
    }

    public override bool IsDefined(Type attributeType, bool inherit)
    {
        return method.IsDefined(attributeType, inherit);
    }

    public override MemberTypes MemberType => method.MemberType;

    public override string Name => method.Name + "[" + name + "]";

    public override Type ReflectedType => method.ReflectedType;

    public override MethodAttributes Attributes => method.Attributes;

    public override MethodImplAttributes GetMethodImplementationFlags()
    {
        return method.GetMethodImplementationFlags();
    }

    public override ParameterInfo[] GetParameters()
    {
        return method.GetParameters();
    }

    public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }

    public override RuntimeMethodHandle MethodHandle => throw new InvalidOperationException();
}