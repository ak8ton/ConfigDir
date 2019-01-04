using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace ConfigDir.Internal
{
    static class TypeInspector
    {
        const BindingFlags flags = BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance;
        static readonly string[] baseAsm;
        static readonly Type arrayType;
        static readonly Type[] primitiveTypes;

        static TypeInspector()
        {
            baseAsm = typeof(Config).GetMembers(flags).Select(m => m.Module.Assembly.FullName).Distinct().ToArray();
            arrayType = typeof(IEnumerable<>);

            primitiveTypes = new Type[] {
                typeof(bool),
                typeof(byte),
                typeof(sbyte),
                typeof(char),
                typeof(decimal),
                typeof(double),
                typeof(float),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(short),
                typeof(ushort),
                typeof(string)
            };
        }

        static public TypeCategory GetTypeCategory(Type type)
        {
            if (type != null)
            {
                if (IsPrimitive(type)) return TypeCategory.Primitive;
                if (IsConfig(type)) return TypeCategory.Config;
                if (IsArray(type)) return TypeCategory.Array;
            }
            return TypeCategory.None;
        }

        static public bool IsSupportedType(Type type)
        {
            return GetTypeCategory(type) != TypeCategory.None;
        }

        static public IEnumerable<PropertyInfo> GetNotImplementedProperties<TConfig>()
        {
            var notImplementedMethods = new List<MethodInfo>();
            var notImplementedProperties = new List<PropertyInfo>();

            var members = typeof(TConfig).GetMembers(flags);
            foreach (var mi in members)
            {
                if (IsInherited(mi))
                {
                    continue;
                }
                else if (mi is MethodInfo methodInfo)
                {
                    if (methodInfo.GetMethodBody() == null)
                    {
                        notImplementedMethods.Add(methodInfo);
                    }
                }
                else if (mi is PropertyInfo propertyInfo)
                {
                    if (NotImplemented(propertyInfo))
                    {
                        CheckPropertyType(propertyInfo);
                        notImplementedProperties.Add(propertyInfo);
                    }
                }
                else
                {
                    throw new NotImplementedException($"Члены типа {mi.MemberType} пока не поддерживаются");
                }
            }

            var settersAndGetters = 0;
            foreach (var pi in notImplementedProperties)
            {
                if (pi.CanRead) settersAndGetters++;
                if (pi.CanWrite) settersAndGetters++;
            }

           // if (notImplementedMethods.Count != settersAndGetters)
           // {
           //     throw new ArgumentException("Тип имеет нереализованные методы");
           // }

            return notImplementedProperties;
        }

        static bool IsInherited(MemberInfo mi)
        {
            return baseAsm.Contains(mi.Module.Assembly.FullName);
        }

        static bool IsPrimitive(Type type)
        {
            foreach (var primitiveType in primitiveTypes)
            {
                if (type == primitiveType) return true;
            }
            return false;
        }

        static bool IsConfig(Type type)
        {
            return typeof(IConfig).IsAssignableFrom(type);
        }

        static bool IsArray(Type type)
        {
            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == arrayType)
                {
                    var item = type.GenericTypeArguments.FirstOrDefault();
                    return IsSupportedType(item);
                }
            }
            return false;
        }

        static bool NotImplemented(PropertyInfo propertyInfo)
        {
            if (propertyInfo.CanRead)
            {
                if (propertyInfo.GetGetMethod().GetMethodBody() == null)
                {
                    if (propertyInfo.GetSetMethod()?.GetMethodBody() != null)
                    {
                        throw new Exception();
                    }
                    return true;
                }
            }

            if (propertyInfo.CanWrite)
            {
                if (propertyInfo.GetSetMethod() == null)
                {
                    throw new Exception();
                }
            }
            return false;
        }

        static void CheckPropertyType(PropertyInfo pi)
        {
            var type = pi.PropertyType;
            if (IsSupportedType(type)) return;
            throw new Exception("Тип {type} не поддерживается");
        }
    }
}
