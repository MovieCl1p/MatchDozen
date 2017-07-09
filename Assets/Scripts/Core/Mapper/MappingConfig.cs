using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Core.Mapper
{
    public class CustomMapper
    {
        public string TypeName { get; set; }
        public Func<object, string, object> MapperAction { get; set; }
    }

    public class MappingConfig<TSource, TTarget>
    {
        private readonly Dictionary<string, string> _bindingFields = new Dictionary<string, string>();
        private readonly Dictionary<string, CustomMapper> _customMappers = new Dictionary<string, CustomMapper>();
        private readonly HashSet<string> _ignoreFields = new HashSet<string>();

        public void Bind(Expression<Func<TSource, object>> source, Expression<Func<TTarget, object>> target)
        {
            string sourceName = GetMemberInfo(source);
            string targetName = GetMemberInfo(target);

            if (string.Equals(sourceName, targetName, StringComparison.Ordinal))
            {
                return;
            }

            _bindingFields[sourceName] = targetName;
        }

        public void AddCustomMapper(Expression<Func<TSource, string>> source, Expression<Func<TSource, object>> type, Func<object, string, object> executeAction)
        {
            var mapper = new CustomMapper
            {
                TypeName = GetMemberInfo(type),
                MapperAction = executeAction
            };

            string sourceName = GetMemberInfo(source);
            _customMappers.Add(sourceName, mapper);
        }

        public Dictionary<string, string> GetBindingFields()
        {
            return _bindingFields;
        }

        public Dictionary<string, CustomMapper> GetCustomMappers()
        {
            return _customMappers;
        }

        public HashSet<string> GetIgnoreFields()
        {
            return _ignoreFields;
        }

        public void Ignore(Expression<Func<TSource, object>> expression)
        {
            string memberName = GetMemberInfo(expression);
            _ignoreFields.Add(memberName);
        }

        private static string GetMemberInfo<T, TField>(Expression<Func<T, TField>> expression)
        {
            var member = expression.Body as MemberExpression;
            if (member == null)
            {
                var unaryExpression = expression.Body as UnaryExpression;
                if (unaryExpression != null)
                {
                    member = unaryExpression.Operand as MemberExpression;
                }

                if (member == null)
                {
                    throw new ArgumentException("Expression is not a MemberExpression", "expression");
                }
            }
            return member.Member.Name;
        }
    }
}