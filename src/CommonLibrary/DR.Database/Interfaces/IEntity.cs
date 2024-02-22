namespace DR.Database.Interfaces;

public interface IEntity {
    public string Id { get; set; }
    public string MerchantId { get; set; }
}

public interface ISyncEntity : IEntity {
    public DateTimeOffset ModifiedDate { get; set; }
}
