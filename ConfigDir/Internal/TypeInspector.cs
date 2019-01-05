using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace ConfigDir.Internal
{
    static class TypeInspector
    {
        const BindingFlags flags = BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        static readonly string[] baseAsm;
        static readonly Type arrayType;

        static TypeInspector()
        {
            baseAsm = typeof(Config).GetMembers(flags).Select(m => m.Module.Assembly.FullName).Distinct().ToArray();
            arrayType = typeof(IEnumerable<>);
        }

        static public TypeCategory GetTypeCategory(Type type)
        {
            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == arrayType)
                {
                    return TypeCategory.Array;
                }
            }

            if (type.IsInterface || type.IsAbstract)
            {
                return TypeCategory.Config;
            }

            return TypeCategory.Value;
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
                        notImplementedProperties.Add(propertyInfo);
                    }
                }
                else if (mi is ConstructorInfo c)
                {
                    if (c.GetParameters().Length == 0)
                    {
                        continue;
                    }
                    else
                    {
                        throw new NotImplementedException("Не должен иметь конструкторов с параметрами");
                    }
                }
                else
                {
                    throw new NotImplementedException($"Члены типа {mi.MemberType} не поддерживаются");
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
    }
}
