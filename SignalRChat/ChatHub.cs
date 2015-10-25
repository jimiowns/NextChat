using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;


namespace SignalRChat {
  public class ChatHub : Hub {

    private readonly IChat _chat;
    private readonly IDbHandler _dbHandler;

    public ChatHub(IChat chat, IDbHandler dbHandler) {
      if (chat == null) {
        throw new ArgumentNullException("chat");
      }
      _chat = chat;

      if (dbHandler == null) {
        throw new ArgumentNullException("dbHandler");
      }
      _dbHandler = dbHandler;
    }

    public void Send(string name, string message) {
      // Call the broadcastMessage method to update clients.
      _chat.BroadcastMessage(name, message);
      _dbHandler.SaveMessageToTableStorage(name, message);
    }

    // Broadcast users
    private void BroadcastUsers() {
      _chat.BroadcastUsers();
    }

    // Add user
    public void AddUser(string userName) {
      _chat.BroadcastMessagesToCaller(Context.ConnectionId, _dbHandler.GetLatestMessages(10));
      // Sleep for a second so that latest messages arrive first
      Thread.Sleep(1000);
      _chat.AddUser(Context.ConnectionId, userName);
    }

    // On connected
    public override Task OnConnected() {
      // AddUser is called from javascript because username is not given yet

      return base.OnConnected();
    }

    public override Task OnDisconnected(bool stopCalled) {
      _chat.RemoveUser(Context.ConnectionId);
      return base.OnDisconnected(stopCalled);
    }

  }
}