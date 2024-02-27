namespace DR.Attributes {
    [AttributeUsage(AttributeTargets.Property)]
    public class DRAuditAttribute : Attribute {
        public string Name { get; set; }

        public DRAuditAttribute(string name) {
            this.Name = name;
        }
    }
}
