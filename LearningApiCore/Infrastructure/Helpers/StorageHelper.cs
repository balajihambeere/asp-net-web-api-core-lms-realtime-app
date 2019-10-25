namespace LearningApiCore.Infrastructure.Helpers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Table;
    using System;
    using System.Threading.Tasks;

    public static class StorageHelper
    {
        public static async Task<string> BlobStorage(IFormFile file)
        {
            // Retrieve storage account from connection string.
            var conn = "DefaultEndpointsProtocol=https;AccountName=yaatvsa;AccountKey=rUNwkih0h77TI6KbnDgJvEyoLDJnkTw9YQ5nwiry5N4KNZl8fcKxFcPpKa5uPx3bUFaX8Rwo7R46qXwM0nalwA==;EndpointSuffix=core.windows.net";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(conn);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("learningcontainer");

            // Create the container if it doesn't already exist.
            await container.CreateIfNotExistsAsync();
            string filename = Guid.NewGuid().ToString() + ".jpg";

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(filename);

            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = file.OpenReadStream())
            {
                await blockBlob.UploadFromStreamAsync(fileStream);
            }

            return blockBlob.Uri.AbsoluteUri;
        }

        public static async Task<CloudTable> TableStorage()
        {
            // Retrieve storage account from connection string.
            var conn = "DefaultEndpointsProtocol=https;AccountName=yaatvsa;AccountKey=rUNwkih0h77TI6KbnDgJvEyoLDJnkTw9YQ5nwiry5N4KNZl8fcKxFcPpKa5uPx3bUFaX8Rwo7R46qXwM0nalwA==;EndpointSuffix=core.windows.net";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(conn);

            // Create the blob client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve reference to a previously created container.
            CloudTable learningtable = tableClient.GetTableReference("learningtable");

            // Create the container if it doesn't already exist.
            await learningtable.CreateIfNotExistsAsync();
            return learningtable;
        }

        public static async Task<TableResult> InsertOrMerge(string partitionKey, string rowkey, string content)
        {
            CloudTable learningtable = await TableStorage();
            BlogEntity entity = new BlogEntity(partitionKey, rowkey)
            {
                Content = content
            };

            // Create the TableOperation that inserts the customer entity.
            TableOperation insertOperation = TableOperation.InsertOrMerge(entity);

            // Execute the insert operation.
            return await learningtable.ExecuteAsync(insertOperation);
        }


        //public static async Task<TableResult> UpdateTable(string partitionKey, string rowkey, string content)
        //{
        //    CloudTable learningtable = await TableStorage();

        //    BlogEntity entity = new BlogEntity(partitionKey, rowkey)
        //    {
        //        Content = content
        //    };

        //    // Create the TableOperation that inserts the customer entity.
        //    TableOperation mergeOperation = TableOperation.InsertOrMerge(entity);

        //    // Execute the merge operation.
        //    return await learningtable.ExecuteAsync(mergeOperation);
        //}


        public static async Task<BlogEntity> GetTableEntity(string partitionKey, string rowkey)
        {
            CloudTable learningtable = await TableStorage();

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<BlogEntity>(partitionKey, rowkey);

            // Execute the retrieve operation.
            TableResult retrievedResult = await learningtable.ExecuteAsync(retrieveOperation);

            // Print the phone number of the result.
            return (BlogEntity)retrievedResult.Result;
        }

        public class BlogEntity : TableEntity
        {
            public BlogEntity() { }
            public BlogEntity(string rowId, string primaryKey)
            {
                PartitionKey = rowId;
                RowKey = primaryKey;
            }
            public string Key { get; set; }
            public string Content { get; set; }
        }
    }
}
