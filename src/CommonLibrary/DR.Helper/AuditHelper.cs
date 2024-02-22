using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace DR.Helper;

public static class AuditHelper {
    public const string RightArrow = "&rarr;";

    #region String

    public static string S(string? value)
        => value ?? string.Empty;

    public static string S(string? left, string? right, string symbol = RightArrow)
        => $"{S(left)} {symbol} {S(right)}";

    public static string SIf(bool condition, string? left, string? right, string symbol = RightArrow)
        => condition ? S(left) : S(left, right, symbol);

    #endregion String

    #region Number

    public static string N(decimal? value)
        => $"[Number({(value.HasValue ? value : "null")})]";

    public static string N(decimal? left, decimal? right, string symbol = RightArrow)
        => $"{N(left)} {symbol} {N(right)}";

    public static string NCompare(decimal? left, decimal? right, string symbol = RightArrow)
        => left == right ? N(right) : N(left, right, symbol);

    public static bool NEqual(decimal? left, decimal? right)
      => N(right) == N(left);

    #endregion Number

    #region Currency

    public static string C(decimal? value)
        => $"[Currency({(value.HasValue ? value : "null")})]";

    public static string C(decimal? left, decimal? right, string symbol = RightArrow)
        => $"{C(left)} {symbol} {C(right)}";

    public static string CCompare(decimal? left, decimal? right, string symbol = RightArrow)
        => left == right ? C(right) : C(left, right, symbol);

    public static bool CEqual(decimal? left, decimal? right)
      => C(left) == C(right);

    #endregion Currency

    #region Short Date

    public static string SD(DateTimeOffset? date)
        => $"[ShortDate({(date.HasValue ? date.Value.ToUnixTimeMilliseconds() : "null")})]";

    public static string SD(DateTimeOffset? left, DateTimeOffset? right, string symbol = RightArrow)
        => $"{SD(left)} {symbol} {SD(right)}";

    #endregion Short Date

    #region Long Date

    public static string LD(DateTimeOffset? date)
        => $"[LongDate({(date.HasValue ? date.Value.ToLocalTime().ToUnixTimeMilliseconds() : "null")})]";

    #endregion Long Date

    #region Boolean

    public static string B(bool value)
        => value ? "Có" : "Không";

    public static string B(bool left, bool right, string symbol = RightArrow)
        => $"{B(left)} {symbol} {B(right)}";

    public static string BIf(bool condition, bool left, bool right, string symbol = RightArrow)
        => condition ? B(left) : B(left, right, symbol);

    #endregion Boolean

    #region Extension method

    public static string AuditInfo<T, TProperty>(this T obj, Expression<Func<T, TProperty>> expression) {
        return AuditInfo(obj.Audit(expression), obj.Value(expression)?.ToString());
    }

    public static string AuditInfo<T, TProperty>(this T obj, Expression<Func<T, TProperty>> expression, string? value) {
        return AuditInfo(obj.Audit(expression), value);
    }

    public static string AuditInfo<T, TProperty>(this T obj, Expression<Func<T, TProperty>> expression, string? value, string? otherValue) {
        return AuditInfo(obj.Audit(expression), value, otherValue);
    }

    public static string AuditInfo<T, TProperty>(this T obj, Expression<Func<T, TProperty>> expression, T otherObj) {
        return AuditInfo(obj.Audit(expression), obj.Value(expression)?.ToString(), otherObj.Value(expression)?.ToString());
    }

    public static string AuditInfo(string auditName, string? value) {
        return $"{auditName}: {value}";
    }

    public static string AuditInfo(string auditName, string? value, string? otherValue) {
        return $"{auditName}: {value} {AuditHelper.RightArrow} {otherValue}";
    }

    private static string Audit<T, TProperty>(this T _, Expression<Func<T, TProperty>> expression) {
        var prop = GetPropertyFromExpression(expression);
        var attrs = prop.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attrs == null || attrs.Length == 0) {
            return prop.Name;
        }
        return ((DescriptionAttribute)attrs[0]).Description;
    }

    private static object? Value<T, TProperty>(this T obj, Expression<Func<T, TProperty>> expression) {
        var prop = GetPropertyFromExpression(expression);
        return prop.GetValue(obj, null);
    }

    private static PropertyInfo GetPropertyFromExpression<T, TProperty>(Expression<Func<T, TProperty>> expression) {
        if (expression.Body is not MemberExpression memberExpression)
            throw new InvalidOperationException("Expression must be a member expression");

        return memberExpression.Member as PropertyInfo
            ?? throw new InvalidOperationException("Expression must be a member expression");
    }

    #endregion Extension method
}
