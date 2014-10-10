namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Reflection;

    internal sealed class MethodFraction : MethodBase
    {
        private readonly MethodBase method;
        private readonly String name;

        public MethodFraction(MethodBase method, String name)
        {
            this.method = method;
            this.name = name;
        }

        public override Type DeclaringType
        {
            get
            {
                return this.method.DeclaringType;
            }
        }

        public static String GetKey(MethodBase method, String name)
        {
            String fullName = method.MethodHandle.Value.ToInt32().ToString("x") + "[" + name + "]";
            return fullName;
        }

        public override Object[] GetCustomAttributes(Boolean inherit)
        {
            return this.method.GetCustomAttributes(inherit);
        }

        public override Object[] GetCustomAttributes(Type attributeType, Boolean inherit)
        {
            return this.method.GetCustomAttributes(attributeType, inherit);
        }

        public override Boolean IsDefined(Type attributeType, Boolean inherit)
        {
            return this.method.IsDefined(attributeType, inherit);
        }

        public override MemberTypes MemberType
        {
            get
            {
                return this.method.MemberType;
            }
        }

        public override String Name
        {
            get
            {
                return this.method.Name + "[" + this.name + "]";
            }
        }

        public override Type ReflectedType
        {
            get
            {
                return this.method.ReflectedType;
            }
        }

        public override MethodAttributes Attributes
        {
            get
            {
                return this.method.Attributes;
            }
        }

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            return this.method.GetMethodImplementationFlags();
        }

        public override ParameterInfo[] GetParameters()
        {
            return this.method.GetParameters();
        }

        public override Object Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, System.Globalization.CultureInfo culture)
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
