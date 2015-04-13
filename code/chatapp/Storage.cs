using System;
using MongoDB.Bson;
using MongoDB.Driver;

public class Storage
{
    const string  name = "chatapp";
    static string addr;
    static int    port;

    public static void SetMongo(string dbaddr, int dbport)
    {
        addr = dbaddr;
        port = dbport;
    }

    public static MongoClient Client()
    {
        MongoUrl url = new MongoUrl( string.Format("mongodb://{0}:{1}", addr, port) );
        var client = new MongoClient(url);
        return client;
    }

    public static IMongoDatabase Database() {
        var client = Client();
        return client.GetDatabase(name);
    }
}
