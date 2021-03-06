using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Composite.Core;
using Composite.Core.Collections.Generic;
using Microsoft.Practices.EnterpriseLibrary.Validation;


namespace Composite.Data.Validation
{
    internal class ValidationFacadeImpl : IValidationFacade
    {
        private ResourceLocker<Resources> _resourceLocker = new ResourceLocker<Resources>(new Resources(), Resources.Initialize);


        public ValidationResults Validate<T>(T data)
            where T : class, IData
        {
            ValidationResults validationResults = Microsoft.Practices.EnterpriseLibrary.Validation.Validation.ValidateFromAttributes<T>(data);

            return validationResults;
        }



        public ValidationResults Validate(Type interfaceType, IData data)
        {
            MethodInfo methodInfo;

            using (_resourceLocker.Locker)
            {
                if (_resourceLocker.Resources.MethodCache.TryGetValue(interfaceType, out methodInfo) == false)
                {
                    methodInfo =
                        (from mi in typeof(ValidationFacade).GetMethods()
                         where (mi.Name == "Validate") &&
                               (mi.ContainsGenericParameters)
                         select mi).First();

                    methodInfo = methodInfo.MakeGenericMethod(new Type[] { interfaceType });

                    _resourceLocker.Resources.MethodCache.Add(interfaceType, methodInfo);
                }
            }

            ValidationResults validationResults;

            try
            {
                validationResults = (ValidationResults)methodInfo.Invoke(null, new object[] { data });
            }
            catch (TargetInvocationException ex)
            {
                Log.LogError("ValidationFacade", ex);
                validationResults = new ValidationResults();
                validationResults.AddResult(new ValidationResult("Exception thrown while validating. Please check field values.", data, "", "", null));
            }

            return validationResults;
        }



        public void OnFlush()
        {
            _resourceLocker.ResetInitialization();
        }



        private sealed class Resources
        {
            public Dictionary<Type, MethodInfo> MethodCache { get; set; }

            public static void Initialize(Resources resources)
            {
                resources.MethodCache = new Dictionary<Type, MethodInfo>();
            }
        }
    }
}
