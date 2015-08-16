using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;


namespace SignalRChat {
  public interface IDbHandler {
    void SaveMessageToTableStorage(string user, string message);
    IQueryable GetLatestMessages(int minutes);

  }

  public class DbHandler : IDbHandler {

    private static CloudTable GetConnectionTable(string table) {
      string account = CloudConfigurationManager.GetSetting("StorageAccountName");
      string key = CloudConfigurationManager.GetSetting("StorageAccountAccessKey");
      string connectionString = String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", account, key);
      var storageAccount = CloudStorageAccount.Parse(connectionString);
      //var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
      var tableClient = storageAccount.CreateCloudTableClient();
      return tableClient.GetTableReference(table);
    }

    public void SaveMessageToTableStorage(string user, string message) {
      CloudTable cloudTable = GetConnectionTable("Message");
      cloudTable.CreateIfNotExists();
      MessageEntity messageEntity = new MessageEntity(DateTime.Now, user, message);

      // Create the TableOperation object that inserts the customer entity.
      TableOperation insertOperation = TableOperation.Insert(messageEntity);

      cloudTable.Execute(insertOperation);
    }

    // Gets latest messages
    // minutes = how many minutes from past to fetch latest messages
    public IQueryable GetLatestMessages(int minutes) {
      CloudTable cloudTable = GetConnectionTable("Message");
      cloudTable.CreateIfNotExists();

      var ticksAgo = DateTime.Now.ToUniversalTime().Subtract(new TimeSpan(0, minutes, 0)).Ticks;

      var result = (from entity in cloudTable.CreateQuery<MessageEntity>() where entity.ticks > ticksAgo select entity);

      return result;
    }


  }
}