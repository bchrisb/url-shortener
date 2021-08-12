using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;

namespace UrlShortener.Repository
{
    public class TableStorageRepository<T> : ITableStorageRepository<T> where T : ITableEntity, new()
    {
        private readonly CloudTable _cloudTable;

        /// <summary>
        /// Instantiates a new <see cref="TableStorageRepository{T}"/>.
        /// </summary>
        /// <param name="cloudTableClient">The Cloud Table SDK client.</param>
        /// <param name="tableName">The name of the table in Azure table store.</param>
        public TableStorageRepository(CloudTableClient cloudTableClient, string tableName)
        {
            cloudTableClient.DefaultRequestOptions.RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(2), 5);

            _cloudTable = cloudTableClient.GetTableReference(tableName);
            _cloudTable.CreateIfNotExistsAsync().Wait();
        }

        public async Task<T> GetAsync(string partitionKey, string rowKey)
        {
            var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var tableResult = await _cloudTable.ExecuteAsync(retrieveOperation);

            if (retrieveOperation == null) return default(T);

            var result = (T)tableResult.Result;

            if (result != null)
            {
                return result;
            }
            return default(T);
        }

        public async Task<IList<T>> GetByPartitionKeyAsync(string partitionKey)
        {
            var resultList = new List<T>();
            var query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

            TableContinuationToken continuationToken = null;

            do
            {
                var queryResult = await _cloudTable.ExecuteQuerySegmentedAsync(query, continuationToken);
                resultList.AddRange(queryResult);
                continuationToken = queryResult.ContinuationToken;
            } while (continuationToken != null);

            return resultList;
        }

        public async Task<T> InsertAsync(T entity)
        {
            var saveOperation = TableOperation.Insert(entity);

            return (T)(await _cloudTable.ExecuteAsync(saveOperation)).Result;
        }

        public async Task<T> ReplaceAsync(T entity)
        {
            var updateOperation = TableOperation.Replace(entity);

            return (T)(await _cloudTable.ExecuteAsync(updateOperation)).Result;
        }

        public async Task<T> UpsertAsync(T entity)
        {
            var saveOperation = TableOperation.InsertOrReplace(entity);

            return (T)(await _cloudTable.ExecuteAsync(saveOperation)).Result;
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            var deleteOperation = TableOperation.Delete(entity);

            await _cloudTable.ExecuteAsync(deleteOperation);

            return true;
        }

        public async Task<bool> DeleteAsync(string partitionKey, string rowKey)
        {
            var entity = await GetAsync(partitionKey, rowKey);

            if (entity != null)
            {
                return await DeleteAsync(entity);
            }
            return false;
        }
    }
}
