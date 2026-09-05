using Dota2GSI.Nodes.FullHeroKillsProvider;
using Newtonsoft.Json.Linq;

namespace Dota2GSI.Nodes
{
    /// <summary>
    /// Class representing the experimental "full_hero_kills" array.<br/>
    /// Each entry is a <see cref="FullHeroKill"/> parsed tolerantly.<br/>
    /// Null when the block is absent from the payload.
    /// </summary>
    public class FullHeroKills : Node
    {
        /// <summary>
        /// The list of kill records (preserves payload order).
        /// </summary>
        public readonly NodeList<FullHeroKill> Kills = new NodeList<FullHeroKill>();

        /// <summary>
        /// Number of kill records.
        /// </summary>
        public int Count { get { return Kills.Count; } }

        internal FullHeroKills(JArray parsed_data = null) : base(null)
        {
            if (parsed_data == null)
            {
                return;
            }

            if (parsed_data.Type != JTokenType.Array)
            {
                return;
            }

            foreach (JToken element in parsed_data.Children())
            {
                if (element.Type == JTokenType.Object)
                {
                    Kills.Add(new FullHeroKill(element as JObject));
                }
            }
        }

        /// <summary>
        /// Gets the kill record at a specified index. Returns an empty record if out of range.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The kill record.</returns>
        public FullHeroKill this[int index]
        {
            get
            {
                if (index < 0 || index >= Kills.Count)
                {
                    return new FullHeroKill();
                }

                return Kills[index];
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[" +
                $"Kills: {Kills}" +
                $"]";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (null == obj)
            {
                return false;
            }

            return obj is FullHeroKills other &&
                Kills.Equals(other.Kills);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 712044531;
            hashCode = hashCode * -1521134295 + Kills.GetHashCode();
            return hashCode;
        }
    }
}
