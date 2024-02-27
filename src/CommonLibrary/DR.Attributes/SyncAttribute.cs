namespace DR.Attributes {
    [AttributeUsage(AttributeTargets.Field)]
    public class SyncAttribute : Attribute {
        public bool IsSync { get; set; } = true;
    }
}
