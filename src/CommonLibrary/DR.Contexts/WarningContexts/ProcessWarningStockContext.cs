namespace DR.Contexts.WarningContexts;

public class ProcessWarningStockContext {
    public string Id { get; set; }
    public bool IsDelete { get; set; }

    public ProcessWarningStockContext(string itemId, bool isDelete) {
        this.Id = itemId;
        this.IsDelete = isDelete;
    }
}
