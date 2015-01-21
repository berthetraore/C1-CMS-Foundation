﻿using System;
using System.Configuration;
using Composite.C1Console.Security.Plugins.PasswordPolicy;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;
using Microsoft.Practices.ObjectBuilder;

namespace Composite.Plugins.Security.PasswordRules.MinimumLength
{
    [ConfigurationElementType(typeof(MinimumLengthPasswordRuleData))]
    internal class MinimumLengthPasswordRule : IPasswordRule
    {
        public MinimumLengthPasswordRule(int minLength)
        {
            _minLength = minLength;
        }

        private readonly int _minLength = 7;

        public bool ValidatePassword(string password)
        {
            return password.Length >= _minLength;
        }

        public string GetRuleDescription()
        {
            // TODO: localize
            return string.Format("Password should be at least '{0}' characters long", _minLength);
        }
    }

    [Assembler(typeof(MinimumLengthPasswordRuleAssembler))]
    internal class MinimumLengthPasswordRuleData : PasswordRuleData
    {
        [ConfigurationProperty("minLength", IsRequired = true)]
        public int MinLength
        {
            get { return (int)base["minLength"]; }
            set { base["minLength"] = value; }
        }
    }

    internal class MinimumLengthPasswordRuleAssembler : IAssembler<IPasswordRule, PasswordRuleData>
    {
        IPasswordRule IAssembler<IPasswordRule, PasswordRuleData>.Assemble(IBuilderContext context, PasswordRuleData objectConfiguration, IConfigurationSource configurationSource, ConfigurationReflectionCache reflectionCache)
        {
            var data = (MinimumLengthPasswordRuleData) objectConfiguration;
            return new MinimumLengthPasswordRule(data.MinLength);
        }
    }
}
