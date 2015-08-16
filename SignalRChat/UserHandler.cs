using System.Collections.Generic;
using System.Linq;

namespace SignalRChat {

  public class UserHandler {
    // key = Context.ConnectionId, value = user name
    private readonly Dictionary<string, string> _connectedUsers = new Dictionary<string, string>();

    public int Count {
      get {
        return _connectedUsers.Count;
      }
    }

    // Returns false if user exists
    public bool AddUser(string connectionId, string userName) {
      lock (_connectedUsers) {
        if (!_connectedUsers.ContainsKey(connectionId)) {
          _connectedUsers.Add(connectionId, userName);
          return true;
        }
      }
      return false;
    }

    // Returns false if connectionId is not found
    public bool RemoveUser(string connectionId) {
      lock (_connectedUsers) {
        return _connectedUsers.Remove(connectionId);
      }
    }

    public string[] GetUserNames() {
      return _connectedUsers.Values.OrderBy(x => x).ToArray();
    }


  }


}