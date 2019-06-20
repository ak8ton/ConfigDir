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
        static readonly Type[] arrayTypes;

        static TypeInspector()
        {
            baseAsm = typeof(Config).GetMembers(flags).Select(m => m.Module.Assembly.FullName).Distinct().ToArray();
            arrayTypes = new[] { typeof(IEnumerable<>), typeof(ICollection<>), typeof(IList<>) };
        }

        static public bool IsConfig(Type type)
        {
            return (type.IsInterface || type.IsAbstract);
        }

        static public bool IsArray(Type type)
        {
            if (type.IsGenericType)
            {
                var gtd = type.GetGenericTypeDefinition();
                foreach (var arrayType in arrayTypes)
                {
                    if (gtd == arrayType)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static public IEnumerable<PropertyInfo> GetNotImplementedProperties(Type type)
        {
            var notImplementedMethods = new List<MethodInfo>();
            var notImplementedProperties = new List<PropertyInfo>();

            var members = type.GetMembers(flags);
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

            if (notImplementedMethods.Count != settersAndGetters)
            {
                throw new Exception("Тип имеет нереализованные методы");
            }

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
