using System.ComponentModel;

namespace DR.Common.Extensions;

public static class EnumExtension {

    public static bool TryGet<TAttribute>(this Enum value, out TAttribute? attr)
        where TAttribute : Attribute {
        var field = value.GetType().GetField(value.ToString()) ?? throw new InvalidEnumArgumentException(nameof(value));
        var customAttr = Attribute.GetCustomAttribute(field, typeof(TAttribute));
        if (customAttr is TAttribute descAttr) {
            attr = descAttr;
            return true;
        }
        attr = null;
        return false;
    }

    public static string GetValue<TAttribute>(this Enum value, Func<TAttribute, string> selector)
        where TAttribute : Attribute {
        return value.TryGet(out TAttribute? descAttr) && descAttr != null ? selector(descAttr) : value.ToString();
    }

    public static string Description(this Enum value) {
        return value.GetValue<DescriptionAttribute>(o => o.Description);
    }
}
