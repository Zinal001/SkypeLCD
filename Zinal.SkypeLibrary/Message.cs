using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Zinal.SkypeLibrary
{
    public class Message
    {
        public int id, is_permanent, convo_id, author_was_live, type, sending_status, consumption_status, param_key, param_value, leavereason, participant_count, error_code,
            chatmsg_type, chatmsg_status, body_is_rawxml, oldoptions, newoptions, newrole, pk_id, crc, remote_id, extprop_contact_reviewed;

        public String chatname, author, from_dispname, dialog_partner, edited_by, body_xml, identities, reason, call_guid, extprop_contact_review_date;

        public Object guid;

        public DateTime timestamp, edited_timestamp, extprop_contact_received_stamp;

        public long timestampLong, edited_timestampLong, extprop_contact_received_stampLong;

        public String ParsedMessage
        {
            get
            {
                if (this.body_xml != null)
                    return System.Net.WebUtility.HtmlDecode(RemoveXmlTags(this.body_xml));

                return null;
            }
        }

        private static String RemoveXmlTags(String str)
        {
            int i = -1;
            do
            {
                i = str.IndexOf("<ss");
                if (i == -1)
                    break;

                int j = str.IndexOf(">", i);

                if (j == -1)
                    break;

                String tagStr = str.Substring(i, j - i + 1);

                int k = str.IndexOf("</ss>");
                if (k == -1)
                    break;

                String wholeTag = str.Substring(i, k - i + 5);
                String text = str.Substring(j + 1, k - j - 1);
                str = str.Replace(wholeTag, text);
            } while (i != -1);

            return str;
        }
    }
}
