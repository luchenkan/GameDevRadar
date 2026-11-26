using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HybridCLR.Runtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace HybridCLR
{
    [Preserve]
    public static class RuntimeApi
    {
        /// <summary>
        /// load supplementary metadata assembly
        /// </summary>
        /// <param name="dllBytes"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
#if UNITY_EDITOR
        public static unsafe LoadImageErrorCode LoadMetadataForAOTAssembly(byte[] dllBytes, HomologousImageMode mode)
        {
            return LoadImageErrorCode.OK;
        }
#else
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern LoadImageErrorCode LoadMetadataForAOTAssembly(byte[] dllBytes, HomologousImageMode mode);
#endif

        /// <summary>
        /// prejit method to avoid the jit cost of first time running
        /// </summary>
        /// <param name="method"></param>
        /// <returns>return true if method is jited, return false if method can't be jited </returns>
        /// 
#if UNITY_EDITOR
        public static bool PreJitMethod(MethodInfo method)
        {
            return false;
        }
#else
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool PreJitMethod(MethodInfo method);
#endif

        /// <summary>
        /// prejit all methods of class to avoid the jit cost of first time running
        /// </summary>
        /// <param name="type"></param>
        /// <returns>return true if class is jited, return false if class can't be jited </returns>
#if UNITY_EDITOR
        public static bool PreJitClass(Type type)
        {
            return false;
        }
#else
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool PreJitClass(Type type);
#endif

        /// <summary>
        /// get the maximum number of StackObjects in the interpreter thread stack (size*8 represents the final memory size occupied
        /// </summary>
        /// <returns></returns>
        public static int GetInterpreterThreadObjectStackSize()
        {
            return GetRuntimeOption(RuntimeOptionId.InterpreterThreadObjectStackSize);
        }

        /// <summary>
        /// set the maximum number of StackObjects for the interpreter thread stack (size*8 represents the final memory size occupied)
        /// </summary>
        /// <param name="size"></param>
        public static void SetInterpreterThreadObjectStackSize(int size)
        {
            SetRuntimeOption(RuntimeOptionId.InterpreterThreadObjectStackSize, size);
        }


        /// <summary>
        /// get the number of interpreter thread function frames (sizeof(InterpreterFrame)*size represents the final memory size occupied)
        /// </summary>
        /// <returns></returns>
        public static int GetInterpreterThreadFrameStackSize()
        {
            return GetRuntimeOption(RuntimeOptionId.InterpreterThreadFrameStackSize);
        }
        
        /// <summary>
        /// set the number of interpreter thread function frames (sizeof(InterpreterFrame)*size represents the final memory size occupied)
        /// </summary>
        /// <param name="size"></param>
        public static void SetInterpreterThreadFrameStackSize(int size)
        {
            SetRuntimeOption(RuntimeOptionId.InterpreterThreadFrameStackSize, size);
        }

        public static bool IsTransformOptimizationEnabled()
        {
            return GetRuntimeOption(RuntimeOptionId.TransformOptimization) != 0;
        }

        public static void EnableTransformOptimization(bool enable)
        {
            SetRuntimeOption(RuntimeOptionId.TransformOptimization, enable ? 1 : 0);
        }


#if UNITY_EDITOR

        private static readonly Dictionary<RuntimeOptionId, int> s_runtimeOptions = new Dictionary<RuntimeOptionId, int>();

        /// <summary>
        /// set runtime option value
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="value"></param>
        public static void SetRuntimeOption(RuntimeOptionId optionId, int value)
        {
            s_runtimeOptions[optionId] = value;
        }
#else
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void SetRuntimeOption(RuntimeOptionId optionId, int value);
#endif

        /// <summary>
        /// get runtime option value
        /// </summary>
        /// <param name="optionId"></param>
        /// <returns></returns>
#if UNITY_EDITOR
        public static int GetRuntimeOption(RuntimeOptionId optionId)
        {
            if (s_runtimeOptions.TryGetValue(optionId, out var value))
            {
                return value;
            }
            return 0;
        }
#else
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern int GetRuntimeOption(RuntimeOptionId optionId);
#endif


        /// <summary>
        /// Indicates whether full generic sharing is enabled.
        /// - For Unity 2020 and earlier versions, returns false.
        /// - For Unity 2021, returns true if the "faster (smaller)" option is selected during building; otherwise, returns false.
        /// - For Unity 2022 and later versions, returns true.
        /// </summary>
#if UNITY_EDITOR || !UNITY_2021
        public static bool IsFullGenericSharingEnabled()
        {
#if UNITY_2022_1_OR_NEWER
            return true;
#else
            return false;
#endif
        }
#else
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool IsFullGenericSharingEnabled();
#endif

#if UNITY_EDITOR
        private static void HotfixAssembly(Assembly targetAssembly, byte[] hotfixAssemblyBytes, int[] hotfixMethodTokens)
        {

        }
#else
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void HotfixAssembly(Assembly targetAssembly, byte[] hotfixAssemblyBytes, int[] hotfixMethodTokens);
#endif

        public static void HotfixAssembly(string assemblyName, byte[] assemblyBytes, List<HotfixType> types)
        {
            Assembly targetAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName);
            if (targetAssembly == null)
            {
                throw new Exception($"HotfixAssembly not found! assembly:{assemblyName}");
            }
            var hotfixMethodTokens = new SortedSet<int>();
            foreach (HotfixType hotfixClass in types)
            {
                Type targetType = targetAssembly.GetType(hotfixClass.name);
                if (targetType == null)
                {
                    throw new Exception($"HotfixClass not found! HotfixAssembly assembly:{assemblyName} class:{hotfixClass.name}");
                }
                ConstructorInfo[] constructors = null;
                (ConstructorInfo, string)[] constructorWithSignatures = null;
                (MethodInfo, string)[] methodWithNames = null;
                (MethodInfo, string)[] methodWithSignatures = null;
                foreach (HotfixMethod hotfixMethod in hotfixClass.methods)
                {
                    if (!string.IsNullOrEmpty(hotfixMethod.name))
                    {
                        if (!string.IsNullOrEmpty(hotfixMethod.signature))
                        {
                            throw new Exception($"can't set both name and signature of HotfixMethod. assembly:{assemblyName} class:{hotfixClass.name} method name:{hotfixMethod.name} signature:{hotfixMethod.signature}");
                        }
                        if (hotfixMethod.name == ".ctor")
                        {
                            bool anyFound = false;
                            // SubString(5) to remove "Void " from the start of the string
                            constructors = constructors ?? targetType.GetConstructors();
                            foreach (var ctor in constructors)
                            {
                                anyFound = true;
                                hotfixMethodTokens.Add(ctor.MetadataToken);
                            }
                            if (!anyFound)
                            {
                                throw new Exception($"HotfixMethod not found! assembly:{assemblyName} class:{hotfixClass.name} method:{hotfixMethod.name}");
                            }
                        }
                        else if (hotfixMethod.name == ".cctor")
                        {
                            if (targetType.TypeInitializer == null)
                            {
                                throw new Exception($"HotfixMethod not found! assembly:{assemblyName} class:{hotfixClass.name} method:{hotfixMethod.name}");
                            }
                            hotfixMethodTokens.Add(targetType.TypeInitializer.MetadataToken);
                        }
                        else
                        {
                            bool anyFound = false;
                            methodWithNames = methodWithNames ?? targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).Select(m => (m, m.Name)).ToArray();
                            foreach (var method in methodWithNames)
                            {
                                if (method.Item2 == hotfixMethod.name)
                                {
                                    anyFound = true;
                                    hotfixMethodTokens.Add(method.Item1.MetadataToken);
                                }
                            }
                            if (!anyFound)
                            {
                                throw new Exception($"HotfixMethod not found! assembly:{assemblyName} class:{hotfixClass.name} method:{hotfixMethod.name}");
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(hotfixMethod.signature))
                    {
                        bool anyFound = false;
                        // SubString(5) to remove "Void " from the start of the string
                        constructorWithSignatures = constructorWithSignatures ?? targetType.GetConstructors().Select(c => (c, c.ToString())).ToArray();
                        foreach (var e in constructorWithSignatures)
                        {
                            if (e.Item2 == hotfixMethod.signature)
                            {
                                anyFound = true;
                                hotfixMethodTokens.Add(e.Item1.MetadataToken);
                            }
                        }

                        methodWithSignatures = methodWithSignatures ?? targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                            .Select(m => (m, m.ToString())).ToArray();
                        foreach (var e in methodWithSignatures)
                        {
                            if (e.Item2 == hotfixMethod.signature)
                            {
                                anyFound = true;
                                hotfixMethodTokens.Add(e.Item1.MetadataToken);
                            }
                        }
                        if (!anyFound)
                        {
                            throw new Exception($"HotfixMethod not found! assembly:{assemblyName} class:{hotfixClass.name} method:{hotfixMethod.signature}");
                        }
                    }
                    else
                    {
                        throw new Exception($"HotfixMethod name and signature can't be both empty! assembly:{assemblyName} class:{hotfixClass.name}");
                    }
                }
            }
            Debug.Log($"HotfixAssembly assembly:{assemblyName} methodTokens:{string.Join(", ", hotfixMethodTokens)}");
            HotfixAssembly(targetAssembly, assemblyBytes, hotfixMethodTokens.ToArray());
        }

        public static void HotfixAssemblies(HotfixManifest hotfixManifest)
        {
            foreach (HotfixAssembly hotfixAssembly in hotfixManifest.assemblies)
            {
                HotfixAssembly(hotfixAssembly.name, hotfixAssembly.assemblyBytes, hotfixAssembly.types);
            }
        }


#if UNITY_EDITOR
        public static unsafe LoadImageErrorCode LoadOriginalDifferentialHybridAssembly(string assName)
        {
            return LoadImageErrorCode.OK;
        }
#else
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern unsafe LoadImageErrorCode LoadOriginalDifferentialHybridAssembly(string assName);
#endif

        public static unsafe LoadImageErrorCode LoadDifferentialHybridAssemblyWithDHAO(byte[] currentDllBytes, byte[] dllSymbolBytes, byte[] dhaoBytes)
        {
            return LoadDifferentialHybridAssemblyWithDHAOImpl(currentDllBytes, dllSymbolBytes, dhaoBytes);
        }

#if UNITY_EDITOR
        private static LoadImageErrorCode LoadDifferentialHybridAssemblyWithDHAOImpl(byte[] dllBytes, byte[] dllSymbolBytes, byte[] dhaoBytes)
        {
            return LoadImageErrorCode.OK;
        }
#else
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe LoadImageErrorCode LoadDifferentialHybridAssemblyWithDHAOImpl(byte[] dllBytes, byte[] dllSymbolBytes, byte[] dhaoBytes);
#endif

        public static unsafe LoadImageErrorCode LoadDifferentialHybridAssemblyWithMetaVersion(byte[] currentDllBytes, byte[] currentDllSymbolBytes, byte[] originalMetaVersionFileBytes, byte[] currentMetaVersionFileBytes)
        {
            return LoadDifferentialHybridAssemblyWithMetaVersionImpl(currentDllBytes, currentDllSymbolBytes, originalMetaVersionFileBytes, currentMetaVersionFileBytes);
        }

#if UNITY_EDITOR
        private static LoadImageErrorCode LoadDifferentialHybridAssemblyWithMetaVersionImpl(byte[] currentDllBytes, byte[] currentDllSymbolBytes, byte[] originalMetaVersionFileBytes, byte[] currentMetaVersionFileBytes)
        {
            return LoadImageErrorCode.OK;
        }
#else
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe LoadImageErrorCode LoadDifferentialHybridAssemblyWithMetaVersionImpl(byte[] currentDllBytes, byte[] currentDllSymbolBytes, byte[] originalMetaVersionFileBytes, byte[] currentMetaVersionFileBytes);
#endif
    }
}
