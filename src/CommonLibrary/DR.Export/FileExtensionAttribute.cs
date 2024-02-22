namespace DR.Export {

    public class FileExtensionAttribute : Attribute {
        public string Extension { get; private set; }

        public FileExtensionAttribute(string extension) {
            Extension = extension;
        }
    }

    public static class EnumExtention {

        public static string FileExtension(this EExportFile value) {
            var field = value.GetType().GetField(value.ToString());
            if (field == null) return string.Empty;

            var attr = Attribute.GetCustomAttribute(field, typeof(FileExtensionAttribute));
            return attr is FileExtensionAttribute FileExtensionAttr ? FileExtensionAttr.Extension : string.Empty;
        }
    }
}
