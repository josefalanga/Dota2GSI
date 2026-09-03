using Newtonsoft.Json.Linq;
using System;
using System.Text.RegularExpressions;

namespace Dota2GSI.Nodes.DraftProvider
{
    /// <summary>
    /// A class representing draft details information.
    /// </summary>
    public class DraftDetails : Node
    {
        /// <summary>
        /// Is home team.
        /// </summary>
        public readonly bool IsHomeTeam;

        /// <summary>
        /// Pick IDs.
        /// </summary>
        public readonly NodeMap<int, int> PickIDs = new NodeMap<int, int>();

        /// <summary>
        /// Pick Hero IDs.
        /// </summary>
        public readonly NodeMap<int, string> PickHeroIDs = new NodeMap<int, string>();

        /// <summary>
        /// Ban IDs.
        /// </summary>
        public readonly NodeMap<int, int> BanIDs = new NodeMap<int, int>();

        /// <summary>
        /// Ban Hero IDs.
        /// </summary>
        public readonly NodeMap<int, string> BanHeroIDs = new NodeMap<int, string>();

        private Regex _pick_id_regex = new Regex(@"pick(\d+)_id");
        private Regex _pick_class_regex = new Regex(@"pick(\d+)_class");
        private Regex _ban_id_regex = new Regex(@"ban(\d+)_id");
        private Regex _ban_class_regex = new Regex(@"ban(\d+)_class");

        internal DraftDetails(JObject parsed_data = null) : base(parsed_data)
        {
            IsHomeTeam = GetBool("home_team");

            GetMatchingIntegers(parsed_data, _pick_id_regex, (Match match, int value) =>
            {
                var pick_index = Convert.ToInt32(match.Groups[1].Value);

                PickIDs.Add(pick_index, value);
            });

            GetMatchingStrings(parsed_data, _pick_class_regex, (Match match, string value) =>
            {
                var pick_index = Convert.ToInt32(match.Groups[1].Value);

                PickHeroIDs.Add(pick_index, value);
            });

            GetMatchingIntegers(parsed_data, _ban_id_regex, (Match match, int value) =>
            {
                var ban_index = Convert.ToInt32(match.Groups[1].Value);

                BanIDs.Add(ban_index, value);
            });

            GetMatchingStrings(parsed_data, _ban_class_regex, (Match match, string value) =>
            {
                var ban_index = Convert.ToInt32(match.Groups[1].Value);

                BanHeroIDs.Add(ban_index, value);
            });
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[" +
                $"IsHomeTeam: {IsHomeTeam}, " +
                $"PickIDs: {PickIDs}, " +
                $"PickHeroIDs: {PickHeroIDs}, " +
                $"BanIDs: {BanIDs}, " +
                $"BanHeroIDs: {BanHeroIDs}" +
                $"]";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (null == obj)
            {
                return false;
            }

            return obj is DraftDetails other &&
                IsHomeTeam.Equals(other.IsHomeTeam) &&
                PickIDs.Equals(other.PickIDs) &&
                PickHeroIDs.Equals(other.PickHeroIDs) &&
                BanIDs.Equals(other.BanIDs) &&
                BanHeroIDs.Equals(other.BanHeroIDs);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 148535770;
            hashCode = hashCode * -633163745 + IsHomeTeam.GetHashCode();
            hashCode = hashCode * -633163745 + PickIDs.GetHashCode();
            hashCode = hashCode * -633163745 + PickHeroIDs.GetHashCode();
            hashCode = hashCode * -633163745 + BanIDs.GetHashCode();
            hashCode = hashCode * -633163745 + BanHeroIDs.GetHashCode();
            return hashCode;
        }
    }
}
