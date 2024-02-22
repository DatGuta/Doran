namespace DR.Common.Lock;

internal sealed class AsyncLocker {
    private static readonly Dictionary<string, RefCountedSemaphore> lockers = new();

    private static SemaphoreSlim GetOrCreate(string key) {
        RefCountedSemaphore? item;
        lock (lockers) {
            if (lockers.TryGetValue(key, out item) && item != null) {
                ++item.RefCount;
            } else {
                item = new RefCountedSemaphore();
                lockers[key] = item;
            }
        }
        return item.Semaphore;
    }

    internal static async Task<IDisposable> LockAsync(string key, int expirySec = 60) {
        await GetOrCreate(key).WaitAsync().ConfigureAwait(false);
        return new Releaser(key, expirySec);
    }

    private sealed class RefCountedSemaphore {

        public RefCountedSemaphore() {
            RefCount = 1;
            Semaphore = new SemaphoreSlim(1, 1);
        }

        public int RefCount { get; set; }
        public SemaphoreSlim Semaphore { get; private set; }
    }

    private sealed class Releaser : IDisposable {
        private readonly string key;
        private readonly Timer timer;

        public Releaser(string key, int expirySec) {
            this.key = key;
            this.timer = new Timer(_ => Dispose(), null, TimeSpan.FromSeconds(expirySec), Timeout.InfiniteTimeSpan);
        }

        public void Dispose() {
            RefCountedSemaphore item;
            lock (lockers) {
                item = lockers[this.key];
                --item.RefCount;
                if (item.RefCount == 0)
                    lockers.Remove(this.key);
            }
            this.timer.Dispose();
            item.Semaphore.Release();
        }
    }
}
