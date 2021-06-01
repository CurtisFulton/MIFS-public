using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Mifs.Hosting
{
    /// <summary>
    /// Helper class to call the IntegrationStartup class' methods.
    /// Contains helper methods to find the ConfigureServices and Configure method info's.
    /// This was mostly just taken from the Asp.Net Core repo.
    /// </summary>
    internal class StartupLoader
    {
        public StartupLoader(object instance, string environmentName)
        {
            this.Instance = instance;
            this.StartupType = instance.GetType();
            this.EnvironmentName = environmentName;

            this.ConfigureServicesMethodInfo = this.GetConfigureServicesMethodInfo();
            this.ConfigureMethodInfo = this.GetConfigureMethodInfo();
        }

        public object Instance { get; }
        public string EnvironmentName { get; }
        public MethodInfo ConfigureServicesMethodInfo { get; }
        public MethodInfo ConfigureMethodInfo { get; }

        private Type StartupType { get; }

        private MethodInfo GetConfigureServicesMethodInfo()
            => StartupLoader.FindMethod(this.StartupType, "Configure{0}Services", this.EnvironmentName, typeof(void), isRequired: true)!;

        private MethodInfo GetConfigureMethodInfo()
            => StartupLoader.FindMethod(this.StartupType, "Configure{0}", this.EnvironmentName, typeof(Task), isRequired: true)!;

        private static MethodInfo? FindMethod(Type startupType, string methodName, string environmentName, Type returnType, bool isRequired)
        {
            var methodNameWithEnv = string.Format(CultureInfo.InvariantCulture, methodName, environmentName);
            var methodNameWithNoEnv = string.Format(CultureInfo.InvariantCulture, methodName, "");

            var methods = startupType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var selectedMethods = methods.Where(method => method.Name.Equals(methodNameWithEnv, StringComparison.OrdinalIgnoreCase)).ToList();
            if (selectedMethods.Count > 1)
            {
                throw new InvalidOperationException($"Having multiple overloads of method '{methodNameWithEnv}' is not supported.");
            }
            if (selectedMethods.Count == 0)
            {
                selectedMethods = methods.Where(method => method.Name.Equals(methodNameWithNoEnv, StringComparison.OrdinalIgnoreCase)).ToList();
                if (selectedMethods.Count > 1)
                {
                    throw new InvalidOperationException($"Having multiple overloads of method '{methodNameWithNoEnv}' is not supported.");
                }
            }

            var methodInfo = selectedMethods.FirstOrDefault();
            if (methodInfo == null)
            {
                if (isRequired)
                {
                    throw new InvalidOperationException(string.Format(
                        CultureInfo.CurrentCulture,
                        "A public method named '{0}' or '{1}' could not be found in the '{2}' type.",
                        methodNameWithEnv,
                        methodNameWithNoEnv,
                        startupType.FullName));

                }
                return null;
            }
            if (returnType != null && methodInfo.ReturnType != returnType)
            {
                if (isRequired)
                {
                    throw new InvalidOperationException(string.Format(
                        CultureInfo.CurrentCulture,
                        "The '{0}' method in the type '{1}' must have a return type of '{2}'.",
                        methodInfo.Name,
                        startupType.FullName,
                        returnType.Name));
                }
                return null;
            }

            return methodInfo;
        }
    }
}
