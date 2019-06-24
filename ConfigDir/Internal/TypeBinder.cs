using ConfigDir.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using static System.Reflection.MethodAttributes;


namespace ConfigDir.Internal
{
    static class TypeBinder
    {
        static readonly Type BaseConfigType;
        static readonly Type[] GetterArgs;
        static readonly Type[] SetterArgs;
        static readonly MethodInfo Getter;
        static readonly MethodInfo Setter;
        static readonly MethodInfo DataGetter;

        const MethodAttributes getSetAttr = Public | SpecialName | HideBySig | Virtual | ReuseSlot;

        static int counter;
        static readonly string unicName;
        static readonly Dictionary<Type, Tuple<Type, string[]>> TypeDictionary;

        static TypeBinder()
        {
            var getterName = nameof(Finder.GetValue);
            var setterName = nameof(Finder.SetValue);
            var dataGetterName = nameof(Config.Finder);

            BaseConfigType = typeof(Config);

            var data = typeof(Finder);
            Getter = data.GetMethod(getterName);
            Setter = data.GetMethod(setterName);
            DataGetter = BaseConfigType.GetProperty(dataGetterName).GetGetMethod();

            GetterArgs = Getter.GetParameters().Select(p => p.ParameterType).ToArray();
            SetterArgs = Setter.GetParameters().Select(p => p.ParameterType).ToArray();

            counter = 0;
            unicName = string.Join("", Guid.NewGuid().ToByteArray().Select(c => c.ToString("X")));
            TypeDictionary = new Dictionary<Type, Tuple<Type, string[]>>();
        }

        public static object CreateDynamicInstance(string key, Type type, Finder parent)
        {
            if (!TypeDictionary.ContainsKey(type))
            {
                var properties = TypeInspector.GetNotImplementedProperties(type);
                TypeDictionary[type] = new Tuple<Type, string[]>
                (
                    GetDynamicType(type, properties),
                    properties.Select(p => p.Name).ToArray()
                );
            }

            var c = TypeDictionary[type];
            var instance = Activator.CreateInstance(c.Item1);
            var finder = new Finder(type, key, c.Item2, parent);

            if (parent != null)
            {
                finder.Extend(new ParentSource(finder));
            }
            
            ((Config)instance).SetFinder(finder);
            return instance;
        }

        static Type GetDynamicType(Type baseType, IEnumerable<PropertyInfo> properties)
        {
            var parent = BaseConfigType;
            var interfaces = new Type[0];

            if (baseType.IsInterface)
            {
                interfaces = new Type[] { baseType };
            }
            else
            {
                parent = baseType;
            }

            if (!BaseConfigType.IsAssignableFrom(parent))
            {
                throw new Exception("Класс конфига должен быть унаследован от " + BaseConfigType.FullName);
            }

            var tb = GetTypeBuilder(parent, interfaces);

            foreach (var pi in properties)
            {
                EmitGetMethod(tb, pi);

                if (pi.CanWrite)
                {
                    EmitSetMethod(tb, pi);
                }
            }

            return tb.CreateType();
        }

        static TypeBuilder GetTypeBuilder(Type parent, Type[] interfaces)
        {
            var name = $"Config_{unicName}_{counter++}";
            var an = new AssemblyName(name);
            var ab = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            var mb = ab.DefineDynamicModule(name);
            var tb = mb.DefineType("ConfigType", TypeAttributes.Public, parent, interfaces);
            return tb;
        }

        static void EmitGetMethod(TypeBuilder tb, PropertyInfo pi)
        {
            var virtGetter = pi.GetGetMethod();

            var getAccessor = tb.DefineMethod(
                virtGetter.Name,
                getSetAttr,
                pi.PropertyType,
                Type.EmptyTypes);

            var il = getAccessor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.EmitCall(OpCodes.Call, DataGetter, Type.EmptyTypes);
            il.Emit(OpCodes.Ldstr, pi.Name);
            il.EmitCall(OpCodes.Call, Getter.MakeGenericMethod(pi.PropertyType), GetterArgs);
            il.Emit(OpCodes.Ret);

            tb.DefineMethodOverride(getAccessor, virtGetter);
        }

        static void EmitSetMethod(TypeBuilder tb, PropertyInfo pi)
        {
            var virtSetter = pi.GetSetMethod();

            var setter = tb.DefineMethod(
                virtSetter.Name,
                getSetAttr,
                null,
                new Type[] { pi.PropertyType });

            var il = setter.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.EmitCall(OpCodes.Call, DataGetter, Type.EmptyTypes);
            il.Emit(OpCodes.Ldstr, pi.Name);
            il.Emit(OpCodes.Ldarg_1);
            il.EmitCall(OpCodes.Call, Setter, SetterArgs);
            il.Emit(OpCodes.Ret);

            tb.DefineMethodOverride(setter, virtSetter);
        }
    }
}

