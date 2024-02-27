
using DR.Constant.Enums;
using DR.Operators.AutoGenerate;

namespace DR.Operators.AutoGenerate {
    public static class AutoGenerateFactory {

        public static IAutoGenerateOperator Create(ESetting settingType) {
            var type = typeof(IAutoGenerateOperator);
            var autoGenOpTypes = type.Assembly.GetTypes()
                .Where(o => type.IsAssignableFrom(o) && !o.IsInterface)
                .ToList();

            foreach (var autoGenOpType in autoGenOpTypes) {
                var autoGenOp = (IAutoGenerateOperator)Activator.CreateInstance(autoGenOpType)!;
                if (autoGenOp.Handler == settingType) {
                    return autoGenOp;
                }
            }
            throw new NotImplementedException();
        }
    }
}
