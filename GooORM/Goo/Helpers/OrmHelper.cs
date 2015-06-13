using Goo.ConfigurationManagement;
using System;
using System.Configuration;
using System.Linq.Expressions;

namespace Goo.Helpers
{
    public class OrmHelper
    {
        #region Private References
        private static Lazy<ConfigurationManagementManager> configReader = new Lazy<ConfigurationManagementManager>(() => new ConfigurationManagementManager());
        #endregion

        #region Constructor
        private static readonly Lazy<OrmHelper> instance = new Lazy<OrmHelper>(() => new OrmHelper());
        public static OrmHelper getInstance
        {
            get { return instance.Value; }
        }
        private OrmHelper()
        {

        }
        #endregion

        #region Public Methods
        public string GetExpressionPropertyName<TEntity>(Expression<Func<TEntity, object>> predicate)
        {
            MemberExpression body = predicate.Body as MemberExpression;

            if (body == null)
            {
                UnaryExpression ubody = (UnaryExpression)predicate.Body;
                body = ubody.Operand as MemberExpression;
            }

            return body.Member.Name;
        }

        public T ConvertTo<T>(object obj)
        {
            if (!(obj is IConvertible))
            {
                if (obj is T)
                    return (T)obj;

                if (typeof(T).BaseType == typeof(Enum))
                    return (T)Enum.Parse(typeof(T), obj.ToString());

                if (typeof(T) == typeof(Boolean))
                {
                    if (obj.ToString() == "1")
                        return (T)(object)(true);
                    else
                        return (T)(object)(false);
                }
            }

            return (T)Convert.ChangeType(obj, typeof(T));
        }

        public string DbProviderInvariantName
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["gooConnectionString"].ProviderName;
            }
        }
        #endregion
    }
}