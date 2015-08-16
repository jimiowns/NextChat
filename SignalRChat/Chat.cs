using System;
using System.Linq;
using System.Web.Script.Serialization;
using Microsoft.AspNet.SignalR.Hubs;

namespace SignalRChat {

  public interface IChat {
    IHubConnectionContext<object> Clients { get; set; }
    void BroadcastMessage(string name, string message);
    void BroadcastUsers();
    void AddUser(string connectionId, string userName);
    void RemoveUser(string connectionId);
    void BroadcastMessageToClient(string connectionId, string name, string message, DateTime dateTime);
    void BroadcastMessagesToCaller(string connectionId, IQueryable messageEntityQuery);
  }

  public class Chat : IChat {

    public IHubConnectionContext<dynamic> Clients { get; set; }

    public Chat(IHubConnectionContext<dynamic> clients) {
      if (clients == null) {
        throw new ArgumentNullException("clients");
      }
      Clients = clients;
    }

    private readonly static UserHandler userHandler = new UserHandler();

    public void BroadcastMessage(string name, string message) {
      Clients.All.broadcastMessage(System.DateTime.Now.ToString("[HH:mm] ") + name, message);
    }

    public void BroadcastMessageToClient(string connectionId, string name, string message, DateTime dateTime) {
      Clients.Client(connectionId).broadcastMessage(dateTime.ToString("[HH:mm] ") + name, message);
    }

    public void BroadcastMessagesToCaller(string connectionId, IQueryable messageEntityQuery) {
      foreach (MessageEntity message in messageEntityQuery) {
        BroadcastMessageToClient(connectionId, message.userName, message.message, message.dateTime.ToLocalTime());
      }
    }

    public void BroadcastUsers() {
      var serialize = new JavaScriptSerializer();
      var users = userHandler.GetUserNames();
      Clients.All.broadcastUsers(serialize.Serialize(users));
    }

    public void AddUser(string connectionId, string userName) {
      if (userHandler.AddUser(connectionId, userName)) {
        // Broadcasts users to clients
        BroadcastUsers();
        // Broadcasts new user joined message to clients
        Clients.All.broadcastMessage("***", userName + " has joined");
      }
      else {
        // todo handle duplicates
      }
    }

    public void RemoveUser(string connectionId) {
      userHandler.RemoveUser(connectionId);
      BroadcastUsers();
    }

  }
}