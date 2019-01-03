using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using static System.Reflection.MethodAttributes;
using static ConfigDir.ConfigBase;


namespace ConfigDir.Internal
{
    static class TypeBinder
    {
        const MethodAttributes getSetAttr = Public | SpecialName | HideBySig | Virtual | ReuseSlot;

        static int counter;
        static readonly string unicName;
        static readonly Dictionary<Type, Tuple<Type, string[]>> cash;

        static TypeBinder()
        {
            counter = 0;
            unicName = string.Join("", Guid.NewGuid().ToByteArray().Select(c => c.ToString("X")));
            cash = new Dictionary<Type, Tuple<Type, string[]>>();
        }

        public static TConfig CreateSubConfig<TConfig>(IFinder finder)
        {
            var cfgType = typeof(TConfig);
            if (!typeof(IConfigBase).IsAssignableFrom(cfgType))
            {
                throw new Exception(typeof(TConfig).FullName);
            }

            var m = typeof(TypeBinder).GetMethod(nameof(CreateDynamicInstance));
            var gm = m.MakeGenericMethod(new[] { cfgType });
            return (TConfig)gm.Invoke(null, new object[] { finder });
        }

        public static TConfig CreateDynamicInstance<TConfig>(IFinder finder) where TConfig : IConfigBase
        {
            var type = typeof(TConfig);

            if (!cash.ContainsKey(type))
            {
                var properties = TypeInspector.GetNotImplementedProperties<TConfig>();
                cash[type] = new Tuple<Type, string[]>
                (
                    GetDynamicType(type, properties),
                    properties.Select(p => p.Name).ToArray()
                );
            }

            var c = cash[type];
            var instance = (TConfig)Activator.CreateInstance(c.Item1);

            var props = new InstancePropertyes
            {
                KeysSet = c.Item2.ToHashSet(),
                Finder = finder,
            };

            Init(instance, props);

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
            // var propAttr = PropertyAttributes.HasDefault;

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
