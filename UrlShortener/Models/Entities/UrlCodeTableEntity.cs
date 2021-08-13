using Microsoft.WindowsAzure.Storage.Table;

namespace UrlShortener.Models.Entities
{
    public class UrlCodeTableEntity : TableEntity
    {
        public string FullUrl { get; set; }

        public override string ToString()
        {
            return $"PartitionKey: {PartitionKey} | RowKey: {RowKey}";
        }
    }
}
