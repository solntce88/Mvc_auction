using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvc_auction.Models
{
    public class Message
    {
        private string _name;
        private string _subject;
        private string _body;
        private int _messageId;

        public string Name
        {
            get
            { return _name; }
            set
            { _name = value; }
        }
        public string Subject
        {
            get
            { return _subject; }
            set
            { _subject = value; }
        }
        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }
        public int MessageId
        {
            get { return _messageId; }
            set { _messageId = value; }
        }

        public Message()
        {
            _messageId = 0;
        }

        public Message(int messageId)
        {
            _messageId = messageId;
        }
    }
}