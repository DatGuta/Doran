namespace DR.Attributes;

public class FileExtensionAttribute : Attribute {
    public string Extension { get; private set; }

    public FileExtensionAttribute(string extension) {
        Extension = extension;
    }
}
