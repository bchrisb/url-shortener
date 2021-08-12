using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace UrlShortener.Repository
{
    public interface ITableStorageRepository<T> where T : ITableEntity
    {
        /// <summary>
        /// Retrieves a single item using a partition key and row key.
        /// A null is returned if the item is not found.
        /// </summary>
        /// <param name="partitionKey">The partition key of the item.</param>
        /// <param name="rowKey">The row key of the item.</param>
        /// <returns>Returns a single item.</returns>
        Task<T> GetAsync(string partitionKey, string rowKey);

        /// <summary>
        /// Retrieves a list of items using a partition key.
        /// An empty list is returned if no items are found.
        /// </summary>
        /// <param name="partitionKey">The partition key of the items to retrieve.</param>
        /// <returns>Returns a list of items.</returns>
        Task<IList<T>> GetByPartitionKeyAsync(string partitionKey);

        /// <summary>
        /// Creates a new item in table storage
        /// </summary>
        /// <param name="entity">The item to store.</param>
        /// <returns>Returns the newly created item.</returns>
        Task<T> InsertAsync(T entity);

        /// <summary>
        /// Update an item in table storage
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> ReplaceAsync(T entity);

        /// <summary>
        /// Creates a new item in table storage if one does not exist already.
        /// If an item already exists, it will be replaced.
        /// </summary>
        /// <param name="entity">The item to store.</param>
        /// <returns>Returns the newly created item.</returns>
        Task<T> UpsertAsync(T entity);

        /// <summary>
        /// Delete the item from table storage.
        /// </summary>
        /// <param name="entity">The item to delete.</param>
        /// <returns>Returns true if the item was successfully deleted.</returns>
        Task<bool> DeleteAsync(T entity);

        /// <summary>
        /// Delete an item from table storage using a partition and row key.
        /// </summary>
        /// <param name="partitionKey">The partition key of the item to delete.</param>
        /// <param name="rowKey">The row key of the item to delete.</param>
        /// <returns>Returns true if the item was successfully deleted.</returns>
        Task<bool> DeleteAsync(string partitionKey, string rowKey);
    }
}
