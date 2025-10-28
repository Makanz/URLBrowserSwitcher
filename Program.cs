using Microsoft.Extensions.Configuration;
using Serilog;
using System.Diagnostics;

Log.Logger = new LoggerConfiguration()
#if DEBUG
    .WriteTo.Console()
#endif
    .WriteTo.File(@"C:\temp\URLBrowserSwitcher_log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

Log.Information("Application Starting");

// Log the command line arguments and their count and values and the current directory
Log.Information("Command line arguments: {CommandLineArguments}", args);
Log.Information("Command line arguments count: {CommandLineArgumentsCount}", args.Length);
Log.Information("Current directory: {CurrentDirectory}", Directory.GetCurrentDirectory());

try
{
    // Get the directory of the executable
    var directory = AppDomain.CurrentDomain.BaseDirectory;

    if (directory == null)
    {
        Log.Error("Could not determine the directory of the executable");
        return;
    }

    var pathToAppsettings = Path.Combine(directory, "appsettings.json");
    Log.Information("Path to appsettings.json: {PathToAppsettings}", pathToAppsettings);

    if (args.Length == 0)
    {
        Log.Warning("No URL provided.");
        return;
    }

    // The URL passed to the program
    string url = args[0];

    // Clean up the URL remove any whitespace and remove everything before http in the URL
    url = url.Trim();
    int httpIndex = url.IndexOf("http", StringComparison.OrdinalIgnoreCase);
    if (httpIndex == -1)
    {
        Log.Error("URL does not contain 'http' or 'https': {Url}", url);
        return;
    }
    url = url.Substring(httpIndex);

    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile(pathToAppsettings, optional: true, reloadOnChange: true)
        .Build();

    // Check if configuration is null
    if (configuration == null)
    {
        Log.Error("Configuration is null");
        return;
    }

    // Load chromePath from settings
    var defaultBrowser = configuration["defaultBrowser"];
    var alternativeBrowser = configuration["alternativeBrowser"];
    var urls = configuration.GetSection("urls").GetChildren().Select(x => x.Value).ToList();

    // Validate configuration values
    if (string.IsNullOrEmpty(defaultBrowser) || string.IsNullOrEmpty(alternativeBrowser))
    {
        Log.Error("Please provide the path to Chrome and Brave in appsettings.json");
        return;
    }

    // Decide which browser to use
    string browserPath;

    // Create a Uri object from the URL
    var urlDomainObject = new Uri(url);

    // Extract the host from the URL and normalize it by removing www. prefix if present
    var host = urlDomainObject.Host;
    if (host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
    {
        host = host.Substring(4);
    }

    // Extract domains from configured URLs and check if the current host matches any of them
    var configuredDomains = urls
        .Where(x => x != null)
        .Select(x =>
        {
            try
            {
                var uri = new Uri(x);
                var domain = uri.Host;
                // Normalize by removing www. prefix
                if (domain.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                {
                    domain = domain.Substring(4);
                }
                return domain;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to parse configured URL: {Url}", x);
                return null;
            }
        })
        .Where(x => x != null)
        .ToList();

    // Check if the current host matches any of the configured domains
    if (configuredDomains.Any(domain => host.Equals(domain, StringComparison.OrdinalIgnoreCase)))
    {
        browserPath = alternativeBrowser;
    }
    else
    {
        browserPath = defaultBrowser;
    }

    // Open the URL in the selected browser
    Process.Start(new ProcessStartInfo
    {
        FileName = browserPath,
        Arguments = url,
        UseShellExecute = true
    });

    Log.Information("Opened {Url} in {Browser}", url, browserPath);
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occurred");
}
finally
{
    Log.CloseAndFlush();
}
