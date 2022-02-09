using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;

namespace ColorLib.ASP
{
    public abstract class ASPServer : ILogger, ILoggerProvider
    {
        private WebApplication app;
        private readonly ConcurrentDictionary<string, ASPServer> loggers = new();

        public abstract void LogError(Exception a);
        public abstract void LogError(string a);
        public abstract void LogOut(string a);

        public void Dispose() 
        {
            loggers.Clear();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
                        Exception? exception, Func<TState, Exception, string> formatter)
        {
            if (eventId.Id == 100 || eventId.Id == 101)
                return;
            if (logLevel is LogLevel.Warning or LogLevel.Error)
                LogError($"{logLevel}-{eventId.Id} {state} {exception} {exception?.StackTrace}");
            else
                LogOut($"{logLevel}-{eventId.Id} {state} {exception}");
        }

        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd(categoryName, name => this);
        }

        private static byte[] StringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];

            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }

        public void Init(ServerConfig config)
        {
            var builder = WebApplication.CreateBuilder();
            builder.Logging.ClearProviders();
            builder.Logging.AddProvider(this);

            builder.WebHost.UseUrls(config.Url);

            if (config.Ssl)
            {
                builder.Services.AddCertificateForwarding(options =>
                {
                    options.CertificateHeader = "X-SSL-CERT";
                    options.HeaderConverter = (headerValue) =>
                    {
                        X509Certificate2 clientCertificate = null;

                        if (!string.IsNullOrWhiteSpace(headerValue))
                        {
                            byte[] bytes = StringToByteArray(headerValue);
                            clientCertificate = new X509Certificate2(bytes);
                        }

                        return clientCertificate;
                    };
                });

                if (File.Exists(config.SslFile))
                {
                    try
                    {
                        var ssl = new X509Certificate2(config.SslFile, config.SslPassword);
                        builder.WebHost.UseKestrel(a => 
                        a.ConfigureHttpsDefaults(b => 
                        b.ServerCertificate = ssl));
                    }
                    catch (Exception e)
                    {
                        LogError(e);
                    }
                }
                else
                {
                    LogError($"SSL证书找不到:{config.SslFile}");
                }
            }

            app = builder.Build();
        }
    }
}
