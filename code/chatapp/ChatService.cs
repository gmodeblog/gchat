using System;
using System.Collections.Generic;
using System.Linq;
using WebSocketSharp;
using WebSocketSharp.Server;

using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class MessageType
{
    public const string UserLogined    = "user_logined";
    public const string UserAdded      = "user_added";
    public const string MessageRequest = "message_request";
    public const string MessageAdding  = "message_adding";
}

public class ChatMessage {
    public ObjectId Id     { get; set; }
    public string UserName { get; set; }
    public string Comment  { get; set; }
}


public class ChatService : WebSocketBehavior {

    static int counter = 0;
    public static int NextSequence() {
        counter++;
        return counter;
    }

    protected override void OnOpen ()
    {
        // ユーザーログイン時メッセージ
        dynamic userLoginedMassage = new JObject();
        userLoginedMassage.type = MessageType.UserLogined;
        userLoginedMassage.username = "プレイヤー" +  NextSequence().ToString();

        // 自分に送信
        string json = JsonConvert.SerializeObject(userLoginedMassage);
        Send(json);

        // ユーザーが追加されたメッセージを全員に送信
        dynamic user_added = new JObject();
        user_added.type = MessageType.UserAdded;
        user_added.username = userLoginedMassage.username;
        string user_added_json = JsonConvert.SerializeObject(user_added);
        Sessions.Broadcast(user_added_json);
    }

    protected override async void OnMessage (MessageEventArgs e)
    {
        dynamic msg = JsonConvert.DeserializeObject(e.Data);
        switch( (string)msg.type ){
        case MessageType.MessageRequest:

            // DB取得
            var database = Storage.Database();
            // コレクション取得
            var collection = database.GetCollection<ChatMessage>("chat_message");
            // チャットメッセージを生成
            var chatmsg = new ChatMessage();
            chatmsg.UserName = msg.username;
            chatmsg.Comment = msg.comment;
            // データを追加
            await collection.InsertOneAsync(chatmsg);
            Console.WriteLine( string.Format("ChatMessage Id:{0}",chatmsg.Id) );

            // メッセージ追加メッセージの生成
            dynamic message_adding = new JObject();
            message_adding.type = MessageType.MessageAdding;
            message_adding.username = msg.username;
            message_adding.comment = msg.comment;
            // 全員に送信
            string json = JsonConvert.SerializeObject(message_adding);
            Sessions.Broadcast(json);
            break;
        }
    }
}
