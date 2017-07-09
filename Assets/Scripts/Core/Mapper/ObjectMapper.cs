using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Core.Mapper
{
    public class ObjectMapper
    {
        private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> PropertyDict =
            new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        public static void Bind<TSource, TTarget>(Action<MappingConfig<TSource, TTarget>> config)
        {
            var mapPair = MappingPair.Add(typeof(TSource), typeof(TTarget));

            var mappingConfig = new MappingConfig<TSource, TTarget>();
            config(mappingConfig);

            mapPair.SetConfig(mappingConfig);
        }


        public static TTarget Map<TTarget>(object data) where TTarget : new()
        {
            var target = new TTarget();
            if (data != null)
                Map(data, target);
            return target;
        }

        public static void Map(object source, object target)
        {
            Type sourceType = source.GetType();
            Type targetType = target.GetType();

            var sourceProperties = GetProperties(sourceType);
            var targetProperties = GetProperties(targetType);

            var mapPair = MappingPair.Get(sourceType, targetType);

            foreach (var sourceProp in sourceProperties)
            {
                var targetPropertyName = (mapPair != null) ? mapPair.GetPropertyName(sourceProp.Key) : sourceProp.Key;
                if (targetProperties.ContainsKey(targetPropertyName))
                {
                    var targetProperty = targetProperties[targetPropertyName];

                    if (mapPair != null && mapPair.IsInIgnore(targetProperty.Name))
                    {
                        continue;
                    }

                    var sourceProperty = sourceProp.Value;
                    Type sourcePropType = (sourceProperty.PropertyType);
                    Type targetPropType = (targetProperty.PropertyType);


                    try
                    {
                        object sourceValue = sourceProperty.GetValue(source, null);

                        if(mapPair != null)
                        {
                            var customMapper = mapPair.GetCustomMapper(sourceProperty.Name);
                            if (customMapper != null)
                            {
                                PropertyInfo typeProperty;
                                sourceProperties.TryGetValue(customMapper.TypeName, out typeProperty);

                                if (typeProperty != null)
                                {
                                    var typeValue = typeProperty.GetValue(source, null);
                                    var result = customMapper.MapperAction.Invoke(typeValue, Convert.ToString(sourceValue));
                                    targetProperty.SetValue(target, result, null);
                                    return;
                                }
                            }
                        }

                        if (targetPropType == sourcePropType)
                        {
                            targetProperty.SetValue(target, sourceValue, null);
                        }
                        else if (sourcePropType.IsEnum)
                            targetProperty.SetValue(target, (int) sourceValue, null);
                        else
                        {
                            if (targetPropType == typeof(int))
                                targetProperty.SetValue(target, Convert.ToInt32(sourceValue), null);
                            else if (targetPropType == typeof(long))
                                targetProperty.SetValue(target, Convert.ToInt64(sourceValue), null);
                            else if (targetPropType == typeof(bool))
                                targetProperty.SetValue(target, Convert.ToBoolean(sourceValue), null);
                            else if (targetPropType == typeof(string))
                                targetProperty.SetValue(target, Convert.ToString(sourceValue), null);
                            else
                                targetProperty.SetValue(target, sourceValue, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("Can't map " + targetProperty.Name + " with " + sourceProperty.Name + " >> " + ex.Message);
                        throw;
                    }
                }

            }
        }

        public static List<TTarget> MapList<TTarget>(IEnumerable items) where TTarget : new()
        {
            var list = new List<TTarget>();
            foreach (var item in items)
            {
                list.Add(Map<TTarget>(item));
            }

            return list;
        }

        private static Dictionary<string, PropertyInfo> GetProperties(Type type)
        {
            if (!PropertyDict.ContainsKey(type))
                PropertyDict[type] = ConvertProps(type);

            return PropertyDict[type];
        }

        private static Dictionary<string, PropertyInfo> ConvertProps(Type type)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var dict = new Dictionary<string, PropertyInfo>();
            for (int i = 0; i < properties.Length; i++)
            {
                dict.Add(properties[i].Name, properties[i]);
            }

            return dict;
        }

        public static void RemoveAllBindings()
        {
            PropertyDict.Clear();
            MappingPair.Clear();
        }
    }
}