using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ken.Spikes.Aspects.Emit
{
    public class DecoratorProxyGenerator
    {
        /// <summary>
        /// Easy enough, right? So let’s grab System.Reflection.Emit and start working.
        /// 
        /// First we start with making a class that generates code. 
        /// I use ILGenerator and System.Reflection.Emit to create myself a new assembly with a single module that I keep in memory. 
        /// The first thing to do in this assembly is to create a Type and then add two fields to the type. 
        /// Next, we want to initialize the fields with a constructor.
        /// 
        /// Then the tricky part begins: creating all the methods. 
        /// This is done by enumerating all the methods in the decorator and creating a mapping of what needs to be implemented. 
        /// Then the implementation itself begins: first the methods of the interface are enumerated, then the methods are created as implementation of the interface. 
        /// When creating this kind of low-level code, especially the tools Reflector from Red Gate and PEVerify from Microsoft are a must-have; if you don’t have them, get them; they’re well worth the money.
        /// </summary>
        /// <param name="serviceInterface"></param>
        /// <param name="decoratorType"></param>
        /// <returns></returns>
        public Type CreateDecorator(Type serviceInterface, Type decoratorType)
        {
            string fqn = "DECO" + Guid.NewGuid().ToString().Replace("-", "");

            // Create assembly
            var assemblyName = new AssemblyName(fqn);

            AssemblyBuilder assemblyBuilder =
                AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess
                //.RunAndSave);
                .RunAndCollect);

            // Create module
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(fqn, false);
            //ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(fqn, fqn +  ".dll");

            // Create type
            TypeBuilder typeBuilder = moduleBuilder.DefineType(fqn + ".Decorator", TypeAttributes.Public);
            typeBuilder.AddInterfaceImplementation(serviceInterface);

            // Construct the proxy object
            FieldBuilder proxyField = typeBuilder.DefineField("proxy", serviceInterface, FieldAttributes.Private);
            FieldBuilder decoratorField = typeBuilder.DefineField("decorator", decoratorType, FieldAttributes.Private);

            // Create type constructor
            AddConstructor(typeBuilder, serviceInterface, proxyField, decoratorField);

            // Create a mapping:

            // Methodname -> [Usage -> [Methods + additional info]]
            Dictionary<string, Dictionary<DecoratorUsage, List<Tuple<MethodInfo, DecoratorAttribute>>>> dict =
                new Dictionary<string, Dictionary<DecoratorUsage, List<Tuple<MethodInfo, DecoratorAttribute>>>>();
            foreach (MethodInfo mi in decoratorType.GetMethods())
            {
                object[] deco = mi.GetCustomAttributes(typeof (DecoratorAttribute), true);
                foreach (DecoratorAttribute attr in deco)
                {
                    if (attr.MethodName == null)
                    {
                        foreach (MethodInfo info in serviceInterface.GetMethods())
                        {
                            Dictionary<DecoratorUsage, List<Tuple<MethodInfo, DecoratorAttribute>>> usages;
                            if (!dict.TryGetValue(info.Name, out usages))
                            {
                                usages = new Dictionary<DecoratorUsage, List<Tuple<MethodInfo, DecoratorAttribute>>>();
                                dict.Add(info.Name, usages);
                            }
                            List<Tuple<MethodInfo, DecoratorAttribute>> ll;
                            if (!usages.TryGetValue(attr.Usage, out ll))
                            {
                                ll = new List<Tuple<MethodInfo, DecoratorAttribute>>();
                                usages.Add(attr.Usage, ll);
                            }
                            ll.Add(new Tuple<MethodInfo, DecoratorAttribute>(mi, attr));
                        }
                    }
                    else
                    {
                        Dictionary<DecoratorUsage, List<Tuple<MethodInfo, DecoratorAttribute>>> usages;
                        if (!dict.TryGetValue(attr.MethodName ?? ".any", out usages))
                        {
                            usages = new Dictionary<DecoratorUsage, List<Tuple<MethodInfo, DecoratorAttribute>>>();
                            dict.Add(attr.MethodName ?? ".any", usages);
                        }
                        List<Tuple<MethodInfo, DecoratorAttribute>> ll;
                        if (!usages.TryGetValue(attr.Usage, out ll))
                        {
                            ll = new List<Tuple<MethodInfo, DecoratorAttribute>>();
                            usages.Add(attr.Usage, ll);
                        }
                        ll.Add(new Tuple<MethodInfo, DecoratorAttribute>(mi, attr));
                    }
                }
            }

            // Generate the code
            foreach (MethodInfo info in serviceInterface.GetMethods())
            {
                Dictionary<DecoratorUsage, List<Tuple<MethodInfo, DecoratorAttribute>>> usages;
                if (!dict.TryGetValue(info.Name, out usages))
                {
                    usages = new Dictionary<DecoratorUsage, List<Tuple<MethodInfo, DecoratorAttribute>>>();
                }

                // Add the method contents
                AddMember(typeBuilder, info, serviceInterface, decoratorField, proxyField, usages);
            }

            var t = typeBuilder.CreateType();
            //assemblyBuilder.Save(assemblyName.Name + ".dll"); //when testing make sure assemblybuilder and modelbuilder are save ready
            return t;
        }

        private void AddConstructor(TypeBuilder typeBuilder, Type serviceInterface, FieldBuilder proxyField, FieldBuilder decoratorField)
        {
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public | MethodAttributes.HideBySig |
                MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                CallingConventions.HasThis,
                new Type[] {serviceInterface, decoratorField.FieldType});

            // Generate the constructor IL. 
            ILGenerator gen = constructorBuilder.GetILGenerator();

            // The constructor calls the base constructor with the string[] parameter.
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call, typeof (object).GetConstructor(Type.EmptyTypes));

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Stfld, proxyField);

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_2);
            gen.Emit(OpCodes.Stfld, decoratorField);

            gen.Emit(OpCodes.Ret);
        }

        private void AddMember(TypeBuilder typeBuilder, MethodInfo info, Type serviceInterface,
                               FieldBuilder decoratorField, FieldBuilder proxyField,
                               Dictionary<DecoratorUsage, List<Tuple<MethodInfo, DecoratorAttribute>>> usages)
        {
            // Generate the method
            Type[] parameterTypes = info.GetParameters().Select((a) => (a.ParameterType)).ToArray();
            MethodBuilder mb = typeBuilder.DefineMethod(
                info.Name,
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot |
                MethodAttributes.Virtual | MethodAttributes.Final, CallingConventions.HasThis,
                info.ReturnType,
                parameterTypes);

            bool isnotvoid = info.ReturnType != typeof (void);

            // Generate the method variables
            ILGenerator gen = mb.GetILGenerator();
            var context = gen.DeclareLocal(typeof (object));
            var pars = gen.DeclareLocal(typeof (object[]));
            
            //var obj = (isnotvoid) ? gen.DeclareLocal(info.ReturnType) : gen.DeclareLocal(typeof (object)); //adjusted
            var obj = gen.DeclareLocal(typeof (object));
            
            var exc = gen.DeclareLocal(typeof(Exception));

            // Generate the code
            //    object context = null;
            //    int CS$1$0000;
            //    object[] par = new object[] { x, y };
            gen.Emit(OpCodes.Ldnull);
            gen.Emit(OpCodes.Stloc, context);
            gen.Emit(OpCodes.Ldnull);
            gen.Emit(OpCodes.Stloc, obj);

            gen.Emit(OpCodes.Ldc_I4, parameterTypes.Length);
            gen.Emit(OpCodes.Newarr, typeof (object));
            gen.Emit(OpCodes.Stloc, pars);
            for (int i = 0; i < parameterTypes.Length; ++i)
            {
                gen.Emit(OpCodes.Ldloc, pars);
                gen.Emit(OpCodes.Ldc_I4, i);
                gen.Emit(OpCodes.Ldarg, i + 1);
                if (parameterTypes[i].IsValueType)
                {
                    gen.Emit(OpCodes.Box, parameterTypes[i]);
                }
                gen.Emit(OpCodes.Stelem_Ref);
            }

            //again:
            Label again = gen.DefineLabel();
            Label endMethod = gen.DefineLabel();
            gen.MarkLabel(again);

            //    this.decorator.CallBefore("Calc", par, ref context);
            List<Tuple<MethodInfo, DecoratorAttribute>> usage;
            if (usages.TryGetValue(DecoratorUsage.Before, out usage))
            {
                foreach (var method in usage)
                {
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Ldfld, decoratorField);
                    gen.Emit(OpCodes.Ldstr, info.Name);
                    gen.Emit(OpCodes.Ldloc, pars);
                    gen.Emit(OpCodes.Ldloca, context);
                    gen.Emit(OpCodes.Callvirt, method.Item1);
                }
            }
            gen.BeginExceptionBlock();
            //    try
            //    {
            //        object obj = this.target.Calc((int) par[0], (int) par[1]);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, proxyField);
            for (int i = 0; i < parameterTypes.Length; ++i)
            {
                gen.Emit(OpCodes.Ldloc, pars);
                gen.Emit(OpCodes.Ldc_I4, i);
                gen.Emit(OpCodes.Ldelem_Ref);
                if (parameterTypes[i].IsValueType)
                {
                    gen.Emit(OpCodes.Unbox_Any, parameterTypes[i]);
                }
                else
                {
                    gen.Emit(OpCodes.Castclass, parameterTypes[i]);
                }
            }
            gen.Emit(OpCodes.Callvirt, info);
            if (isnotvoid)
            {
                if (info.ReturnType.IsValueType) gen.Emit(OpCodes.Box, info.ReturnType); //adjusted
                
                gen.Emit(OpCodes.Stloc, obj);
            }
            //        if (this.decorator.CallSuccess("Calc", par, ref obj, ref context))
            //        {
            //            goto Label_0021;
            //        }

            if (usages.TryGetValue(DecoratorUsage.Success, out usage))
            {
                foreach (var method in usage)
                {
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Ldfld, decoratorField);
                    gen.Emit(OpCodes.Ldstr, info.Name);
                    gen.Emit(OpCodes.Ldloc, pars);

                    //gen.Emit(OpCodes.Ldloc, obj); //adjusted
                    //if (isnotvoid && info.ReturnType.IsValueType) 
                    //{
                    //    gen.Emit(OpCodes.Box, info.ReturnType);
                    //}
                    gen.Emit(OpCodes.Ldloca, obj);

                    gen.Emit(OpCodes.Ldloca, context);
                    gen.Emit(OpCodes.Callvirt, method.Item1);

                    if (isnotvoid) //adjusted
                    {
                        gen.Emit(OpCodes.Ldloc, obj);
                        //gen.Emit(OpCodes.Ldc_I4, 6); //fake the result
                        //gen.Emit(OpCodes.Box, info.ReturnType); 
                        gen.Emit(OpCodes.Unbox_Any, info.ReturnType);
                        gen.Emit(OpCodes.Stloc, obj);
                    }

                    Label next = gen.DefineLabel();
                    gen.Emit(OpCodes.Brfalse, next);
                    gen.Emit(OpCodes.Leave, again);
                    gen.MarkLabel(next);
                }
            }
            gen.Emit(OpCodes.Leave, endMethod);
            //    }


            gen.BeginCatchBlock(typeof (Exception));
            //    catch (Exception ex)
            //    {

            gen.Emit(OpCodes.Stloc, exc);
            if (usages.TryGetValue(DecoratorUsage.OnException, out usage))
            {
                //        if (this.decorator.CallException("Calc", par, ex, ref context))
                //        {
                //            goto Label_0021;
                //        }

                foreach (var method in usage)
                {
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Ldfld, decoratorField);
                    gen.Emit(OpCodes.Ldstr, info.Name);
                    gen.Emit(OpCodes.Ldloc, pars);
                    gen.Emit(OpCodes.Ldloc, exc);
                    gen.Emit(OpCodes.Ldloca, context);
                    gen.Emit(OpCodes.Callvirt, method.Item1);

                    Label next = gen.DefineLabel();
                    gen.Emit(OpCodes.Brfalse, next);
                    gen.Emit(OpCodes.Leave, again);
                    gen.MarkLabel(next);
                }
            }

            //        throw;
            gen.Emit(OpCodes.Rethrow);
            //    }


            gen.BeginFinallyBlock();
            if (usages.TryGetValue(DecoratorUsage.After, out usage))
            {
                //    finally
                //    {
                //        this.decorator.CallAfter1("Calc", par, ref context);
                //    }
                foreach (var method in usage)
                {
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Ldfld, decoratorField);
                    gen.Emit(OpCodes.Ldstr, info.Name);
                    gen.Emit(OpCodes.Ldloc, pars);
                    gen.Emit(OpCodes.Ldloca, context);
                    gen.Emit(OpCodes.Callvirt, method.Item1);
                }
            }
            gen.EndExceptionBlock();

            gen.MarkLabel(endMethod);
            if (isnotvoid)
            {
                gen.Emit(OpCodes.Ldloc, obj);
            }
            gen.Emit(OpCodes.Ret);
        }
    }
}
