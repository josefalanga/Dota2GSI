using Newtonsoft.Json.Linq;
using System;

namespace Dota2GSI.Nodes.FullHeroKillsProvider
{
    /// <summary>
    /// Class representing a single entry from the experimental "full_hero_kills" array.<br/>
    /// The wire shape is not fully verified, so fields are parsed tolerantly:
    /// missing or mis-typed values fall back to safe defaults and never throw.
    /// </summary>
    public class FullHeroKill : Node
    {
        /// <summary>
        /// Raw token for the kill record. Always set (empty JObject if absent).
        /// </summary>
        public readonly JToken Raw;

        /// <summary>
        /// Victim identifier (player id, name, or unit string — exact wire semantics unverified).
        /// Empty when missing.
        /// </summary>
        public readonly string Victim;

        /// <summary>
        /// Killer identifier (player id, name, or unit string — exact wire semantics unverified).
        /// Empty when missing.
        /// </summary>
        public readonly string Killer;

        /// <summary>
        /// Victim team as raw string. Empty when missing.
        /// </summary>
        public readonly string VictimTeam;

        /// <summary>
        /// Killer team as raw string. Empty when missing.
        /// </summary>
        public readonly string KillerTeam;

        internal FullHeroKill(JObject parsed_data = null) : base(parsed_data)
        {
            Raw = parsed_data ?? new JObject();

            Victim = GetStringSafe("victim");
            Killer = GetStringSafe("killer");
            VictimTeam = GetStringSafe("victim_team");
            KillerTeam = GetStringSafe("killer_team");
        }

        private string GetStringSafe(params string[] keys)
        {
            if (_ParsedData == null || keys == null)
            {
                return string.Empty;
            }

            foreach (var key in keys)
            {
                try
                {
                    var token = GetJToken(key);
                    if (token != null && token.Type != JTokenType.Null)
                    {
                        return token.ToString();
                    }
                }
                catch
                {
                }
            }

            return string.Empty;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[" +
                $"Victim: {Victim}, " +
                $"Killer: {Killer}, " +
                $"VictimTeam: {VictimTeam}, " +
                $"KillerTeam: {KillerTeam}" +
                $"]";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (null == obj)
            {
                return false;
            }

            return obj is FullHeroKill other &&
                Victim.Equals(other.Victim) &&
                Killer.Equals(other.Killer) &&
                VictimTeam.Equals(other.VictimTeam) &&
                KillerTeam.Equals(other.KillerTeam);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 190837421;
            hashCode = hashCode * -1521134295 + (Victim?.GetHashCode() ?? 0);
            hashCode = hashCode * -1521134295 + (Killer?.GetHashCode() ?? 0);
            hashCode = hashCode * -1521134295 + (VictimTeam?.GetHashCode() ?? 0);
            hashCode = hashCode * -1521134295 + (KillerTeam?.GetHashCode() ?? 0);
            return hashCode;
        }
    }
}
