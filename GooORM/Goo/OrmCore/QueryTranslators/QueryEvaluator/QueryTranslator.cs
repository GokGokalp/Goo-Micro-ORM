using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Goo.OrmCore
{
    /// <summary>
    /// QueryTranslator sınıfı lambda ifadelerini Visitor deseni aracılığı ile handle edebilmemizi sağlamaktadır. VisitConstant tiplerinde 'Contains' desteğini sağladım.
    /// The QueryTranslator class allows us to use Visitor design pattern to handle the lambda expressions. The VisitConstant types have support for the "Contains" method.
    /// Referrer: http://blogs.msdn.com/b/mattwar/archive/2007/07/31/linq-building-an-iqueryable-provider-part-ii.aspx
    /// </summary>
    public class QueryTranslator : ExpressionVisitor
    {
        #region Private Members
        private StringBuilder sb;
        private bool IsConstantContains = false;
        private string _constantContainsName = string.Empty;
        private string _whereClause = string.Empty;
        #endregion

        #region Public Methods
        public string WhereClause
        {
            get
            {
                return _whereClause;
            }
        }

        public QueryTranslator()
        {
        }

        public string Translate(Expression expression)
        {
            this.sb = new StringBuilder();
            this.Visit(expression);

            _whereClause = this.sb.ToString();

            return _whereClause;
        }
        #endregion

        #region Protected Methods
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.Name == "Contains")
            {
                IsConstantContains = true;
                _constantContainsName = ((MemberExpression)m.Object).Member.Name;

                Expression nextExpression = m.Arguments[0];

                return this.Visit(nextExpression);
            }

            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    sb.Append(" NOT ");
                    this.Visit(u.Operand);
                    break;
                case ExpressionType.Convert:
                    this.Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }
            return u;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            sb.Append("(");
            this.Visit(b.Left);

            switch (b.NodeType)
            {
                case ExpressionType.And:
                    sb.Append(" AND ");
                    break;

                case ExpressionType.AndAlso:
                    sb.Append(" AND ");
                    break;

                case ExpressionType.Or:
                    sb.Append(" OR ");
                    break;

                case ExpressionType.OrElse:
                    sb.Append(" OR ");
                    break;

                case ExpressionType.Equal:
                    if (IsNullConstant(b.Right))
                    {
                        sb.Append(" IS ");
                    }
                    else
                    {
                        sb.Append(" = ");
                    }
                    break;

                case ExpressionType.NotEqual:
                    if (IsNullConstant(b.Right))
                    {
                        sb.Append(" IS NOT ");
                    }
                    else
                    {
                        sb.Append(" <> ");
                    }
                    break;

                case ExpressionType.LessThan:
                    sb.Append(" < ");
                    break;

                case ExpressionType.LessThanOrEqual:
                    sb.Append(" <= ");
                    break;

                case ExpressionType.GreaterThan:
                    sb.Append(" > ");
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    sb.Append(" >= ");
                    break;

                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));

            }

            this.Visit(b.Right);
            sb.Append(")");
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            IQueryable q = c.Value as IQueryable;

            if (q == null && c.Value == null)
            {
                sb.Append("NULL");
            }
            else if (q == null)
            {
                switch (Type.GetTypeCode(c.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        sb.Append(((bool)c.Value) ? 1 : 0);
                        break;

                    case TypeCode.String:
                        if (IsConstantContains)
                        {
                            sb.Append(_constantContainsName);
                            sb.Append(" LIKE '%");
                            sb.Append(c.Value);
                            sb.Append("%'");

                            _constantContainsName = string.Empty;
                            IsConstantContains = false;
                        }
                        else
                        {
                            sb.Append("'");
                            sb.Append(c.Value);
                            sb.Append("'");
                        }
                        break;

                    case TypeCode.DateTime:
                        sb.Append("'");
                        sb.Append(Convert.ToDateTime(c.Value).ToString("yyyy-MM-dd HH:mm:ss"));
                        sb.Append("'");
                        break;

                    case TypeCode.Object:
                        throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", c.Value));

                    default:
                        sb.Append(c.Value);
                        break;
                }
            }

            return c;
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                sb.Append(m.Member.Name);
                return m;
            }

            throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
        }

        protected bool IsNullConstant(Expression exp)
        {
            return (exp.NodeType == ExpressionType.Constant && ((ConstantExpression)exp).Value == null);
        }
        #endregion
    }
}