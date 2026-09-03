using Newtonsoft.Json.Linq;

namespace Dota2GSI.Nodes.MapProvider
{
    /// <summary>
    /// Class representing a map watcher (tormentor/twin gate watcher).
    /// </summary>
    public class Watcher : Node
    {
        /// <summary>
        /// The watcher's X location.
        /// </summary>
        public readonly int LocationX;

        /// <summary>
        /// The watcher's Y location.
        /// </summary>
        public readonly int LocationY;

        /// <summary>
        /// The watcher's capture state.
        /// </summary>
        public readonly string CaptureState;

        internal Watcher(JObject parsed_data = null) : base(parsed_data)
        {
            LocationX = GetInt("location_x");
            LocationY = GetInt("location_y");
            CaptureState = GetString("capture_state");
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[" +
                $"LocationX: {LocationX}, " +
                $"LocationY: {LocationY}, " +
                $"CaptureState: {CaptureState}" +
                $"]";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (null == obj)
            {
                return false;
            }

            return obj is Watcher other &&
                LocationX.Equals(other.LocationX) &&
                LocationY.Equals(other.LocationY) &&
                CaptureState.Equals(other.CaptureState);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 438211705;
            hashCode = hashCode * -280848075 + LocationX.GetHashCode();
            hashCode = hashCode * -280848075 + LocationY.GetHashCode();
            hashCode = hashCode * -280848075 + CaptureState.GetHashCode();
            return hashCode;
        }
    }
}