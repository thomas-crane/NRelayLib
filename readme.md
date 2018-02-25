# NRelayLib
A C# Library which provides an event driven method of communicating with nrelay.

## Example usage: KRelay plugin
The following code is an example of how to use NRelayLib in a KRelay plugin to provide communication between nrelay and KRelay
```csharp
using System;
using Lib_K_Relay.Interface;
using Lib_K_Relay;
using Lib_K_Relay.Utilities;
using Lib_K_Relay.Networking;
using NRelay.Server;
using NRelay.Server.Events;

namespace NRelayServerTestPlugin
{
    public class NRelayServerTestPlugin : IPlugin
    {
        public string GetAuthor() => "tcrane";
        public string[] GetCommands() => null;
        public string GetDescription() => "A plugin to test connecting to the nrelay local server.";
        public string GetName() => "nrelay Server Test";

        private Client client;

        public void Initialize(Proxy proxy)
        {
            // Create the connection object.
            var connection = new Connection();
            
            // Attach the event listeners.
            connection.ConnectedToServer += OnNRelayConnected;
            connection.MessageReceived += NRelayMessageReceived;
            connection.Error += NRelayError;

            proxy.ClientConnected += (client) =>
            {
                this.client = client;
            };

            // A method to test sending data to nrelay.
            proxy.HookCommand("testsend", (client, cmd, args) =>
            {
                client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Sending test message"));
                connection.Write("Test Message sent at " + DateTime.Now.ToShortTimeString());
            });

            // Delay the connection by a second to allow KRelay to initialize.
            PluginUtils.Delay(1000, () =>
            {
                connection.Connect();
            });
        }

        // Invoked when an error occurs.
        private void NRelayError(object sender, ServerErrorEventArgs args)
        {
            PluginUtils.Log("NRelay Error", args.Error.Message);
        }

        // Invoked when a message is received from the local server.
        private void NRelayMessageReceived(object sender, MessageEventArgs args)
        {
            if (client?.Connected ?? false)
            {
                client.SendToClient(PluginUtils.CreateOryxNotification("NRelay", args.Payload));
            }
        }

        // Invoked when the the plugin connects to the local server.
        private void OnNRelayConnected(object sender, ConnectedToServerEventArgs args)
        {
            PluginUtils.Log("NRelay", "Connected to local server");
        }
    }
}
```
