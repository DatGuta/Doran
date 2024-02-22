using FluentFTP;
using Microsoft.Extensions.Configuration;
using FtpConfig = DR.Configuration.FtpConfig;
namespace DR.Helper;

public static class FtpHelper {

    public static async Task UploadImage(string[] directories, string filename, byte[]? data, IConfiguration configuration, string key = "Ftp") {
        await UploadFile(directories, filename, data, configuration.GetSection(key).Get<FtpConfig>()!);
    }

    public static async Task UploadFile(string[] directories, string filename, byte[]? data, IConfiguration configuration, string key = "Ftp") {
        await UploadFile(directories, filename, data, configuration.GetSection(key).Get<FtpConfig>()!);
    }

    public static async Task<byte[]> DownloadBytes(string filename, IConfiguration configuration, string key = "Ftp") {
        return await DownloadBytes(filename, configuration.GetSection(key).Get<FtpConfig>()!);
    }

    public static async Task<FtpStatus> DownloadFile(string localPath, string filename, IConfiguration configuration, string key = "Ftp") {
        return await DownloadFile(localPath, filename, configuration.GetSection(key).Get<FtpConfig>()!);
    }

    public static async Task DeleteFile(string filename, IConfiguration configuration, string key = "Ftp") {
        await DeleteFile(filename, configuration.GetSection(key).Get<FtpConfig>()!);
    }

    private static async Task UploadFile(string[] directories, string filename, byte[]? data, FtpConfig config) {
        if (data == null) return;
        using var client = new AsyncFtpClient(config.Host, config.Username, config.Password);
        await client.AutoConnect();

        foreach (var directory in directories) {
            var existed = await client.DirectoryExists(directory);
            if (existed) continue;
            await client.CreateDirectory(directory);
        }

        using Stream stream = new MemoryStream(data);
        await client.UploadStream(stream, filename);
    }

    private static async Task<byte[]> DownloadBytes(string path, FtpConfig config) {
        using var client = new AsyncFtpClient(config.Host, config.Username, config.Password);
        await client.AutoConnect();
        var existed = await client.FileExists(path);
        if (!existed) return Array.Empty<byte>();
        return await client.DownloadBytes(path, 0);
    }

    private static async Task<FtpStatus> DownloadFile(string localPath, string path, FtpConfig config) {
        using var client = new AsyncFtpClient(config.Host, config.Username, config.Password);
        await client.AutoConnect();
        var existed = await client.FileExists(path);
        if (!existed) return FtpStatus.Failed;
        return await client.DownloadFile(localPath, path, FtpLocalExists.Overwrite);
    }

    private static async Task DeleteFile(string filename, FtpConfig config) {
        using var client = new AsyncFtpClient(config.Host, config.Username, config.Password);
        await client.AutoConnect();
        var existed = await client.FileExists(filename);
        if (!existed) return;

        await client.DeleteFile(filename);
    }
}
