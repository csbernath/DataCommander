namespace DataCommander.Foundation.Diagnostics.MethodProfiler
{
    using System;
    using System.Globalization;
    using System.Reflection;

    internal sealed class MethodFraction : MethodBase
    {
        private readonly MethodBase method;
        private readonly string name;

        public MethodFraction(MethodBase method, string name)
        {
            this.method = method;
            this.name = name;
        }

        public override Type DeclaringType => this.method.DeclaringType;

        public static string GetKey(MethodBase method, string name)
        {
            string fullName = method.MethodHandle.Value.ToInt32().ToString("x") + "[" + name + "]";
            return fullName;
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return this.method.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return this.method.GetCustomAttributes(attributeType, inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return this.method.IsDefined(attributeType, inherit);
        }

        public override MemberTypes MemberType => this.method.MemberType;

        public override string Name => this.method.Name + "[" + this.name + "]";

        public override Type ReflectedType => this.method.ReflectedType;

        public override MethodAttributes Attributes => this.method.Attributes;

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            return this.method.GetMethodImplementationFlags();
        }

        public override ParameterInfo[] GetParameters()
        {
            return this.method.GetParameters();
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }

        public override RuntimeMethodHandle MethodHandle
        {
            get
            {
                throw new InvalidOperationException();
            }
        }
    }
}
