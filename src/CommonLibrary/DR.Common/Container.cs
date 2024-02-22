using Microsoft.Extensions.DependencyInjection;

namespace DR.Common;

public sealed class Container {
    private static readonly Lazy<Container> lazy = new(() => new Container());

    public static Container Instance => lazy.Value;

    private Container() {
    }

    private IServiceProvider? serviceProvider;

    public void SetServiceProvider(IServiceProvider serviceProvider) {
        this.serviceProvider = serviceProvider;
    }

    public IServiceScope CreateScope() {
        if (this.serviceProvider == null)
            throw new MissingMemberException(nameof(serviceProvider));

        return this.serviceProvider.CreateScope();
    }
}
