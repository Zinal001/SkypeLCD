using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zinal.SkypeLibrary
{
    public class Chat
    {
        public int id, is_permanent, options, type, mystatus, myrole, is_bookmarked, unconsumed_suppressed_msg, unconsumed_normal_msg,
            unconsumed_elevated_msg, unconsumed_msg_voice, lifesigns, first_unread_message, pk_type, conv_dbid;

        public String name, friendlyname, description, dialog_partner, adder, posters, participants, applicants, banned_users, name_text, topic, topic_xml, guidelines, alertstring,
            passwordhint, activemembers, dbpath, split_friendlyname;

        public Object picture, state_data;

        public DateTime timestamp, activity_timestamp, last_change;

    }
}
