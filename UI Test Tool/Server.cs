using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alchemy;
using Alchemy.Classes;
using Newtonsoft.Json;

namespace UI_Test_Tool
{
    class Server
    {

        protected ConcurrentDictionary<User, string> OnlineUsers = new ConcurrentDictionary<User, string>();

        public Server()
        {
            
        }

        public void StartServer()
        {
            var aServer = new WebSocketServer(81, IPAddress.Any)
            {
                OnReceive = OnReceive,
                OnSend = OnSend,
                OnConnected = OnConnect,
                OnDisconnect = OnDisconnect,
                
                //5min timeout
                TimeOut = new TimeSpan(0, 5, 0)
            };

            aServer.Start();

            // Accept comands on the console and keep it alive
            var command = string.Empty;
            while (command != "exit")
            {
                command = Console.ReadLine();
            }

            aServer.Stop();
        }


        /// <summary>
        /// Event fired when a client connects to the Alchemy Websockets server instance.
        /// Adds the client to the online users list.
        /// </summary>
        /// <param name="context">The user's connection context</param>
        public void OnConnect(UserContext context)
        {
            Console.WriteLine("Client Connection From : " + context.ClientAddress);

            var me = new User { Context = context };

            OnlineUsers.TryAdd(me, String.Empty);
        }

        /// <summary>
        /// Event fired when a data is received from the Alchemy Websockets server instance.
        /// Parses data as JSON and calls the appropriate message or sends an error message.
        /// </summary>
        /// <param name="context">The user's connection context</param>
        public void OnReceive(UserContext context)
        {
            Console.WriteLine("Received Data From :" + context.ClientAddress);

            try
            {
                var json = context.DataFrame.ToString();

                // <3 dynamics
                dynamic obj = JsonConvert.DeserializeObject(json);

                switch ((int)obj.Type)
                {
                    case (int)CommandType.Register:
                        Register(obj.Name.Value, context);
                        break;
                    case (int)CommandType.Message:
                        ChatMessage(obj.Message.Value, context);
                        break;
                    case (int)CommandType.NameChange:
                        NameChange(obj.Name.Value, context);
                        break;
                }
            }
            catch (Exception e) // Bad JSON! For shame.
            {
                var r = new Response { Type = ResponseType.Error, Data = new { e.Message } };

                context.Send(JsonConvert.SerializeObject(r));
            }
        }

        /// <summary>
        /// Event fired when the Alchemy Websockets server instance sends data to a client.
        /// Logs the data to the console and performs no further action.
        /// </summary>
        /// <param name="context">The user's connection context</param>
        public void OnSend(UserContext context)
        {
            Console.WriteLine("Data Send To : " + context.ClientAddress);
        }

        /// <summary>
        /// Event fired when a client disconnects from the Alchemy Websockets server instance.
        /// Removes the user from the online users list and broadcasts the disconnection message
        /// to all connected users.
        /// </summary>
        /// <param name="context">The user's connection context</param>
        public void OnDisconnect(UserContext context)
        {
            Console.WriteLine("Client Disconnected : " + context.ClientAddress);
            var user = OnlineUsers.Keys.Where(o => o.Context.ClientAddress == context.ClientAddress).Single();

            string trash; // Concurrent dictionaries make things weird

            OnlineUsers.TryRemove(user, out trash);

            if (!String.IsNullOrEmpty(user.Name))
            {
                var r = new Response { Type = ResponseType.Disconnect, Data = new { user.Name } };

                Broadcast(JsonConvert.SerializeObject(r));
            }

            BroadcastNameList();
        }

        /// <summary>
        /// Register a user's context for the first time with a username, and add it to the list of online users
        /// </summary>
        /// <param name="name">The name to register the user under</param>
        /// <param name="context">The user's connection context</param>
        private void Register(string name, UserContext context)
        {
            var u = OnlineUsers.Keys.Where(o => o.Context.ClientAddress == context.ClientAddress).Single();
            var r = new Response();

            if (ValidateName(name))
            {
                u.Name = name;

                r.Type = ResponseType.Connection;
                r.Data = new { u.Name };

                Broadcast(JsonConvert.SerializeObject(r));

                BroadcastNameList();
                OnlineUsers[u] = name;
            }
            else
            {
                SendError("Name is of incorrect length.", context);
            }
        }

