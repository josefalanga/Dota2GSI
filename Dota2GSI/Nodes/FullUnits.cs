using Dota2GSI.Nodes.FullUnitsProvider;
using Newtonsoft.Json.Linq;
using System;

namespace Dota2GSI.Nodes
{
    /// <summary>
    /// Class representing the experimental "full_units" block.<br/>
    /// Holds a map of unit id (string key from the wire, e.g. entity index) to its <see cref="FullUnit"/> entry.<br/>
    /// Empty when the block is absent from the payload.
    /// </summary>
    public class FullUnits : Node
    {
        /// <summary>
        /// Map of unit id (string key) to unit entry.
        /// </summary>
        public readonly NodeMap<string, FullUnit> Units = new NodeMap<string, FullUnit>();

        internal FullUnits(JObject parsed_data = null) : base(parsed_data)
        {
            if (parsed_data == null)
            {
                return;
            }

            foreach (var property in parsed_data.Properties())
            {
                if (property.Value.Type == JTokenType.Object)
                {
                    Units[property.Name] = new FullUnit(property.Value as JObject);
                }
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[" +
                $"Units: {Units}" +
                $"]";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (null == obj)
            {
                return false;
            }

            return obj is FullUnits other &&
                Units.Equals(other.Units);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 401932118;
            hashCode = hashCode * -1521134295 + Units.GetHashCode();
            return hashCode;
        }
    }
}
