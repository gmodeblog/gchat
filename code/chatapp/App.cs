using System;
using WebSocketSharp;
using WebSocketSharp.Server;

public class App
{
    public static void Main (string[] args)
    {
        // DB設定
        var dbaddr = System.Environment.GetEnvironmentVariable("DB_PORT_27017_TCP_ADDR");
        var dbport = int.Parse(System.Environment.GetEnvironmentVariable("DB_PORT_27017_TCP_PORT"));
        Storage.SetMongo(dbaddr, dbport);

        var server = new WebSocketServer (System.Net.IPAddress.Any, 57200);
        server.AddWebSocketService<ChatService> ("/chat");
        server.Start ();

        Console.WriteLine ("\nPress Enter key to stop the server...");
        Console.ReadLine ();

        server.Stop ();
    }
}
