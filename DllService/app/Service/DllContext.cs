using DllService.app.Model;
using DllService.app.Validators;
using DllService.Common.Exceptions;
using DllService.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DllService.service
{
    public class DllContext : IDllContext
    {
        private readonly ILogger<DllContext> _logger;
        private readonly Dictionary<string, Assembly> dlls = new Dictionary<string, Assembly>();
        private readonly Dictionary<string, object> instances = new Dictionary<string, object>();
        public DllContext(ILogger<DllContext> logger)
        {
            _logger = logger;
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public object CallMethod(TypeLoadEntity entity)
        {
            var obj = TryToInit(entity, true);
            return TryToCallMethod(entity, obj, obj.GetType());
        }
        private object TryToInit(object input, bool loadTypeCare)
        {
            if (input is PrimitiveArg primitive) return primitive.Arg;
            if (!(input is TypeLoadEntity)) return input;

            var entity = input as TypeLoadEntity;
            var dll = getDll(entity);
            var classType = getClass(entity, dll);

            object ins;
            if (loadTypeCare && instances.TryGetValue(classType.FullName, out ins))
            {
                return ins;
            }
            var constructor = getConstructor(entity.ConstructorArgs, classType);
            List<object> consArgs = new List<object>();

            foreach (var argInfo in entity.ConstructorArgs)
            {
                consArgs.Add(TryToInit(argInfo, false));
            }
            if (!loadTypeCare || entity.LoadType == LoadType.Portotype)
            {
                return CreateInstance(classType, consArgs, constructor);
            }
            if (!instances.TryGetValue(classType.FullName, out ins))
            {
                ins = CreateInstance(classType, consArgs, constructor);
                instances.Add(classType.FullName, ins);
            }
            return ins;


        }

        private static Type getClass(TypeLoadEntity entity, Assembly dll)
        {
            Type classType = dll.GetType(entity.FullClassName);
            if (classType == null)
                throw new ClassNotFoundException(entity.FullClassName);
            return classType;
        }

        private Assembly getDll(TypeLoadEntity entity)
        {
            Assembly dll;
            if (!dlls.TryGetValue(entity.DllName, out dll))
                throw new Common.Exceptions.DllNotFoundException(string.Format("Dll {0} not found", entity.DllName));
            return dll;
        }

        private object TryToCallMethod(TypeLoadEntity entity, object obj, Type type)
        {
            var method = GetMethod(entity.MethodArgs, type, entity.MethodName);
            List<object> args = new List<object>();
            foreach (var item in entity.MethodArgs)
            {
                args.Add(TryToInit(item, false));
            }
            return method.Invoke(obj, args.ToArray());

        }
        private object CreateInstance(Type classType, List<object> args, ConstructorInfo constructor)
        {
            if (constructor != null)
            {
                if (args == null || args.Count == 0)
                    return constructor.Invoke(null);
                return constructor.Invoke(args.ToArray());
            }

            if (args == null || args.Count == 0)
                return Activator.CreateInstance(classType);
            return Activator.CreateInstance(classType, args.ToArray());
        }

        private ConstructorInfo getConstructor(List<AbstractArg> providedArgs, Type classType)
        {
            foreach (var cnt in classType.GetConstructors())
            {
                var neededArgs = cnt.GetParameters();
                if (neededArgs.Length != providedArgs.Count)
                    continue;
                for (int i = 0; i < neededArgs.Length; i++)
                {
                    string expectedName = neededArgs[i].ParameterType.FullName;
                    string actualName = "";
                    if (providedArgs[i] is TypeLoadEntity typeLoad)
                        actualName = typeLoad.FullClassName;
                    else if (providedArgs[i] is PrimitiveArg primitive)
                        actualName = primitive.Arg.GetType().FullName;
                    else
                        actualName = (providedArgs[i].GetType().FullName);
                    if (!expectedName.Equals(actualName))
                        continue;

                }
                return cnt;
            }
            throw new ConstructorNotFoundException(string.Format("Could not find suitable constructor for class {0} with provided args", classType.FullName));

        }
        private MethodInfo GetMethod(List<AbstractArg> providedArgs, Type classType, string methodName)
        {
            foreach (var m in classType.GetMethods())
            {
                if (m.Name != methodName)
                    continue;
                var neededArgs = m.GetParameters();
                if (neededArgs.Length != providedArgs.Count)
                    continue;
                for (int i = 0; i < neededArgs.Length; i++)
                {
                    string expectedName = neededArgs[i].ParameterType.FullName;
                    string actualName = "";
                    if (providedArgs[i] is TypeLoadEntity typeLoad)
                        actualName = typeLoad.FullClassName;
                    else if (providedArgs[i] is PrimitiveArg primitive)
                        actualName = primitive.Arg.GetType().FullName;
                    else
                        actualName = (providedArgs[i].GetType().FullName);
                    if (!expectedName.Equals(actualName))
                        continue;
                }
                return m;
            }
            throw new MethodNotFoundException(string.Format("Method {0} not found in {1}", methodName, classType.FullName));

        }

        public void Init()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return null;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LoadDll(DllEntity entity)
        {
            if (dlls.ContainsKey(entity.Name))
                throw new DllAlreadyLoadedException();
            var path = Path.GetFullPath(entity.Path);
            if (!File.Exists(path))
                throw new Common.Exceptions.DllNotFoundException(string.Format("Dll file {0} not found", path));

            try
            {
                var assembly = Assembly.LoadFile(path);
                dlls.Add(entity.Name, assembly);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new DllLoadException();
            }

        }



    }
}