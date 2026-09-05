using Dota2GSI.Nodes.MapProvider;
using Newtonsoft.Json.Linq;
using System;
using System.Text.RegularExpressions;

namespace Dota2GSI.Nodes
{
    /// <summary>
    /// Enum list for each Game State.
    /// </summary>
    public enum DOTA_GameState
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined,

        /// <summary>
        /// Disconnected.
        /// </summary>
        DOTA_GAMERULES_STATE_DISCONNECT,

        /// <summary>
        /// Game is in progress.
        /// </summary>
        DOTA_GAMERULES_STATE_GAME_IN_PROGRESS,

        /// <summary>
        /// Players are currently selecting heroes.
        /// </summary>
        DOTA_GAMERULES_STATE_HERO_SELECTION,

        /// <summary>
        /// Game is starting.
        /// </summary>
        DOTA_GAMERULES_STATE_INIT,

        /// <summary>
        /// Game is ending.
        /// </summary>
        DOTA_GAMERULES_STATE_LAST,

        /// <summary>
        /// Game has ended, post game scoreboard.
        /// </summary>
        DOTA_GAMERULES_STATE_POST_GAME,

        /// <summary>
        /// Game has started, pre game preparations.
        /// </summary>
        DOTA_GAMERULES_STATE_PRE_GAME,

        /// <summary>
        /// Players are selecting/banning heroes.
        /// </summary>
        DOTA_GAMERULES_STATE_STRATEGY_TIME,

        /// <summary>
        /// Waiting for everyone to connect and load.
        /// </summary>
        DOTA_GAMERULES_STATE_WAIT_FOR_PLAYERS_TO_LOAD,

        /// <summary>
        /// Game is a custom game.
        /// </summary>
        DOTA_GAMERULES_STATE_CUSTOM_GAME_SETUP,

        /// <summary>
        /// Waiting for the map to load.
        /// </summary>
        DOTA_GAMERULES_STATE_WAIT_FOR_MAP_TO_LOAD,

