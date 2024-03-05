using Microsoft.Extensions.DependencyInjection;

namespace DR.Common;

public sealed class Container {
    private static readonly Lazy<Container> lazy = new(() => new Container());

    public static Container Instance => lazy.Value;

    private Container() {
    }

    private IServiceProvider? serviceProvider;
    private int access = 0;

    public Container SetServiceProvider(IServiceProvider serviceProvider) {
        this.serviceProvider = serviceProvider;
        return this;
    }

    public Container SetAccess(int access) {
        this.access = access;
        return this;
    }

    public IServiceScope CreateScope() {
        return this.serviceProvider?.CreateScope() ?? throw new MissingMemberException(nameof(serviceProvider));
    }

    public int GetAccess() => this.access;
}
