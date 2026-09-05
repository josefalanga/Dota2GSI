using Newtonsoft.Json.Linq;
using System;
using System.Text.RegularExpressions;

namespace Dota2GSI.Nodes.NeutralItemsProvider
{
    /// <summary>
    /// Class representing a player's Madstone state.
    /// </summary>
    public class NeutralPlayer : Node
    {
        /// <summary>
        /// The player's current Madstone amount.
        /// </summary>
        public readonly int CurrentMadstone;

        /// <summary>
        /// The player's total Madstone collected.
        /// </summary>
        public readonly int TotalMadstone;

        /// <summary>
        /// The player's current crafting tier.
        /// </summary>
        public readonly int CraftingTier;

        /// <summary>
        /// The player's per-tier Madstone state. Key is the tier.
        /// </summary>
        public readonly NodeMap<int, NeutralPlayerTier> Tiers = new NodeMap<int, NeutralPlayerTier>();

        private Regex _tier_id_regex = new Regex(@"tier(\d+)");

        internal NeutralPlayer(JObject parsed_data = null) : base(parsed_data)
        {
            CurrentMadstone = GetInt("current_madstone");
            TotalMadstone = GetInt("total_madstone");
            CraftingTier = GetInt("crafting_tier");

            GetMatchingObjects(parsed_data, _tier_id_regex, (Match match, JObject obj) =>
            {
                var tier_index = Convert.ToInt32(match.Groups[1].Value);
                var tier = new NeutralPlayerTier(obj);

                if (!Tiers.ContainsKey(tier_index))
                {
                    Tiers.Add(tier_index, tier);
                }
                else
                {
                    Tiers[tier_index] = tier;
                }
            });
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[" +
                $"CurrentMadstone: {CurrentMadstone}, " +
                $"TotalMadstone: {TotalMadstone}, " +
                $"CraftingTier: {CraftingTier}, " +
                $"Tiers: {Tiers}" +
                $"]";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (null == obj)
            {
                return false;
            }

            return obj is NeutralPlayer other &&
                CurrentMadstone.Equals(other.CurrentMadstone) &&
                TotalMadstone.Equals(other.TotalMadstone) &&
                CraftingTier.Equals(other.CraftingTier) &&
                Tiers.Equals(other.Tiers);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 1751276873;
            hashCode = hashCode * -1025569008 + CurrentMadstone.GetHashCode();
            hashCode = hashCode * -1025569008 + TotalMadstone.GetHashCode();
            hashCode = hashCode * -1025569008 + CraftingTier.GetHashCode();
            hashCode = hashCode * -1025569008 + Tiers.GetHashCode();
            return hashCode;
        }
    }
}