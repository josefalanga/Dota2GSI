using Dota2GSI.Nodes.DraftProvider;
using Newtonsoft.Json.Linq;
using System;
using System.Text.RegularExpressions;

namespace Dota2GSI.Nodes
{
    /// <summary>
    /// A class representing draft information.
    /// </summary>
    public class Draft : Node
    {
        /// <summary>
        /// The active team.
        /// </summary>
        public readonly int ActiveTeam;

        /// <summary>
        /// Is hero picking state. Ban state if false.
        /// </summary>
        public readonly bool Pick;

        /// <summary>
        /// The active team remaining time in seconds.
        /// </summary>
        public readonly int ActiveTeamRemainingTime;

        /// <summary>
        /// The radiant team bonus time in seconds.
        /// </summary>
        public readonly int RadiantBonusTime;

        /// <summary>
        /// The dire team bonus time in seconds.
        /// </summary>
        public readonly int DireBonusTime;

        /// <summary>
        /// Radiant picks, as hero unit names/slots (e.g. "npc_dota_hero_pudge").
        /// </summary>
        public readonly NodeList<string> RadiantPicks = new NodeList<string>();

        /// <summary>
        /// Radiant bans, as hero unit names/slots.
        /// </summary>
        public readonly NodeList<string> RadiantBans = new NodeList<string>();

        /// <summary>
        /// Dire picks, as hero unit names/slots.
        /// </summary>
        public readonly NodeList<string> DirePicks = new NodeList<string>();

        /// <summary>
        /// Dire bans, as hero unit names/slots.
        /// </summary>
        public readonly NodeList<string> DireBans = new NodeList<string>();

        /// <summary>
        /// The team draft information.
        /// </summary>
        public readonly NodeMap<PlayerTeam, DraftDetails> Teams = new NodeMap<PlayerTeam, DraftDetails>();

        private Regex _team_id_regex = new Regex(@"team(\d+)");

        internal Draft(JObject parsed_data = null) : base(parsed_data)
        {
            // Real GSI emits "activeteam"/"activeteam_time_remaining"; fall back to
            // legacy misspelled keys "active_team"/"activeteam_remaining_time".
            ActiveTeam = GetInt(GetJToken("activeteam") != null ? "activeteam" : "active_team");
            Pick = GetBool("pick");
            ActiveTeamRemainingTime = GetInt(GetJToken("activeteam_time_remaining") != null ? "activeteam_time_remaining" : "activeteam_remaining_time");
            RadiantBonusTime = GetInt("radiant_bonus_time");
            DireBonusTime = GetInt("dire_bonus_time");
            RadiantPicks = GetStringArray("radiant_picks");
            RadiantBans = GetStringArray("radiant_bans");
            DirePicks = GetStringArray("dire_picks");
            DireBans = GetStringArray("dire_bans");

            GetMatchingObjects(parsed_data, _team_id_regex, (Match match, JObject obj) =>
            {
                var team_id = (PlayerTeam)Convert.ToInt32(match.Groups[1].Value);

                Teams.Add(team_id, new DraftDetails(obj));
            });
        }

        private NodeList<string> GetStringArray(string name)
        {
            var result = new NodeList<string>();
            var arr = GetArray(name);
            foreach (var token in arr)
            {
                var value = token.ToString();
                if (!string.IsNullOrEmpty(value))
                    result.Add(value);
            }
            return result;
        }

        /// <summary>
        /// Gets the draft for a specific team.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <returns>The draft details.</returns>
        public DraftDetails GetForTeam(PlayerTeam team)
        {
            if (Teams.ContainsKey(team))
            {
                return Teams[team];
            }

            return new DraftDetails();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[" +
                $"ActiveTeam: {ActiveTeam}, " +
                $"Pick: {Pick}, " +
                $"ActiveTeamRemainingTime: {ActiveTeamRemainingTime}, " +
                $"RadiantBonusTime: {RadiantBonusTime}, " +
                $"DireBonusTime: {DireBonusTime}, " +
                $"RadiantPicks: {RadiantPicks}, " +
                $"RadiantBans: {RadiantBans}, " +
                $"DirePicks: {DirePicks}, " +
                $"DireBans: {DireBans}, " +
                $"Teams: {Teams}" +
                $"]";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (null == obj)
            {
                return false;
            }

            return obj is Draft other &&
                ActiveTeam.Equals(other.ActiveTeam) &&
                Pick.Equals(other.Pick) &&
                ActiveTeamRemainingTime.Equals(other.ActiveTeamRemainingTime) &&
                RadiantBonusTime.Equals(other.RadiantBonusTime) &&
                DireBonusTime.Equals(other.DireBonusTime) &&
                RadiantPicks.Equals(other.RadiantPicks) &&
                RadiantBans.Equals(other.RadiantBans) &&
                DirePicks.Equals(other.DirePicks) &&
                DireBans.Equals(other.DireBans) &&
                Teams.Equals(other.Teams);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 370669188;
            hashCode = hashCode * -824566422 + ActiveTeam.GetHashCode();
            hashCode = hashCode * -824566422 + Pick.GetHashCode();
            hashCode = hashCode * -824566422 + ActiveTeamRemainingTime.GetHashCode();
            hashCode = hashCode * -824566422 + RadiantBonusTime.GetHashCode();
            hashCode = hashCode * -824566422 + DireBonusTime.GetHashCode();
            hashCode = hashCode * -824566422 + RadiantPicks.GetHashCode();
            hashCode = hashCode * -824566422 + RadiantBans.GetHashCode();
            hashCode = hashCode * -824566422 + DirePicks.GetHashCode();
            hashCode = hashCode * -824566422 + DireBans.GetHashCode();
            hashCode = hashCode * -824566422 + Teams.GetHashCode();
            return hashCode;
        }
    }
}
