using Microsoft.WindowsAzure.Storage.Table;

namespace Dimps.Extensions.Configuration.AzureTableStorage
{
    internal class ConfigurationEntry : TableEntity
    {
        public string Value { get; set; }
        public bool IsActive { get; set; }
    }
}
