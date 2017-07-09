using System;
using System.Collections.Generic;
using System.Reflection;

namespace Core.Mapper
{
    public class MappingPair
    {
        private static readonly Dictionary<string, MappingPair> MappingPairs = new Dictionary<string, MappingPair>();

        public Dictionary<string, string> BindingFields { get; private set; }
        public Dictionary<string, CustomMapper> CustomMappers { get; private set; }
        public HashSet<string> IgnoreFields { get; private set; }

        private bool _isHaveIgnores;

        public void SetConfig<TTarget, TSource>(MappingConfig<TTarget, TSource> config)
        {
            BindingFields = config.GetBindingFields();
            IgnoreFields = config.GetIgnoreFields();
            CustomMappers = config.GetCustomMappers();

            if (IgnoreFields.Count > 0)
                _isHaveIgnores = true;
        }

        public CustomMapper GetCustomMapper(string property)
        {
            if(CustomMappers.ContainsKey(property))
                return CustomMappers[property];

            return null;
        }

        public string GetPropertyName(string property)
        {
            if (BindingFields.ContainsKey(property))
                return BindingFields[property];

            return property;
        }

        public bool IsInIgnore(string property)
        {
            if (!_isHaveIgnores)
                return false;

            return IgnoreFields.Contains(property);
        }

        public static MappingPair Add(Type source, Type target)
        {
            var key = source.FullName + target.FullName;
            if (!MappingPairs.ContainsKey(key))
            {
                MappingPairs[key] = new MappingPair();
            }

            return MappingPairs[key];
        }

        public static MappingPair Get(Type source, Type target)
        {
            var key = source.FullName + target.FullName;

            if (MappingPairs.ContainsKey(key))
            {
                return MappingPairs[key];
            }

            return null;
        }

        public static void Clear()
        {
            MappingPairs.Clear();
        }
    }
}