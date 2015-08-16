using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace SignalRChat {
  public class MessageEntity : TableEntity {
    private const string MessagePartionKey = "Message";

    public MessageEntity(DateTime dateTime, string userName, string message) {
      this.PartitionKey = MessagePartionKey;
      this.RowKey = Convert.ToString(dateTime.ToUniversalTime().Ticks);
      this.dateTime = dateTime;
      this.ticks = dateTime.ToUniversalTime().Ticks;
      this.userName = userName;
      this.message = message;
    }

    public MessageEntity() {
      this.PartitionKey = MessagePartionKey;
      this.RowKey = dateTime.ToUniversalTime().Ticks + " " + Guid.NewGuid();
    }

    public DateTime dateTime { get; set; }
    public long ticks { get; set; }
    public string userName { get; set; }
    public string message { get; set; }

  }
}