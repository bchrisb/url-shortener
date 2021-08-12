using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using UrlShortener;
using UrlShortener.Models.Entities;
using UrlShortener.Repository;

[assembly: FunctionsStartup(typeof(Startup))]

namespace UrlShortener
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<ICodeGenerator, CodeGenerator>();
            builder.Services.AddSingleton<ITableStorageRepository<UrlCodeTableEntity>, TableStorageRepository<UrlCodeTableEntity>>(sp => GetTableStorageRepository<UrlCodeTableEntity>("UrlCodes"));
        }

        private static TableStorageRepository<T> GetTableStorageRepository<T>(string tableName) where T : ITableEntity, new()
        {
            var cloudStorageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true"); // TODO: make a setting
            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            var tableRepo = new TableStorageRepository<T>(cloudTableClient, tableName);
            return tableRepo;
        }
    }
}