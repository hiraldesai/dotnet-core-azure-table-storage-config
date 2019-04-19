[![Build Status](https://dev.azure.com/dimps/AzureTableStorageConfigurationProvider/_apis/build/status/hiraldesai.AzureTableStorageConfigurationProvider?branchName=master)](https://dev.azure.com/dimps/AzureTableStorageConfigurationProvider/_build/latest?definitionId=1&branchName=master)

# Azure Table Storage Configuration Provider

This repository hosts the code for Azure Table Storage Configuration provider for .Net Core. The artifacts from this repository are [published on NuGet](https://www.nuget.org/packages/Dimps.Extensions.Configuration.AzureTableStorage) as `Dimps.Extensions.Configuration.AzureTableStorage`.

# When should I use this package?

This package can be used as a replacement for file based configuration providers from Microsoft for large solution with many projects. (e.g. [Microsoft.Extensions.Configuration.Json](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Json),  [Microsoft.Extensions.Configuration.Ini](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Ini),  [Microsoft.Extensions.Configuration.Xml](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Xml)) 

It is useful to have one centralized place for all your configuration key value pairs when you're working on a large solution with numerous micro-service projects where maintaining one Json/ini/XML file per project becomes a maintenance hassle.

# Usage

1. In ***Package Manager Console*** window of ***Visual Studio*** - run the following command to install the package

```
Install-Package Dimps.Extensions.Configuration.AzureTableStorage
```

If you prefer ***Manage NuGet Packages for Solution*** through UI instead, just search for `Dimps.Extensions.Configuration.AzureTableStorage` and install the latest

2. [Create Azure Table Storage Account](https://docs.microsoft.com/en-us/azure/storage/common/storage-quickstart-create-account?tabs=azure-portal) if you haven't created one already, obtain the storage connection string to be used in later steps.

3. [Download Azure Storage Explorer](https://azure.microsoft.com/en-us/features/storage-explorer/), sign in using your credentials and create a new table and give it a name of your liking - say `MyAppConfig`.

4. Add a few configuration entries to this newly created table in following format.

![image](https://user-images.githubusercontent.com/1005174/56399249-b1dc3980-6201-11e9-9962-47c24cf677cc.png)

* ***PartitionKey***: key specified when initializing the provider in step 6. below.
* ***RowKey***: configuration key for this entry
* ***Value***: value for the configuration key
* ***IsActive***: boolean indicating if the key is active, inactive keys aren't loaded by the provider.

5. Add following code snippet at the entry point of your application (`Program -> Main -> CreateWebHostBuilder` for .Net Core Web/Api projects or `Program -> Main` for Console Application projects.

```C#
// Sample Program.cs for .Net Core Web Projects

public class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }
        
    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((context, config) =>
                {
                    var builtConfig = config.Build();
                    var tableStorageBuilder = new ConfigurationBuilder();                    
                    // CreateDefaultBuilder adds appsettings.json based config
                    // This assumes you have defined a key "TableStorageConnectionStringKey" with the value of your
                    // Azure Table Storage connection string
                    var storageConnectionString = builtConfig["TableStorageConnectionStringKey"];
                    tableStorageBuilder.AddAzureTableStorage(storageConnectionString, "MyAppConfig", "MyAppConfigPartitionKey");
                    var storageConfig = tableStorageBuilder.Build();
                    config.AddConfiguration(storageConfig);
                });
}

```

```C#
// Sample Program.cs for .Net Core Console Applications

public class Program
{
    public static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json");
        var config = builder.Build();
        var storageConnectionString = builtConfig["TableStorageConnectionStringKey"];
        builder.AddAzureTableStorage(storageConnectionString, "MyAppConfig", "MyAppConfigPartitionKey");              
        config = builder.Build();

        // Add config to ServiceCollection or use as you like
    }
}
```

6. ***Please Note*** - for simplicity, the samples ask you to put the storage connection string in the `appsettings.json` file. However, it is recommended that production applications use something like [Azure KeyVault Provider](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.AzureKeyVault) to store secrets such as storage connection string.