        /// <summary>
        /// Broadcasts a chat message to all online users
        /// </summary>
        /// <param name="message">The chat message to be broadcasted</param>
        /// <param name="context">The user's connection context</param>
        private void ChatMessage(string message, UserContext context)
        {
            var u = OnlineUsers.Keys.Where(o => o.Context.ClientAddress == context.ClientAddress).Single();
            var r = new Response { Type = ResponseType.Message, Data = new { u.Name, Message = message } };

            Broadcast(JsonConvert.SerializeObject(r));

        }

        /// <summary>
        /// Update a user's name if they sent a name-change command from the client.
        /// </summary>
        /// <param name="name">The name to be changed to</param>
        /// <param name="aContext">The user's connection context</param>
        private void NameChange(string name, UserContext aContext)
        {
            var u = OnlineUsers.Keys.Where(o => o.Context.ClientAddress == aContext.ClientAddress).Single();

            if (ValidateName(name))
            {
                var r = new Response
                                 {
                                     Type = ResponseType.NameChange,
                                     Data = new { Message = u.Name + " is now known as " + name }
                                 };
                Broadcast(JsonConvert.SerializeObject(r));

                u.Name = name;
                OnlineUsers[u] = name;

                BroadcastNameList();
            }
            else
            {
                SendError("Name is of incorrect length.", aContext);
            }
        }

        /// <summary>
        /// Broadcasts an error message to the client who caused the error
        /// </summary>
        /// <param name="errorMessage">Details of the error</param>
        /// <param name="context">The user's connection context</param>
        private void SendError(string errorMessage, UserContext context)
        {
            var r = new Response { Type = ResponseType.Error, Data = new { Message = errorMessage } };

            context.Send(JsonConvert.SerializeObject(r));
        }

        /// <summary>
        /// Broadcasts a list of all online users to all online users
        /// </summary>
        private void BroadcastNameList()
        {
            var r = new Response
                        {
                            Type = ResponseType.UserCount,
                            Data = new { Users = OnlineUsers.Values.Where(o => !String.IsNullOrEmpty(o)).ToArray() }
                        };
            Broadcast(JsonConvert.SerializeObject(r));
        }

        /// <summary>
        /// Broadcasts a message to all users, or if users is populated, a select list of users
        /// </summary>
        /// <param name="message">Message to be broadcast</param>
        /// <param name="users">Optional list of users to broadcast to. If null, broadcasts to all. Defaults to null.</param>
        private void Broadcast(string message, ICollection<User> users = null)
        {
            if (users == null)
            {
                foreach (var u in OnlineUsers.Keys)
                {
                    u.Context.Send(message);
                }
            }
            else
            {
                foreach (var u in OnlineUsers.Keys.Where(users.Contains))
                {
                    u.Context.Send(message);
                }
            }
        }

        /// <summary>
        /// Checks validity of a user's name
        /// </summary>
        /// <param name="name">Name to check</param>
        /// <returns></returns>
        private bool ValidateName(string name)
        {
            var isValid = false;
            if (name.Length > 3 && name.Length < 25)
            {
                isValid = true;
            }

            return isValid;
        }

        /// <summary>
        /// Defines the type of response to send back to the client for parsing logic
        /// </summary>
        public enum ResponseType
        {
            Connection = 0,
            Disconnect = 1,
            Message = 2,
            NameChange = 3,
            UserCount = 4,
            Error = 255
        }

        /// <summary>
        /// Defines the response object to send back to the client
        /// </summary>
        public class Response
        {
            public ResponseType Type { get; set; }
            public dynamic Data { get; set; }
        }

        /// <summary>
        /// Holds the name and context instance for an online user
        /// </summary>
        public class User
        {
            public string Name = String.Empty;
            public UserContext Context { get; set; }
        }

        /// <summary>
        /// Defines a type of command that the client sends to the server
        /// </summary>
        public enum CommandType
        {
            Register = 0,
            Message,
            NameChange
        }
    }
}
