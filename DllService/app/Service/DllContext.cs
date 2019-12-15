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
            var obj = TryToInit(entity);

            return TryToCallMethod(entity, obj, obj.GetType());
        }
        private object TryToInit(object input)
        {
            if (!(input is TypeLoadEntity))
            {
                return input;
            }
            var entity = (TypeLoadEntity)input;
            Assembly dll;

            if (!dlls.TryGetValue(entity.DllName, out dll))
                throw new Common.Exceptions.DllNotFoundException(string.Format("Dll {0} not found", entity.DllName));

            var classType = dll.GetType(entity.FullClassName);
            if (classType == null)
                throw new ClassNotFoundException(entity.FullClassName);

            ConstructorInfo constructor = GetConstructor(entity.ConstructorArgs, classType);
            if (constructor == null)
                throw new ConstructorNotFoundException(string.Format("Could not find suitable constructor for class {0} with provided args", entity.FullClassName));
            List<object> consArgs = new List<object>();

            foreach (var argInfo in entity.ConstructorArgs)
            {
                consArgs.Add(TryToInit(argInfo));
            }
            if (entity.LoadType == LoadType.Portotype)
            {
                return CreateInstance(classType, consArgs);
            }
            object ins;
            if (!instances.TryGetValue(classType.FullName, out ins))
            {
                ins = CreateInstance(classType, consArgs);
                instances.Add(classType.FullName, ins);
            }
            return ins;


        }
        private object TryToCallMethod(TypeLoadEntity entity, object obj, Type type)
        {
            var method = GetMethod(entity.MethodArgs, type, entity.MethodName);
            if (method == null)
                throw new MethodNotFoundException(string.Format("Method {0} not found in {1}", entity.MethodName, entity.FullClassName));
            List<object> args = new List<object>();
            foreach (var item in entity.MethodArgs)
            {
                args.Add(TryToInit(item));
            }
            return method.Invoke(obj, args.ToArray());

        }
        private object CreateInstance(Type classType, List<object> args)
        {
            if (args == null || args.Count == 0)
                return Activator.CreateInstance(classType);
            return Activator.CreateInstance(classType, args);
        }

        private ConstructorInfo GetConstructor(List<AbstractArg> providedArgs, Type classType)
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
                    if (providedArgs[i] is TypeLoadEntity)
                        actualName = ((TypeLoadEntity)providedArgs[i]).FullClassName;
                    else
                        actualName = (providedArgs[i].GetType().FullName);
                    if (!expectedName.Equals(actualName))
                        continue;

                }
                return cnt;
            }
            return null;
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
                    if (providedArgs[i] is TypeLoadEntity)
                        actualName = ((TypeLoadEntity)providedArgs[i]).FullClassName;
                    else
                        actualName = (providedArgs[i].GetType().FullName);
                    if (!expectedName.Equals(actualName))
                        continue;
                }
                return m;
            }
            return null;
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