using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using static System.Reflection.MethodAttributes;
using ConfigDir.Internal;


namespace ConfigDir
{
    public abstract partial class Config
    {
        static readonly Type BaseConfigType;
        static readonly Type[] GetterArgs;
        static readonly Type[] SetterArgs;
        static readonly MethodInfo Getter;
        static readonly MethodInfo Setter;

        const MethodAttributes getSetAttr = Public | SpecialName | HideBySig | Virtual | ReuseSlot;

        static int counter;
        static readonly string unicName;
        static readonly Dictionary<Type, Tuple<Type, string[]>> TypeDictionary;

        static Config()
        {
            var getterName = nameof(Config.GetValue);
            var setterName = nameof(Config.SetValue);

            BaseConfigType = typeof(Config);
            Getter = BaseConfigType.GetMethod(getterName);
            Setter = BaseConfigType.GetMethod(setterName);

            GetterArgs = Getter.GetParameters().Select(p => p.ParameterType).ToArray();
            SetterArgs = Setter.GetParameters().Select(p => p.ParameterType).ToArray();

            counter = 0;
            unicName = string.Join("", Guid.NewGuid().ToByteArray().Select(c => c.ToString("X")));
            TypeDictionary = new Dictionary<Type, Tuple<Type, string[]>>();
        }

        TConfig CreateSubConfig<TConfig>(string key)
        {
            var cfgType = typeof(TConfig);
            if (!typeof(IConfig).IsAssignableFrom(cfgType))
            {
                throw new Exception(typeof(TConfig).FullName);
            }

            var config = CreateDynamicInstance<TConfig>();
            ((Config)config).SetParent(this, key);
            return (TConfig)config;
        }

        static object CreateDynamicInstance<TConfig>()
        {
            var type = typeof(TConfig);

            if (!TypeDictionary.ContainsKey(type))
            {
                var properties = TypeInspector.GetNotImplementedProperties<TConfig>();
                TypeDictionary[type] = new Tuple<Type, string[]>
                    (
                        GetDynamicType(type, properties),
                        properties.Select(p => p.Name).ToArray()
                    );
            }

            var c = TypeDictionary[type];
            var instance = Activator.CreateInstance(c.Item1);
            ((Config)instance).Keys = c.Item2;
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
            il.Emit(OpCodes.Ldstr, pi.Name);
            il.Emit(OpCodes.Ldarg_1);
            il.EmitCall(OpCodes.Call, Setter, SetterArgs);
            il.Emit(OpCodes.Ret);

            tb.DefineMethodOverride(setter, virtSetter);
        }
    }
}