        /// <summary>
        /// Game is in the team showcase state.
        /// </summary>
        DOTA_GAMERULES_STATE_TEAM_SHOWCASE
    }

    /// <summary>
    /// Enum list for each player team.
    /// </summary>
    public enum PlayerTeam
    {
        /// <summary>
        /// Unknown team.
        /// </summary>
        Unknown = -2,

        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined = -1,

        /// <summary>
        /// No team.
        /// </summary>
        None = 0,

        /// <summary>
        /// Spectator.
        /// </summary>
        Spectator = 1,

        /// <summary>
        /// Radiant team.
        /// </summary>
        Radiant = 2,

        /// <summary>
        /// Dire team.
        /// </summary>
        Dire = 3,

        /// <summary>
        /// Neutral team.
        /// </summary>
        Neutrals = 4
    }

    /// <summary>
    /// Enum list for each Roshan state.
    /// </summary>
    public enum RoshanState
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined = -1,

        /// <summary>
        /// Alive.
        /// </summary>
        Alive,

        /// <summary>
        /// Waiting for respawn at base respawn rate.
        /// </summary>
        Respawn_Base,

        /// <summary>
        /// Waiting for respawn at varied respawn rate.
        /// </summary>
        Respawn_Variable
    }

    /// <summary>
    /// Class representing information about the map.
    /// </summary>
    public class Map : Node
    {
        /// <summary>
        /// Name of the current map.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Match ID of the current game.
        /// </summary>
        public readonly long MatchID;

        /// <summary>
        /// Game time.
        /// </summary>
        public readonly int GameTime;

        /// <summary>
        /// Clock time (time shown at the top of the game hud).
        /// </summary>
        public readonly int ClockTime;

        /// <summary>
        /// A boolean representing whether it is daytime.
        /// </summary>
        public readonly bool IsDaytime;

        /// <summary>
        /// A boolean representing whether Nightstalker forced night time.
        /// </summary>
        public readonly bool IsNightstalkerNight;

        /// <summary>
        /// Current Radiant team score.
        /// </summary>
        public readonly int RadiantScore;

        /// <summary>
        /// Current Dire team score.
        /// </summary>
        public readonly int DireScore;

        /// <summary>
        /// Current game state.
        /// </summary>
        public readonly DOTA_GameState GameState;

        /// <summary>
        /// A boolean representing whether the game is currently paused.
        /// </summary>
        public readonly bool IsPaused;

        /// <summary>
        /// The winning team.
        /// </summary>
        public readonly PlayerTeam WinningTeam;

        /// <summary>
        /// The name of the custom game.
        /// </summary>
        public readonly string CustomGameName;

        /// <summary>
        /// The cooldown on ward purchases.
        /// </summary>
        public readonly int WardPurchaseCooldown;

        /// <summary>
        /// The cooldown on ward purchases for the Radiant team. (SPECTATOR ONLY)
        /// </summary>
        public readonly int RadiantWardPurchaseCooldown;

        /// <summary>
        /// The cooldown on ward purchases for the Dire team. (SPECTATOR ONLY)
        /// </summary>
        public readonly int DireWardPurchaseCooldown;

        /// <summary>
        /// The Radiant team's win probability as a float (0..1). -1 when not provided.
        /// </summary>
        public readonly float RadiantWinChance;

        /// <summary>
        /// The state of Roshan. (SPECTATOR ONLY)
        /// </summary>
        public readonly RoshanState RoshanState;

/// <summary>
        /// The time in seconds until the Roshan state changes. (SPECTATOR ONLY)
        /// </summary>
        public readonly int RoshanStateEndTime;

        /// <summary>
        /// The current state of the Tormentor.
        /// </summary>
        public readonly string TormentorState;

        /// <summary>
        /// The time in seconds until the Tormentor state changes.
        /// </summary>
        public readonly int TormentorStateEndTime;

        /// <summary>
        /// The Tormentor's location ("top" or "bottom").
        /// </summary>
        public readonly string TormentorStateLocation;

        /// <summary>
        /// The Radiant team's glyph cooldown.
        /// </summary>
        public readonly int RadiantGlyphCooldown;

        /// <summary>
        /// The Dire team's glyph cooldown.
        /// </summary>
        public readonly int DireGlyphCooldown;

        /// <summary>
        /// The Radiant team's scan cooldown.
        /// </summary>
        public readonly int RadiantScanCooldown;

        /// <summary>
        /// The Radiant team's scan charges.
        /// </summary>
        public readonly int RadiantScanCharges;

        /// <summary>
        /// The Dire team's scan cooldown.
        /// </summary>
        public readonly int DireScanCooldown;

        /// <summary>
        /// The Dire team's scan charges.
        /// </summary>
        public readonly int DireScanCharges;

        /// <summary>
        /// A boolean representing whether the Radiant wisdom shrine is available.
        /// </summary>
        public readonly bool RadiantWisdomShrine;

        /// <summary>
        /// A boolean representing whether the Dire wisdom shrine is available.
        /// </summary>
        public readonly bool DireWisdomShrine;

        /// <summary>
        /// The Radiant team's lotus pool count.
        /// </summary>
        public readonly int RadiantLotusPoolCount;

        /// <summary>
        /// The Dire team's lotus pool count.
        /// </summary>
        public readonly int DireLotusPoolCount;

        /// <summary>
        /// The map watchers. The key is the watcher ID.
        /// </summary>
        public readonly NodeMap<int, Watcher> Watchers = new NodeMap<int, Watcher>();

        private Regex _watcher_id_regex = new Regex(@"watcher(\d+)");

        internal Map(JObject parsed_data = null) : base(parsed_data)
        {
            Name = GetString("name");
            MatchID = GetLong("matchid");
            GameTime = GetInt("game_time");
            ClockTime = GetInt("clock_time");
            IsDaytime = GetBool("daytime");
            IsNightstalkerNight = GetBool("nightstalker_night");
            RadiantScore = GetInt("radiant_score");
            DireScore = GetInt("dire_score");
            GameState = GetEnum<DOTA_GameState>("game_state");
            IsPaused = GetBool("paused");
            WinningTeam = GetEnum<PlayerTeam>("win_team");
            CustomGameName = GetString("customgamename");
            RadiantWardPurchaseCooldown = GetInt("radiant_ward_purchase_cooldown");
            DireWardPurchaseCooldown = GetInt("dire_ward_purchase_cooldown");
            RadiantWinChance = GetFloat("radiant_win_chance");
            // There is mentions of "radiant_win_chance" in game code, but no mention of "dire_win_chance".
            // Omitting "radiant_win_chance" since there is no dire counterpart.
            RoshanState = GetEnum<RoshanState>("roshan_state");
            RoshanStateEndTime = GetInt("roshan_state_end_seconds");
            WardPurchaseCooldown = GetInt("ward_purchase_cooldown");

            TormentorState = GetString("tormentor_state");
            TormentorStateEndTime = GetInt("tormentor_state_end_seconds");
            TormentorStateLocation = GetString("tormentor_state_location");
            RadiantGlyphCooldown = GetInt("radiant_glyph_cooldown");
            DireGlyphCooldown = GetInt("dire_glyph_cooldown");
            RadiantScanCooldown = GetInt("radiant_scan_cooldown");
            RadiantScanCharges = GetInt("radiant_scan_charges");
            DireScanCooldown = GetInt("dire_scan_cooldown");
            DireScanCharges = GetInt("dire_scan_charges");
            RadiantWisdomShrine = GetBool("radiant_wisdom_shrine");
            DireWisdomShrine = GetBool("dire_wisdom_shrine");
            RadiantLotusPoolCount = GetInt("radiant_lotus_pool_count");
            DireLotusPoolCount = GetInt("dire_lotus_pool_count");

            GetMatchingObjects(GetJObject("watchers"), _watcher_id_regex, (Match match, JObject obj) =>
            {
                var watcher_index = Convert.ToInt32(match.Groups[1].Value);
                var watcher = new Watcher(obj);

                if (!Watchers.ContainsKey(watcher_index))
                {
                    Watchers.Add(watcher_index, watcher);
                }
                else
                {
                    Watchers[watcher_index] = watcher;
                }
            });
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[" +
                $"Name: {Name}, " +
                $"MatchID: {MatchID}, " +
                $"GameTime: {GameTime}, " +
                $"ClockTime: {ClockTime}, " +
                $"IsDaytime: {IsDaytime}, " +
                $"IsNightstalkerNight: {IsNightstalkerNight}, " +
                $"RadiantScore: {RadiantScore}, " +
                $"DireScore: {DireScore}, " +
                $"RadiantWinChance: {RadiantWinChance}, " +
                $"GameState: {GameState}, " +
                $"IsPaused: {IsPaused}, " +
                $"WinningTeam: {WinningTeam}, " +
                $"CustomGameName: {CustomGameName}, " +
                $"WardPurchaseCooldown: {WardPurchaseCooldown}, " +
                $"RadiantWardPurchaseCooldown: {RadiantWardPurchaseCooldown}, " +
                $"DireWardPurchaseCooldown: {DireWardPurchaseCooldown}, " +
                $"RoshanState: {RoshanState}, " +
                $"RoshanStateEndTime: {RoshanStateEndTime}, " +
                $"TormentorState: {TormentorState}, " +
                $"TormentorStateEndTime: {TormentorStateEndTime}, " +
                $"TormentorStateLocation: {TormentorStateLocation}, " +
                $"RadiantGlyphCooldown: {RadiantGlyphCooldown}, " +
                $"DireGlyphCooldown: {DireGlyphCooldown}, " +
                $"RadiantScanCooldown: {RadiantScanCooldown}, " +
                $"RadiantScanCharges: {RadiantScanCharges}, " +
                $"DireScanCooldown: {DireScanCooldown}, " +
                $"DireScanCharges: {DireScanCharges}, " +
                $"RadiantWisdomShrine: {RadiantWisdomShrine}, " +
                $"DireWisdomShrine: {DireWisdomShrine}, " +
                $"RadiantLotusPoolCount: {RadiantLotusPoolCount}, " +
                $"DireLotusPoolCount: {DireLotusPoolCount}, " +
                $"Watchers: {Watchers}" +
                $"]";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (null == obj)
            {
                return false;
            }

            return obj is Map other &&
                Name.Equals(other.Name) &&
                MatchID.Equals(other.MatchID) &&
                GameTime.Equals(other.GameTime) &&
                ClockTime.Equals(other.ClockTime) &&
                IsDaytime.Equals(other.IsDaytime) &&
                IsNightstalkerNight.Equals(other.IsNightstalkerNight) &&
                RadiantScore.Equals(other.RadiantScore) &&
                DireScore.Equals(other.DireScore) &&
                RadiantWinChance.Equals(other.RadiantWinChance) &&
                GameState.Equals(other.GameState) &&
                IsPaused.Equals(other.IsPaused) &&
                WinningTeam.Equals(other.WinningTeam) &&
                CustomGameName.Equals(other.CustomGameName) &&
                WardPurchaseCooldown.Equals(other.WardPurchaseCooldown) &&
                RadiantWardPurchaseCooldown.Equals(other.RadiantWardPurchaseCooldown) &&
                DireWardPurchaseCooldown.Equals(other.DireWardPurchaseCooldown) &&
                RoshanState.Equals(other.RoshanState) &&
                RoshanStateEndTime.Equals(other.RoshanStateEndTime) &&
                TormentorState.Equals(other.TormentorState) &&
                TormentorStateEndTime.Equals(other.TormentorStateEndTime) &&
                TormentorStateLocation.Equals(other.TormentorStateLocation) &&
                RadiantGlyphCooldown.Equals(other.RadiantGlyphCooldown) &&
                DireGlyphCooldown.Equals(other.DireGlyphCooldown) &&
                RadiantScanCooldown.Equals(other.RadiantScanCooldown) &&
                RadiantScanCharges.Equals(other.RadiantScanCharges) &&
                DireScanCooldown.Equals(other.DireScanCooldown) &&
                DireScanCharges.Equals(other.DireScanCharges) &&
                RadiantWisdomShrine.Equals(other.RadiantWisdomShrine) &&
                DireWisdomShrine.Equals(other.DireWisdomShrine) &&
                RadiantLotusPoolCount.Equals(other.RadiantLotusPoolCount) &&
                DireLotusPoolCount.Equals(other.DireLotusPoolCount) &&
                Watchers.Equals(other.Watchers);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 225797103;
            hashCode = hashCode * -955669127 + Name.GetHashCode();
            hashCode = hashCode * -955669127 + MatchID.GetHashCode();
            hashCode = hashCode * -955669127 + GameTime.GetHashCode();
            hashCode = hashCode * -955669127 + ClockTime.GetHashCode();
            hashCode = hashCode * -955669127 + IsDaytime.GetHashCode();
            hashCode = hashCode * -955669127 + IsNightstalkerNight.GetHashCode();
            hashCode = hashCode * -955669127 + RadiantScore.GetHashCode();
            hashCode = hashCode * -955669127 + DireScore.GetHashCode();
            hashCode = hashCode * -955669127 + RadiantWinChance.GetHashCode();
            hashCode = hashCode * -955669127 + GameState.GetHashCode();
            hashCode = hashCode * -955669127 + IsPaused.GetHashCode();
            hashCode = hashCode * -955669127 + WinningTeam.GetHashCode();
            hashCode = hashCode * -955669127 + CustomGameName.GetHashCode();
            hashCode = hashCode * -955669127 + WardPurchaseCooldown.GetHashCode();
            hashCode = hashCode * -955669127 + RadiantWardPurchaseCooldown.GetHashCode();
            hashCode = hashCode * -955669127 + DireWardPurchaseCooldown.GetHashCode();
            hashCode = hashCode * -955669127 + RoshanState.GetHashCode();
            hashCode = hashCode * -955669127 + RoshanStateEndTime.GetHashCode();
            hashCode = hashCode * -955669127 + TormentorState.GetHashCode();
            hashCode = hashCode * -955669127 + TormentorStateEndTime.GetHashCode();
            hashCode = hashCode * -955669127 + TormentorStateLocation.GetHashCode();
            hashCode = hashCode * -955669127 + RadiantGlyphCooldown.GetHashCode();
            hashCode = hashCode * -955669127 + DireGlyphCooldown.GetHashCode();
            hashCode = hashCode * -955669127 + RadiantScanCooldown.GetHashCode();
            hashCode = hashCode * -955669127 + RadiantScanCharges.GetHashCode();
            hashCode = hashCode * -955669127 + DireScanCooldown.GetHashCode();
            hashCode = hashCode * -955669127 + DireScanCharges.GetHashCode();
            hashCode = hashCode * -955669127 + RadiantWisdomShrine.GetHashCode();
            hashCode = hashCode * -955669127 + DireWisdomShrine.GetHashCode();
            hashCode = hashCode * -955669127 + RadiantLotusPoolCount.GetHashCode();
            hashCode = hashCode * -955669127 + DireLotusPoolCount.GetHashCode();
            hashCode = hashCode * -955669127 + Watchers.GetHashCode();
            return hashCode;
        }
    }
}
