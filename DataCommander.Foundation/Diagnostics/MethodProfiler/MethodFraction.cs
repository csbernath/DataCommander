using System;
using System.Globalization;
using System.Reflection;

namespace Foundation.Diagnostics.MethodProfiler
{
    internal sealed class MethodFraction : MethodBase
    {
        private readonly MethodBase _method;
        private readonly string _name;

        public MethodFraction(MethodBase method, string name)
        {
            this._method = method;
            this._name = name;
        }

        public override Type DeclaringType => this._method.DeclaringType;

        public static string GetKey(MethodBase method, string name)
        {
            var fullName = method.MethodHandle.Value.ToInt32().ToString("x") + "[" + name + "]";
            return fullName;
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return this._method.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return this._method.GetCustomAttributes(attributeType, inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return this._method.IsDefined(attributeType, inherit);
        }

        public override MemberTypes MemberType => this._method.MemberType;

        public override string Name => this._method.Name + "[" + this._name + "]";

        public override Type ReflectedType => this._method.ReflectedType;

        public override MethodAttributes Attributes => this._method.Attributes;

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            return this._method.GetMethodImplementationFlags();
        }

        public override ParameterInfo[] GetParameters()
        {
            return this._method.GetParameters();
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }

        public override RuntimeMethodHandle MethodHandle => throw new InvalidOperationException();
    }
}
