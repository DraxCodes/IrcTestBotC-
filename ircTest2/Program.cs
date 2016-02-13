using System;
using System.Net.Sockets;
using System.IO;

namespace ircTest2 {

    class IrcClient {
        private string userName;
        private TcpClient tcpClient;
        private StreamReader inputStream;
        private StreamWriter outputStream;

        public IrcClient ( string ip, int port ) {
            tcpClient = new TcpClient(ip, port);
            inputStream = new StreamReader(tcpClient.GetStream());
            outputStream = new StreamWriter(tcpClient.GetStream());
            registerConection("WarzoneBotTest", "Warzone", true);
        }

        /// <summary>
        /// Intial Bot Registration
        /// </summary>
        /// <param name="nick">Bot Nickname as String</param>
        /// <param name="realName">Owner Name as String</param>
        /// <param name="visibilty">True: Visibile - False: None Visible</param>
        private void registerConection ( string nick, string realName, bool visibilty ) {
            this.userName = nick;
            int isInvisible = visibilty ? 0 : 8;

            outputStream.WriteLine("USER {0} {1} * :{2}", nick, isInvisible, realName);
            outputStream.Flush();
            outputStream.WriteLine("NICK {0}", nick);
            outputStream.Flush();
        }
        public void joinRoom (string channel) {
            if ( channel.Contains("#") != true ) {
                outputStream.WriteLine("JOIN #{0}", channel); outputStream.Flush();
            } else { outputStream.WriteLine("JOIN {0}", channel); outputStream.Flush(); }
        }

        public void sendIrcMessage (string message) {
            outputStream.WriteLine(message); outputStream.Flush();
        }

        public void sendChatMessage (string message, string channel) {
            outputStream.WriteLine("MSG {0} {1}", channel, message);
            outputStream.Flush();
        }

        public string readMessage () {
            var message = inputStream.ReadLine();
            return message;
        }

        public void PONG (string ping) {
            char[] del = { ':' };
            string[] splitPING = ping.Split(del);
            outputStream.WriteLine("PONG :{0}", splitPING[1]);
            Console.WriteLine(">>> PONG :{0}", splitPING[1]);
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

