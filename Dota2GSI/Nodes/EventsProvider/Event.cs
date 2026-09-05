using Newtonsoft.Json.Linq;
using System;

namespace Dota2GSI.Nodes.EventsProvider
{
    /// <summary>
    /// Enum for types of events.
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined = -1,

        /// <summary>
        /// Courier was killed.
        /// </summary>
        Courier_killed,

        /// <summary>
        /// Roshan was killed.
        /// </summary>
        Roshan_killed,

        /// <summary>
        /// Aegis was picked up.
        /// </summary>
        Aegis_picked_up,

        /// <summary>
        /// Aegis was denied.
        /// </summary>
        Aegis_denied,

        /// <summary>
        /// Player was tipped.
        /// </summary>
        Tip,

/// <summary>
        /// Bounty rune was picked up.
        /// </summary>
        Bounty_rune_pickup,

        /// <summary>
        /// A chat message was sent.
        /// </summary>
        Chat_message,

        /// <summary>
        /// A generic event carrying a nested <c>data</c> payload.
        /// </summary>
        Generic_event
    }

    /// <summary>
    /// Known real event types carried inside a <see cref="EventType.Generic_event"/>
    /// <c>data</c> payload. Dota emits these as <c>CHAT_MESSAGE_*</c> strings; the
    /// numeric fields on <see cref="EventData"/> mean different things per type.
    /// </summary>
    public enum GenericEventType
    {
        /// <summary>Not one of the known generic event types.</summary>
        Unknown = -1,

        /// <summary>A hero was banned during draft. <see cref="EventData.Value"/> is the banned hero id.</summary>
        Hero_banned,

        /// <summary>Two players collided on a blind pick and the hero was unpicked and banned. <see cref="EventData.Value"/> is the hero id; <see cref="EventData.PlayerID1"/> and <see cref="EventData.PlayerID2"/> are the colliding players.</summary>
        Hero_choice_invalid,

        /// <summary>A hero was killed. <see cref="EventData.PlayerID1"/> is the killer, <see cref="EventData.PlayerID2"/> the victim.</summary>
        Hero_kill,

        /// <summary>A kill streak message was broadcast.</summary>
        Streak_kill,

        /// <summary>A player bought an item. <see cref="EventData.Value"/> is the item id, <see cref="EventData.PlayerID1"/> the buyer.</summary>
        Item_purchase,

        /// <summary>A tower fell. <see cref="EventData.Value"/> is the team that lost it.</summary>
        Tower_kill,

        /// <summary>A barracks fell.</summary>
        Barracks_kill,

        /// <summary>A team used glyph of fortification. <see cref="EventData.PlayerID1"/> is the team.</summary>
        Glyph_used,

        /// <summary>A team used Scan. <see cref="EventData.Value"/> is the team.</summary>
        Scan_used,

        /// <summary>A courier was killed.</summary>
        Courier_lost,

        /// <summary>A courier respawned. <see cref="EventData.PlayerID1"/> is the owning team.</summary>
        Courier_respawned,

        /// <summary>A sentry ward was destroyed. <see cref="EventData.PlayerID1"/> is the killer.</summary>
        Sentry_ward_killed,

        /// <summary>A player bought back. <see cref="EventData.PlayerID1"/> is the player.</summary>
        Buyback,

        /// <summary>An item was placed in the neutral-item bag. <see cref="EventData.PlayerID1"/> is the player.</summary>
        Inthebag,

        /// <summary>An observer ward was destroyed. <see cref="EventData.PlayerID1"/> is the killer.</summary>
        Observer_ward_killed,

        /// <summary>First blood was drawn. <see cref="EventData.PlayerID1"/> is the killer, <see cref="EventData.PlayerID2"/> the victim.</summary>
        Firstblood,

        /// <summary>Super creeps were activated. <see cref="EventData.PlayerID1"/> is the team.</summary>
        Super_creeps,

        /// <summary>New-player reminder surfaced for a player.</summary>
        New_player_reminder
    }

    /// <summary>
    /// Class representing an event.
    /// </summary>
    public class Event : Node
    {
        /// <summary>
        /// The game time when this event took place.
        /// </summary>
        public readonly int GameTime;

        /// <summary>
        /// The type of event.
        /// </summary>
        public readonly EventType EventType;

        /// <summary>
        /// The team invovled in the event.
        /// </summary>
        public readonly PlayerTeam Team;

        /// <summary>
        /// The ID of the killer player invovled in the event.
        /// </summary>
        public readonly int KillerPlayerID;

        /// <summary>
        /// The ID of the player invovled in the event.
        /// </summary>
        public readonly int PlayerID;

        /// <summary>
        /// Was the aegis snatched from the other team in the event.
        /// </summary>
        public readonly bool WasSnatched;

        /// <summary>
        /// The ID of the player that received a tip in the event.
        /// </summary>
        public readonly int TipReceiverPlayerID;

        /// <summary>
        /// The amount that was tipped in the event.
        /// </summary>
        public readonly int TipAmount;

/// <summary>
        /// The amount that was picked up from a bounty rune in the event.
        /// </summary>
        public readonly int BountyValue;

        /// <summary>
        /// The ID of the player that owned the courier killed in the event. (courier_killed)
        /// </summary>
        public readonly int OwningPlayerID;

        /// <summary>
        /// The channel type of a chat message event. (chat_message)
        /// </summary>
        public readonly int ChannelType;

        /// <summary>
        /// The message text of a chat message event. (chat_message)
        /// </summary>
        public readonly string Message;

/// <summary>
        /// The amount of team gold after the event.
        /// </summary>
        public readonly int TeamGold;

        /// <summary>
        /// The nested event payload. Dota serializes most events as a
        /// <c>generic_event</c> with the real type and ids inside the
        /// <c>data</c> string, so the typed fields above are only set for the
        /// handful of legacy event types. <see cref="EventData"/> exposes the
        /// nested payload for everything else.
        /// </summary>
        public readonly EventData Data;

        internal Event(JObject parsed_data = null) : base(parsed_data)
        {
            GameTime = GetInt("game_time");
            EventType = GetEnum<EventType>("event_type");

            switch (EventType)
            {
                case EventType.Courier_killed:
                    Team = GetEnum<PlayerTeam>("courier_team");
                    KillerPlayerID = GetInt("killer_player_id");
                    OwningPlayerID = GetInt("owning_player_id");
                    break;
                case EventType.Roshan_killed:
                    Team = GetEnum<PlayerTeam>("killed_by_team");
                    KillerPlayerID = GetInt("killer_player_id");
                    break;
                case EventType.Aegis_picked_up:
                    PlayerID = GetInt("player_id");
                    WasSnatched = GetBool("snatched");
                    break;
                case EventType.Aegis_denied:
                    PlayerID = GetInt("player_id");
                    break;
                case EventType.Tip:
                    PlayerID = GetInt("sender_player_id");
                    TipReceiverPlayerID = GetInt("receiver_player_id");
                    TipAmount = GetInt("tip_amount");
                    break;
                case EventType.Bounty_rune_pickup:
                    PlayerID = GetInt("player_id");
                    Team = GetEnum<PlayerTeam>("team");
                    BountyValue = GetInt("bounty_value");
                    TeamGold = GetInt("team_gold");
                    break;
                case EventType.Chat_message:
                    PlayerID = GetInt("player_id");
                    ChannelType = GetInt("channel_type");
                    Message = GetString("message");
                    break;
default:
                    break;
            }

            Data = new EventData(GetString("data"));
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[" +
                $"GameTime: {GameTime}, " +
                $"EventType: {EventType}, " +
                $"Team: {Team}, " +
                $"KillerPlayerID: {KillerPlayerID}, " +
                $"PlayerID: {PlayerID}, " +
                $"WasSnatched: {WasSnatched}, " +
                $"TipReceiverPlayerID: {TipReceiverPlayerID}, " +
                $"TipAmount: {TipAmount}, " +
                $"BountyValue: {BountyValue}, " +
                $"TeamGold: {TeamGold}, " +
                $"OwningPlayerID: {OwningPlayerID}, " +
                $"ChannelType: {ChannelType}, " +
                $"Message: {Message}, " +
                $"Data: {Data}" +
                $"]";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (null == obj)
            {
                return false;
            }

            return obj is Event other &&
                GameTime.Equals(other.GameTime) &&
                EventType.Equals(other.EventType) &&
                Team.Equals(other.Team) &&
                KillerPlayerID.Equals(other.KillerPlayerID) &&
                PlayerID.Equals(other.PlayerID) &&
                WasSnatched.Equals(other.WasSnatched) &&
                TipReceiverPlayerID.Equals(other.TipReceiverPlayerID) &&
                TipAmount.Equals(other.TipAmount) &&
                BountyValue.Equals(other.BountyValue) &&
                TeamGold.Equals(other.TeamGold) &&
                OwningPlayerID.Equals(other.OwningPlayerID) &&
                ChannelType.Equals(other.ChannelType) &&
                Message.Equals(other.Message) &&
                Data.Equals(other.Data);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 973835034;
            hashCode = hashCode * -320607063 + GameTime.GetHashCode();
            hashCode = hashCode * -320607063 + EventType.GetHashCode();
            hashCode = hashCode * -320607063 + Team.GetHashCode();
            hashCode = hashCode * -320607063 + KillerPlayerID.GetHashCode();
            hashCode = hashCode * -320607063 + PlayerID.GetHashCode();
            hashCode = hashCode * -320607063 + WasSnatched.GetHashCode();
            hashCode = hashCode * -320607063 + TipReceiverPlayerID.GetHashCode();
            hashCode = hashCode * -320607063 + TipAmount.GetHashCode();
hashCode = hashCode * -320607063 + BountyValue.GetHashCode();
            hashCode = hashCode * -320607063 + TeamGold.GetHashCode();
            hashCode = hashCode * -320607063 + OwningPlayerID.GetHashCode();
            hashCode = hashCode * -320607063 + ChannelType.GetHashCode();
            hashCode = hashCode * -320607063 + Message.GetHashCode();
            hashCode = hashCode * -320607063 + Data.GetHashCode();
            return hashCode;
        }
    }

    /// <summary>
    /// The nested event payload carried in the <c>data</c> string for the
    /// generic event type. Dota puts the real event type and the involved
    /// player ids here rather than as top-level fields.
    /// </summary>
    public class EventData
    {
        /// <summary>The raw event type string as Dota sent it (e.g. "CHAT_MESSAGE_HERO_KILL").</summary>
        public readonly string Type;
        /// <summary><see cref="Type"/> parsed into the known generic event types; <see cref="GenericEventType.Unknown"/> for anything else.</summary>
        public readonly GenericEventType GenericType;
        /// <summary>First involved player id (attacker for kills).</summary>
        public readonly int PlayerID1;
        /// <summary>Second involved player id (victim for kills).</summary>
        public readonly int PlayerID2;
        /// <summary>Event-specific primary value.</summary>
        public readonly int Value;
        /// <summary>Event-specific secondary value.</summary>
        public readonly int Value2;
        /// <summary>Third involved player id.</summary>
        public readonly int PlayerID3;
        /// <summary>Fourth involved player id.</summary>
        public readonly int PlayerID4;
        /// <summary>Fifth involved player id.</summary>
        public readonly int PlayerID5;
        /// <summary>Sixth involved player id.</summary>
        public readonly int PlayerID6;
        /// <summary>Event-specific tertiary value.</summary>
        public readonly int Value3;
        /// <summary>Event-specific time.</summary>
        public readonly double Time;
        /// <summary>Hero id for <see cref="GenericEventType.Hero_banned"/> / <see cref="GenericEventType.Hero_choice_invalid"/>; -1 otherwise.</summary>
        public readonly int HeroId;
        /// <summary>Item id for <see cref="GenericEventType.Item_purchase"/>; 0 otherwise.</summary>
        public readonly int ItemId;
        /// <summary>Killer's player id for kill events (<see cref="GenericEventType.Hero_kill"/> / <see cref="GenericEventType.Firstblood"/>); -1 otherwise.</summary>
        public readonly int KillerPlayerId;
        /// <summary>Victim's player id for kill events; -1 otherwise.</summary>
        public readonly int VictimPlayerId;
        /// <summary>Involved team for tower/barracks/scan/glyph/super-creep/courier events; <see cref="PlayerTeam.Undefined"/> otherwise.</summary>
        public readonly PlayerTeam Team;

        internal EventData(string json)
        {
            Type = string.Empty;
            GenericType = GenericEventType.Unknown;
            PlayerID1 = -1;
            PlayerID2 = -1;
            PlayerID3 = -1;
            PlayerID4 = -1;
            PlayerID5 = -1;
            PlayerID6 = -1;
            Value = 0;
            Value2 = 0;
            Value3 = 0;
            Time = 0.0;
            HeroId = -1;
            ItemId = 0;
            KillerPlayerId = -1;
            VictimPlayerId = -1;
            Team = PlayerTeam.Undefined;

            if (string.IsNullOrEmpty(json))
                return;

            try
            {
                var obj = JObject.Parse(json);
                Type = obj["type"]?.ToString() ?? string.Empty;
                GenericType = ParseGenericEventType(Type);
                PlayerID1 = ReadInt(obj, "playerid1", -1);
                PlayerID2 = ReadInt(obj, "playerid2", -1);
                PlayerID3 = ReadInt(obj, "playerid3", -1);
                PlayerID4 = ReadInt(obj, "playerid4", -1);
                PlayerID5 = ReadInt(obj, "playerid5", -1);
                PlayerID6 = ReadInt(obj, "playerid6", -1);
                Value = ReadInt(obj, "value", 0);
                Value2 = ReadInt(obj, "value2", 0);
                Value3 = ReadInt(obj, "value3", 0);
                Time = ReadDouble(obj, "time", 0.0);
            }
            catch
            {
                // Malformed/empty data payload — leave defaults.
            }

            switch (GenericType)
            {
                case GenericEventType.Hero_banned:
                case GenericEventType.Hero_choice_invalid:
                    HeroId = Value;
                    break;
                case GenericEventType.Item_purchase:
                    ItemId = Value;
                    break;
                case GenericEventType.Hero_kill:
                case GenericEventType.Firstblood:
                    KillerPlayerId = PlayerID1;
                    VictimPlayerId = PlayerID2;
                    break;
                case GenericEventType.Tower_kill:
                case GenericEventType.Barracks_kill:
                case GenericEventType.Scan_used:
                    Team = ToTeam(Value);
                    break;
                case GenericEventType.Glyph_used:
                case GenericEventType.Super_creeps:
                case GenericEventType.Courier_respawned:
                    Team = ToTeam(PlayerID1);
                    break;
            }
        }

        private static PlayerTeam ToTeam(int value)
        {
            return value == (int)PlayerTeam.Radiant || value == (int)PlayerTeam.Dire
                ? (PlayerTeam)value
                : PlayerTeam.Undefined;
        }

        private static int ReadInt(JObject obj, string name, int fallback)
        {
            var token = obj[name];
            if (token == null)
                return fallback;
            return int.TryParse(token.ToString(), out var i) ? i : fallback;
        }

        private static GenericEventType ParseGenericEventType(string type)
        {
            if (string.IsNullOrEmpty(type) || !type.StartsWith("CHAT_MESSAGE_", StringComparison.Ordinal))
                return GenericEventType.Unknown;

            var name = type.Substring("CHAT_MESSAGE_".Length);
            return Enum.TryParse(name, true, out GenericEventType parsed) ? parsed : GenericEventType.Unknown;
        }

        private static double ReadDouble(JObject obj, string name, double fallback)
        {
            var token = obj[name];
            if (token == null)
                return fallback;
            return double.TryParse(token.ToString(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : fallback;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[" +
                $"Type: {Type}, " +
                $"GenericType: {GenericType}, " +
                $"PlayerID1: {PlayerID1}, " +
                $"PlayerID2: {PlayerID2}, " +
                $"PlayerID3: {PlayerID3}, " +
                $"PlayerID4: {PlayerID4}, " +
                $"PlayerID5: {PlayerID5}, " +
                $"PlayerID6: {PlayerID6}, " +
                $"Value: {Value}, " +
                $"Value2: {Value2}, " +
                $"Value3: {Value3}, " +
                $"Time: {Time}, " +
                $"HeroId: {HeroId}, " +
                $"ItemId: {ItemId}, " +
                $"KillerPlayerId: {KillerPlayerId}, " +
                $"VictimPlayerId: {VictimPlayerId}, " +
                $"Team: {Team}" +
                $"]";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (null == obj)
            {
                return false;
            }

            return obj is EventData other &&
                Type.Equals(other.Type) &&
                PlayerID1.Equals(other.PlayerID1) &&
                PlayerID2.Equals(other.PlayerID2) &&
                PlayerID3.Equals(other.PlayerID3) &&
                PlayerID4.Equals(other.PlayerID4) &&
                PlayerID5.Equals(other.PlayerID5) &&
                PlayerID6.Equals(other.PlayerID6) &&
                Value.Equals(other.Value) &&
                Value2.Equals(other.Value2) &&
                Value3.Equals(other.Value3) &&
                Time.Equals(other.Time);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 1924922583;
            hashCode = hashCode * -453949297 + Type.GetHashCode();
            hashCode = hashCode * -453949297 + PlayerID1.GetHashCode();
            hashCode = hashCode * -453949297 + PlayerID2.GetHashCode();
            hashCode = hashCode * -453949297 + PlayerID3.GetHashCode();
            hashCode = hashCode * -453949297 + PlayerID4.GetHashCode();
            hashCode = hashCode * -453949297 + PlayerID5.GetHashCode();
            hashCode = hashCode * -453949297 + PlayerID6.GetHashCode();
            hashCode = hashCode * -453949297 + Value.GetHashCode();
            hashCode = hashCode * -453949297 + Value2.GetHashCode();
            hashCode = hashCode * -453949297 + Value3.GetHashCode();
            hashCode = hashCode * -453949297 + Time.GetHashCode();
            return hashCode;
        }
    }
}
