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
        static readonly Dictionary<Type, Type> typeDictionary;

        static TypeBinder()
        {
            var getterName = nameof(Finder.GetValue);
            var setterName = nameof(Finder.SetValue);
            var dataGetterName = nameof(Config.Finder);

            BaseConfigType = typeof(Config);

            var data = typeof(Finder);

            GetterArgs = new[] { typeof(string) };
            SetterArgs = new[] { typeof(string), typeof(object) };

            Getter = data.GetMethod(getterName, GetterArgs);
            Setter = data.GetMethod(setterName, SetterArgs);
            DataGetter = BaseConfigType.GetProperty(dataGetterName).GetGetMethod();

            counter = 0;
            unicName = string.Join("", Guid.NewGuid().ToByteArray().Select(c => c.ToString("X")));
            typeDictionary = new Dictionary<Type, Type>();
        }

        public static object CreateDynamicInstance(KeyOrIndex keyOrIndex, Type type, Finder parent)
        {
            if (!typeDictionary.ContainsKey(type))
            {
                var properties = TypeInspector.GetNotImplementedProperties(type);
                typeDictionary[type] = GetDynamicType(type, properties);
            }

            var t = typeDictionary[type];
            var instance = Activator.CreateInstance(t);
            var finder = new Finder(type, keyOrIndex, parent);

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

