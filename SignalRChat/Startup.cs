using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Owin;
using Ninject;
using Owin;

[assembly: OwinStartup(typeof(SignalRChat.Startup))]
namespace SignalRChat {
  public class Startup {
    public void Configuration(IAppBuilder app) {
      var kernel = new StandardKernel();
      var resolver = new NinjectSignalRDependencyResolver(kernel);

      kernel.Bind<IChat>()
        .To<SignalRChat.Chat>()  // Bind to Chat.
        .InSingletonScope();  // Make it a singleton object.

      kernel.Bind(typeof(IHubConnectionContext<dynamic>)).ToMethod(context =>
              resolver.Resolve<IConnectionManager>().GetHubContext<ChatHub>().Clients
               ).WhenInjectedInto<IChat>();

      kernel.Bind<IDbHandler>()
        .To<SignalRChat.DbHandler>()  // Bind to DbHandler.
        .InSingletonScope();  // Make it a singleton object.

      var config = new HubConfiguration();
      config.Resolver = resolver;

      SignalRChat.Startup.ConfigureSignalR(app, config);

      //app.MapSignalR();
    }

    public static void ConfigureSignalR(IAppBuilder app, HubConfiguration config) {
      app.MapSignalR(config);
    }
  }
}