using ConfigDir.Internal;
using ConfigDir.Exceptions;
using System;
using System.Reflection;


namespace ConfigDir.Data
{
    public partial class Finder
    {
        private object TryChangeType(ValueOrSource value, Type type)
        {
            try
            {
                return ChangeType(value.Value, type);
            }
            catch (Exception ex)
            {
                throw new ValueTypeException("Type conversion error", value, type, ex);
            }
        }

        private object ChangeType(object value, Type type)
        {
            if (type.IsAssignableFrom(value.GetType()))
            {
                return value;
            }

            Exception convertException = null;

            if (value is IConvertible)
            {
                try
                {
                    return Convert.ChangeType(value, type);
                }
                catch (Exception ex)
                {
                    convertException = ex;
                }
            }

            // class
            if (type.IsClass)
            {
                if (type.GetConstructor(Type.EmptyTypes) != null)
                {
                    return CreatePropsObject(type);
                }

                foreach (var constructor in type.GetConstructors())
                {
                    var p = constructor.GetParameters();
                    if (p.Length == 1)
                    {
                        var parameter = ChangeType(value, p[0].ParameterType);
                        return Activator.CreateInstance(type, new object[] { parameter });
                    }
                }
            }

            throw new Exception($"Type {type.FullName} not supported", convertException);
        }

        private object CreatePropsObject(Type type)
        {
            var obj = Activator.CreateInstance(type);

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.CanWrite)
                {
                    property.SetValue(obj, FindValue(new KeyOrIndex(property.Name), property.PropertyType, false));
                }
            }

            return obj;
        }
    }
}
