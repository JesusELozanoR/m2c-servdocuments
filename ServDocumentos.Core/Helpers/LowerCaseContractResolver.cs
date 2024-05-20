using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServDocumentos.Core.Helpers
{
    public class LowerCaseNamingStrategy : NamingStrategy
    {
        public LowerCaseNamingStrategy(bool processDictionaryKeys, bool overrideSpecifiedNames)
        {
            ProcessDictionaryKeys = processDictionaryKeys;
            OverrideSpecifiedNames = overrideSpecifiedNames;
        }

        public LowerCaseNamingStrategy(bool processDictionaryKeys, bool overrideSpecifiedNames, bool processExtensionDataNames)
            : this(processDictionaryKeys, overrideSpecifiedNames)
        {
            ProcessExtensionDataNames = processExtensionDataNames;
        }

        public LowerCaseNamingStrategy()
        {
        }

        protected override string ResolvePropertyName(string name)
        {
            return name.ToLower();
        }
    }
}
