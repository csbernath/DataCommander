using System;
using System.Globalization;
using System.Reflection;

namespace Foundation.Data.MethodProfiler
{
    internal sealed class MethodFraction : MethodBase
    {
        private readonly MethodBase _method;
        private readonly string _name;

        public MethodFraction(MethodBase method, string name)
        {
            _method = method;
            _name = name;
        }

        public override Type DeclaringType => _method.DeclaringType;

        public static string GetKey(MethodBase method, string name)
        {
            var fullName = method.MethodHandle.Value.ToInt32().ToString("x") + "[" + name + "]";
            return fullName;
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _method.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _method.GetCustomAttributes(attributeType, inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _method.IsDefined(attributeType, inherit);
        }

        public override MemberTypes MemberType => _method.MemberType;

        public override string Name => _method.Name + "[" + _name + "]";

        public override Type ReflectedType => _method.ReflectedType;

        public override MethodAttributes Attributes => _method.Attributes;

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            return _method.GetMethodImplementationFlags();
        }

        public override ParameterInfo[] GetParameters()
        {
            return _method.GetParameters();
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }

        public override RuntimeMethodHandle MethodHandle => throw new InvalidOperationException();
    }
}
