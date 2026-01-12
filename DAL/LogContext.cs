using Azure;
using Azure.Data.Tables;
using ImageSharingWithCloud.Models;
using ImageSharingWithCloud.Models.ViewModels;

namespace ImageSharingWithCloud.DAL
{
    public class LogContext : ILogContext
    {
        private readonly TableClient _tableClient;

        private readonly ILogger<LogContext> _logger;

        public LogContext(IConfiguration configuration, ILogger<LogContext> logger)
        {
            _logger = logger;

            Uri logTableServiceUri = null;
            string logTableName = null;
            
            // TODO Get the table service URI and table name.
            logTableServiceUri = new Uri(configuration[StorageConfig.LogEntryDbUri]);
            logTableName = configuration[StorageConfig.LogEntryDbTable];
            logger.LogInformation("Looking up Storage URI... ");


            logger.LogInformation("Using Table Storage URI: {logTableServiceUri}", logTableServiceUri);
            logger.LogInformation("Using Table: {logTableName}", logTableName);
            
            // Access key will have been loaded from Secrets (Development) or Key Vault (Production)
            var credential = new TableSharedKeyCredential(
                configuration[StorageConfig.LogEntryDbAccountName],
                configuration[StorageConfig.LogEntryDbAccessKey]);

            logger.LogInformation("Initializing table client....");
            // TODO Set the table client for interacting with the table service (see TableClient constructors)
            _tableClient = new TableClient(logTableServiceUri,logTableName,credential);

            logger.LogInformation("....table client URI = {tableClientUri}", _tableClient.Uri);
        }


        public async Task AddLogEntryAsync(string userId, string userName, ImageView image)
        {
            var entry = new LogEntry(userId, image.Id)
            {
                Username = userName,
                Caption = image.Caption,
                ImageId = image.Id,
                Uri = image.Uri
            };

            _logger.LogDebug("Adding log entry for image: {imageId}", image.Id);

            Response response = null;
            // TODO add a log entry for this image view
            response = await _tableClient.AddEntityAsync(entry);

            if (response.IsError)
            {
                _logger.LogError("Failed to add log entry, HTTP response {status}", response.Status);
            } 
            else
            {
                _logger.LogDebug("Added log entry with HTTP response {status}", response.Status);
            }

        }

        public AsyncPageable<LogEntry> Logs(bool todayOnly = false)
        {
            if (todayOnly)
            {
                // TODO just return logs for today
                string todayPartitionKey = DateTime.UtcNow.ToString("MMddyyyy");
                return _tableClient.QueryAsync<LogEntry>(log => log.PartitionKey == todayPartitionKey );
                
            }
            else
            {
                return _tableClient.QueryAsync<LogEntry>(logEntry => true);
            }
        }

    }
}