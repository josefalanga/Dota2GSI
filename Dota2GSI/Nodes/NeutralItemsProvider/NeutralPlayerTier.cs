using Newtonsoft.Json.Linq;

namespace Dota2GSI.Nodes.NeutralItemsProvider
{
    /// <summary>
    /// Class representing a player's Madstone state for a single neutral item tier.
    /// </summary>
    public class NeutralPlayerTier : Node
    {
        /// <summary>
        /// The neutral item's tier.
        /// </summary>
        public readonly int Tier;

        /// <summary>
        /// The number of times this tier has been crafted by the player.
        /// </summary>
        public readonly int TimesCrafted;

        /// <summary>
        /// The trinket choices for this tier. Key is the choice name, value is the amount.
        /// </summary>
        public readonly NodeMap<string, int> TrinketChoices = new NodeMap<string, int>();

        /// <summary>
        /// The enchantment choices for this tier. Key is the choice name, value is the amount.
        /// </summary>
        public readonly NodeMap<string, int> EnchantmentChoices = new NodeMap<string, int>();

        internal NeutralPlayerTier(JObject parsed_data = null) : base(parsed_data)
        {
            Tier = GetInt("tier");
            TimesCrafted = GetInt("times_crafted");

            foreach (var kvp in ReadIntMap("trinket_choices"))
            {
                TrinketChoices.Add(kvp.Key, kvp.Value);
            }

            foreach (var kvp in ReadIntMap("enchantment_choices"))
            {
                EnchantmentChoices.Add(kvp.Key, kvp.Value);
            }
        }

        private NodeMap<string, int> ReadIntMap(string name)
        {
            var result = new NodeMap<string, int>();
            var obj = GetJObject(name);

            if (obj != null)
            {
                foreach (var property in obj.Properties())
                {
                    int value;
                    if (int.TryParse(property.Value.ToString(), out value))
                    {
                        result.Add(property.Name, value);
                    }
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[" +
                $"Tier: {Tier}, " +
                $"TimesCrafted: {TimesCrafted}, " +
                $"TrinketChoices: {TrinketChoices}, " +
                $"EnchantmentChoices: {EnchantmentChoices}" +
                $"]";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (null == obj)
            {
                return false;
            }

            return obj is NeutralPlayerTier other &&
                Tier.Equals(other.Tier) &&
                TimesCrafted.Equals(other.TimesCrafted) &&
                TrinketChoices.Equals(other.TrinketChoices) &&
                EnchantmentChoices.Equals(other.EnchantmentChoices);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 877393484;
            hashCode = hashCode * -307313358 + Tier.GetHashCode();
            hashCode = hashCode * -307313358 + TimesCrafted.GetHashCode();
            hashCode = hashCode * -307313358 + TrinketChoices.GetHashCode();
            hashCode = hashCode * -307313358 + EnchantmentChoices.GetHashCode();
            return hashCode;
        }
    }
}