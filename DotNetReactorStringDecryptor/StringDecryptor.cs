using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotNetReactorStringDecryptor
{
    internal static class StringDecryptor
    {
        private static MethodInfo GetStringMethod(int MethodToken, Assembly asm)
        {
            MethodInfo methodInfo = (MethodInfo)asm.ManifestModule.ResolveMethod(MethodToken);
            MethodInfo res;
            if (methodInfo.IsGenericMethodDefinition || methodInfo.IsGenericMethod)
            {
                res = methodInfo.MakeGenericMethod(new Type[]
                {
                    typeof(string)
                });
            }
            else
            {
                res = methodInfo;
            }
            return res;
        }
        public static int Execute()
        {
            int count = 0;
            foreach (TypeDef typeDef in Context.module.GetTypes())
            {
                foreach (MethodDef methodDef in typeDef.Methods)
                {
                    try
                    {
                        for (int i = 0; i < methodDef.Body.Instructions.Count; i++)
                        {
                            try
                            {
                                object str = null;
                                if (methodDef.Body.Instructions[i - 1].IsLdcI4() && methodDef.Body.Instructions[i].OpCode == OpCodes.Call)
                                {
                                    try
                                    {
                                        MethodInfo strMethod;
                                        if (methodDef.Body.Instructions[i].Operand.GetType().ToString().Contains("MethodDef"))
                                        {
                                            strMethod = GetStringMethod((int)((MethodDef)methodDef.Body.Instructions[i].Operand).MDToken.Raw, Context.assembly);
                                        }
                                        else
                                        {
                                            strMethod = GetStringMethod((int)((MethodSpec)methodDef.Body.Instructions[i].Operand).Method.MDToken.Raw, Context.assembly);
                                        }
                                        int value = methodDef.Body.Instructions[i - 1].GetLdcI4Value();
                                        str = strMethod.Invoke(null, new object[]
                                        {
                                            value
                                        });
                                    }
                                    catch {}
                                }
                                if (str != null)
                                {
                                    methodDef.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
                                    methodDef.Body.Instructions[i].OpCode = OpCodes.Ldstr;
                                    methodDef.Body.Instructions[i].Operand = (string)str;
                                    count++;
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            return count;
        }
    }
}
