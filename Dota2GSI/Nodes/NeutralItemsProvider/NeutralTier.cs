using Newtonsoft.Json.Linq;

namespace Dota2GSI.Nodes.NeutralItemsProvider
{
    /// <summary>
    /// Class representing a neutral item tier's global Madstone configuration.
    /// </summary>
    public class NeutralTier : Node
    {
        /// <summary>
        /// The neutral item's tier.
        /// </summary>
        public readonly int Tier;

        /// <summary>
        /// The time after which this tier can drop.
        /// </summary>
        public readonly int DropAfterTime;

        /// <summary>
        /// The Madstone cost required for this tier.
        /// </summary>
        public readonly int MadstoneRequired;

        /// <summary>
        /// The escalating recraft cost for this tier.
        /// </summary>
        public readonly int EscalatingRecraftCost;

        internal NeutralTier(JObject parsed_data = null) : base(parsed_data)
        {
            Tier = GetInt("tier");
            DropAfterTime = GetInt("drop_after_time");
            MadstoneRequired = GetInt("madstone_required");
            EscalatingRecraftCost = GetInt("escalating_recraft_cost");
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[" +
                $"Tier: {Tier}, " +
                $"DropAfterTime: {DropAfterTime}, " +
                $"MadstoneRequired: {MadstoneRequired}, " +
                $"EscalatingRecraftCost: {EscalatingRecraftCost}" +
                $"]";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (null == obj)
            {
                return false;
            }

            return obj is NeutralTier other &&
                Tier.Equals(other.Tier) &&
                DropAfterTime.Equals(other.DropAfterTime) &&
                MadstoneRequired.Equals(other.MadstoneRequired) &&
                EscalatingRecraftCost.Equals(other.EscalatingRecraftCost);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 918324818;
            hashCode = hashCode * -494083635 + Tier.GetHashCode();
            hashCode = hashCode * -494083635 + DropAfterTime.GetHashCode();
            hashCode = hashCode * -494083635 + MadstoneRequired.GetHashCode();
            hashCode = hashCode * -494083635 + EscalatingRecraftCost.GetHashCode();
            return hashCode;
        }
    }
}