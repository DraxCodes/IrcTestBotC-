using System;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading;

namespace ircTest2 {

    class IrcClient {
        private string userName;
        private TcpClient tcpClient;
        private StreamReader inputStream;
        private StreamWriter outputStream;

        public IrcClient ( string ip, int port ) {
            try {
                tcpClient = new TcpClient(ip, port);
                inputStream = new StreamReader(tcpClient.GetStream());
                outputStream = new StreamWriter(tcpClient.GetStream());
            } catch {
                Console.WriteLine("Connection Error");
                throw;
            }
            try {
                if (inputStream.ReadLine().Contains("PING")) {

                }
                registerConection("WarzoneBotTest", "Warzone", "JPC", "p2p", true);
            } catch {
                Console.WriteLine("Register Error");
                throw;
            }
        }

        /// <summary>
        /// Intial Bot Registration
        /// </summary>
        /// <param name="nick">Bot Nickname as String</param>
        /// <param name="realName">Owner Name as String</param>
        /// <param name="visibilty">True: Visibile - False: None Visible</param>
        private void registerConection ( string nick, string realName, string hostName, string serverName, bool visibilty ) {
            this.userName = nick;
            int isInvisible = visibilty ? 0 : 8;
            string test = "USER " + nick + " " + isInvisible + " * :" + realName;
            byte[] bytes = Encoding.Default.GetBytes(test);
            var data = Encoding.UTF8.GetString(bytes);

            // outputStream.Write("USER {0} {1} * :{2}\r\n", nick, isInvisible, realName);#
            outputStream.Write(data);
            Console.WriteLine("<<< " + data);
            outputStream.Flush();
            outputStream.Write("NICK {0}", nick);
            outputStream.Flush();
        }

        public void joinRoom (string channel) {
            if ( channel.Contains("#") != true ) {
                outputStream.Write("JOIN #{0}\r\n", channel); outputStream.Flush();
            } else { outputStream.Write("JOIN {0}\r\n", channel); outputStream.Flush(); }
        }

        public void sendIrcMessage (string message) {
            outputStream.WriteLine(message); outputStream.Flush();
        }

        public void sendChatMessage (string message, string channel) {
            outputStream.Write("PRIVMSG {0} {1}\r\n", channel, message);
            outputStream.Flush();
        }

        public string readMessage () {
            var message = inputStream.ReadLine();
            return message;
        }

        public void PONG (string ping) {
            string[] splitPING = ping.Split(':');
            outputStream.Write("PONG {0}\r\n", splitPING[1]);
            Console.WriteLine("<<< PONG :{0}", splitPING[1]);
        }
    }
    class Program {
        static void Main ( string[] args ) {
            string channel = "#WarzoneAUTODL";
            IrcClient irc = new IrcClient("irc.p2p-network.net", 6667);
            irc.joinRoom(channel);
            while ( true ) {
                string message = irc.readMessage();
                Console.WriteLine(">>> {0}", message);
                if ( message.Contains("PING") ) {
                    irc.PONG(message);
                }
            }
        }
    }
}

