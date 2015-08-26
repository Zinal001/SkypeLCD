using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Zinal.SkypeLibrary
{
    public class SkypeListener : IDisposable
    {
        public event EventHandler<MessageEventArgs> MessageSeen;

        private SkypeDB _DB;

        private Timer UpdateSkypeTimer;

        private long LastSeenMessageTimestamp = -1;

        public String Username { get; set; }

        public SkypeListener(String DBFilePath, String Username)
        {
            UpdateSkypeTimer = new Timer(1000);
            UpdateSkypeTimer.AutoReset = true;
            UpdateSkypeTimer.Elapsed += new ElapsedEventHandler(UpdateSkypeTimer_Elapsed);

            this.Username = Username;

            _DB = new SkypeDB(DBFilePath);
        }

        public void ChangeDBPath(String DBFilePath)
        {
            bool wasOpen = _DB.isOpen();
            if (_DB.isOpen())
                _DB.Close();
            _DB = new SkypeDB(DBFilePath);

            if (wasOpen)
                _DB.Open();
        }

        public void Start()
        {
            _DB.Open();
            UpdateSkypeTimer.Start();
        }

        public void Stop()
        {
            _DB.Close();
            UpdateSkypeTimer.Stop();
        }

        void UpdateSkypeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_DB == null || !_DB.isOpen())
                return;

            if (LastSeenMessageTimestamp == -1)
            {
                SkypeDBRow lastMessageX = _DB.GetCustom("Messages", new String[] { "timestamp" }, null, "timestamp DESC").FirstOrDefault();
                Message msgX = SkypeClassConverter.ConvertTo<Message>(lastMessageX);
                LastSeenMessageTimestamp = msgX.timestampLong;
            }

            SkypeDBRow[] lastMessages = _DB.GetCustom("Messages", new String[] { "*" }, "timestamp > " + LastSeenMessageTimestamp + " AND author != '" + Username + "'", "timestamp ASC");

            if (lastMessages.Length > 0)
            {
                foreach (SkypeDBRow row in lastMessages)
                {
                    Message msg = SkypeClassConverter.ConvertTo<Message>(row);
                    OnMessageSeen(msg);
                }

                LastSeenMessageTimestamp = SkypeClassConverter.ConvertTo<Message>(lastMessages.Last()).timestampLong;
            }
        }

        private void OnMessageSeen(Message Msg)
        {
            if (MessageSeen != null)
                MessageSeen(this, new MessageEventArgs(Msg));
        }

        public void Dispose()
        {
            if (_DB.isOpen())
                _DB.Close();

            UpdateSkypeTimer.Dispose();
        }
    }

    public class MessageEventArgs : EventArgs
    {
        public Message Msg { get; private set; }

        public MessageEventArgs(Message Msg)
        {
            this.Msg = Msg;
        }
    }

}
