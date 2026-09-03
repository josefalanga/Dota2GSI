using Dota2GSI.Nodes.NeutralItemsProvider;
using Newtonsoft.Json.Linq;
using System;
using System.Text.RegularExpressions;

namespace Dota2GSI.Nodes
{
    /// <summary>
    /// Class representing neutral items.
    /// </summary>
    public class NeutralItems : Node
    {
        /// <summary>
        /// The global maximum Madstone amount.
        /// </summary>
        public readonly int MaxMadstone;

        /// <summary>
        /// Information about each neutral item tier. Key is the tier.
        /// </summary>
        public readonly NodeMap<int, NeutralTier> Tiers = new NodeMap<int, NeutralTier>();

        /// <summary>
        /// Information about each team's neutral items. Key is the team, value is a map of player slot to player Madstone state.
        /// </summary>
        public readonly NodeMap<PlayerTeam, NodeMap<int, NeutralPlayer>> Teams = new NodeMap<PlayerTeam, NodeMap<int, NeutralPlayer>>();

        private Regex _tier_id_regex = new Regex(@"tier(\d+)");
        private Regex _team_id_regex = new Regex(@"team(\d+)");
        private Regex _player_id_regex = new Regex(@"player(\d+)");

        internal NeutralItems(JObject parsed_data = null) : base(parsed_data)
        {
            MaxMadstone = GetInt("max_madstone");

            GetMatchingObjects(parsed_data, _tier_id_regex, (Match match, JObject obj) =>
            {
                var tier_index = Convert.ToInt32(match.Groups[1].Value);
                var tier = new NeutralTier(obj);

                if (!Tiers.ContainsKey(tier_index))
                {
                    Tiers.Add(tier_index, tier);
                }
                else
                {
                    Tiers[tier_index] = tier;
                }
            });

            GetMatchingObjects(parsed_data, _team_id_regex, (Match match, JObject obj) =>
            {
                var team_id = (PlayerTeam)Convert.ToInt32(match.Groups[1].Value);

                if (!Teams.ContainsKey(team_id))
                {
                    Teams.Add(team_id, new NodeMap<int, NeutralPlayer>());
                }

                GetMatchingObjects(obj, _player_id_regex, (Match sub_match, JObject sub_obj) =>
                {
                    var player_index = Convert.ToInt32(sub_match.Groups[1].Value);
                    var player = new NeutralPlayer(sub_obj);

                    if (!Teams[team_id].ContainsKey(player_index))
                    {
                        Teams[team_id].Add(player_index, player);
                    }
                    else
                    {
                        Teams[team_id][player_index] = player;
                    }
                });
            });
        }

        /// <summary>
        /// Gets the neutral items for a specific team.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <returns>The map of player slot to player Madstone state.</returns>
        public NodeMap<int, NeutralPlayer> GetForTeam(PlayerTeam team)
        {
            if (Teams.ContainsKey(team))
            {
                return Teams[team];
            }

            return new NodeMap<int, NeutralPlayer>();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[" +
                $"MaxMadstone: {MaxMadstone}, " +
                $"Tiers: {Tiers}, " +
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

            return obj is NeutralItems other &&
                MaxMadstone.Equals(other.MaxMadstone) &&
                Tiers.Equals(other.Tiers) &&
                Teams.Equals(other.Teams);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 904745338;
            hashCode = hashCode * -700564887 + MaxMadstone.GetHashCode();
            hashCode = hashCode * -700564887 + Tiers.GetHashCode();
            hashCode = hashCode * -700564887 + Teams.GetHashCode();
            return hashCode;
        }
    }
}