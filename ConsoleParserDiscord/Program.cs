using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Need WebSocketSharp package and Newnotsoft package

namespace ConsoleParserDiscord
{
    internal class Program
    {
        static void Main()
        {
            string token = "TOKEN"; // YOUR TOKEN HERE
            var socket = new WebSocket($"wss://gateway.discord.gg/?v=10&encoding=json");
            socket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;


            // Message receiving handler. You can change it for yourself
            socket.OnMessage += (sender, e) =>
            {
                JObject message = JObject.Parse(e.Data);    // Parse response Data to JSON

                if (message["t"] != null && message["d"] != null)   // Check if this message
                {
                    string eventType = message["t"].ToString();     // Type response
                    JObject eventData = message["d"] as JObject;    // Data into response

                    if (eventType == "MESSAGE_CREATE" && eventData["guild_id"] == null) // Check if this DM message
                    {
                        string author = eventData["author"]["username"].ToString(); // Get author
                        string content = eventData["content"].ToString();   // Get content (message text)
                        Console.WriteLine("Received a direct message: " + content + " FROM: " + author); 
                    }
                }
            };

            // Open socket handler
            socket.OnOpen += (sender, e) =>
            {
                Console.WriteLine("Connection..");
                string payload = $@"{{
                    ""op"": 2,
                    ""d"": {{
                        ""token"": ""{token}"",
                        ""properties"": {{
                            ""$os"": ""windows"",
                            ""$browser"": ""my_library"",
                            ""$device"": ""my_library""
                        }}
                    }}
                }}";

                socket.Send(payload);
                Console.WriteLine("Good connect");
            };

            // Error socket handler
            socket.OnError += (sender, e) =>
            {
                Console.WriteLine("Error: " + e.Message);
            };

            // Close socket handler
            socket.OnClose += (sender, e) =>
            {
                Console.WriteLine("Closed: " + e.Reason);
            };


            socket.Connect(); // Start connect socket

            while (true)
            {
                // Set while for infinity work socket
                System.Threading.Thread.Sleep(10000);
            }

        }
    }
}
