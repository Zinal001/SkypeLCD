using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zinal.SkypeLibrary
{
    public class Conversation
    {
        public int id, is_permanent, type, live_is_muted, is_bookmarked, is_blocked, local_livestatus, inbox_message_id, last_message_id, unconsumed_suppressed_messages,
            unconsumed_normal_messages, unconsumed_elevated_messages, unconsumed_messages_voice, active_vm_id, context_horizon, consumption_horizon,
            active_invoice_message, spawned_from_convo_id, pinned_order, my_status, opt_joining_enabled, opt_entry_level_rank, opt_disclose_history, opt_history_limit_in_days,
            opt_admin_only_activities, is_p2p_migrated, premium_video_status, premium_video_is_grace_period, chat_dbid, history_horizon, consumption_horizon_set_at,
            in_migrated_thread_since, extprop_profile_height, extprop_chat_width, extprop_chat_left_margin, extprop_chat_right_margin, extprop_entry_height,
            extprop_windowpos_x, extprop_windowpos_y, extprop_windowpos_w, extprop_windowpos_h, extprop_window_maximized, extprop_window_detached, extprop_pinned_order,
            extprop_new_in_inbox, extprop_tab_order, extprop_video_layout, extprop_video_chat_height, extprop_chat_avatar, extprop_form_visible, extprop_recovery_mode,
            live_call_technology;

        public String identity, live_host, alert_string, given_displayname, displayname, creator, opt_access_token, passwordhint, meta_name, meta_topic, meta_guidelines, picture,
            guid, dialog_partner, meta_description, premium_video_sponsor_list, mcr_caller, history_sync_state, thread_version, alt_identity, awareness_liveState;

        public Object meta_picture;

        public DateTime live_start_timestamp, inbox_timestamp, last_activity_timestamp, creation_timestamp, extprop_consumption_timestamp;

    }
}
