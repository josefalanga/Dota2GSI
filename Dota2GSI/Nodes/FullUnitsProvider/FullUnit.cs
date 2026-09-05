using Newtonsoft.Json.Linq;
using System;

namespace Dota2GSI.Nodes.FullUnitsProvider
{
    /// <summary>
    /// Class representing a single unit entry from the experimental "full_units" block.<br/>
    /// All fields are parsed tolerantly: missing or mis-typed values fall back to safe defaults
    /// and never throw, since the exact wire shape is not fully verified.
    /// </summary>
    public class FullUnit : Node
    {
        /// <summary>
        /// Raw token for the unit's payload. Always set (may be an empty JObject if absent).
        /// </summary>
        public readonly JToken Raw;

        /// <summary>
        /// The unit's type / unitname (string). Empty when missing.
        /// </summary>
        public readonly string Type;

        /// <summary>
        /// Unit's team as raw string ("radiant"/"dire"/"neutral"/"spectator"/...). Empty when missing.
        /// Use <see cref="TeamID"/> for numeric comparison; both are tolerated from the wire.
        /// </summary>
        public readonly string Team;

        /// <summary>
        /// Unit's team as numeric id. -1 when missing or not parseable.
        /// </summary>
        public readonly int TeamID;

        /// <summary>
        /// Unit position X. -1 when missing or not parseable.
        /// </summary>
        public readonly float X;

        /// <summary>
        /// Unit position Y. -1 when missing or not parseable.
        /// </summary>
        public readonly float Y;

        /// <summary>
        /// Unit position as a 2-element float array (X, Y). Null when missing.
        /// </summary>
        public readonly float[] Location;

        /// <summary>
        /// Current health. -1 when missing or not parseable.
        /// </summary>
        public readonly float Hp;

        /// <summary>
        /// Maximum health. -1 when missing or not parseable.
        /// </summary>
        public readonly float MaxHp;

        /// <summary>
        /// Unit level. -1 when missing or not parseable.
        /// </summary>
        public readonly int Level;

        /// <summary>
        /// Whether the unit is alive. False when missing or not parseable.
        /// </summary>
        public readonly bool Alive;

        internal FullUnit(JObject parsed_data = null) : base(parsed_data)
        {
            Raw = parsed_data ?? new JObject();

            Type = GetStringSafe("type", "unitname", "name");
            Team = GetStringSafe("team");
            TeamID = GetIntSafe("team_id", "teamid");

            X = GetFloatSafe("xpos", "x");
            Y = GetFloatSafe("ypos", "y");

            float x_val = X;
            float y_val = Y;
            if (x_val < 0f && y_val < 0f)
            {
                // No position read; leave Location null.
                Location = null;
            }
            else
            {
                Location = new[] { x_val, y_val };
            }

            Hp = GetFloatSafe("hp", "health");
            MaxHp = GetFloatSafe("max_hp", "max_health", "maxhp");
            Level = GetIntSafe("level");
            Alive = GetBoolSafe("alive");
        }

        // --- tolerant helpers (do not modify Node base) ---

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

        private int GetIntSafe(params string[] keys)
        {
            if (_ParsedData == null || keys == null)
            {
                return -1;
            }

            foreach (var key in keys)
            {
                try
                {
                    var token = GetJToken(key);
                    if (token != null && token.Type != JTokenType.Null)
                    {
                        if (token.Type == JTokenType.Integer || token.Type == JTokenType.Float)
                        {
                            return Convert.ToInt32(token.ToObject<double>());
                        }

                        if (int.TryParse(token.ToString(), out int parsed))
                        {
                            return parsed;
                        }
                    }
                }
                catch
                {
                }
            }

            return -1;
        }

        private float GetFloatSafe(params string[] keys)
        {
            if (_ParsedData == null || keys == null)
            {
                return -1f;
            }

            foreach (var key in keys)
            {
                try
                {
                    var token = GetJToken(key);
                    if (token != null && token.Type != JTokenType.Null)
                    {
                        if (token.Type == JTokenType.Integer || token.Type == JTokenType.Float)
                        {
                            return Convert.ToSingle(token.ToObject<double>());
                        }

                        if (float.TryParse(token.ToString(), out float parsed))
                        {
                            return parsed;
                        }
                    }
                }
                catch
                {
                }
            }

            return -1f;
        }

        private bool GetBoolSafe(params string[] keys)
        {
            if (_ParsedData == null || keys == null)
            {
                return false;
            }

            foreach (var key in keys)
            {
                try
                {
                    var token = GetJToken(key);
                    if (token != null && token.Type != JTokenType.Null)
                    {
                        if (token.Type == JTokenType.Boolean)
                        {
                            return token.ToObject<bool>();
                        }

                        if (int.TryParse(token.ToString(), out int i))
                        {
                            return i != 0;
                        }

                        if (bool.TryParse(token.ToString(), out bool b))
                        {
                            return b;
                        }
                    }
                }
                catch
                {
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[" +
                $"Type: {Type}, " +
                $"Team: {Team}, " +
                $"TeamID: {TeamID}, " +
                $"Location: {(Location == null ? "[null]" : $"[{Location[0]}, {Location[1]}]")}, " +
                $"Hp: {Hp}, " +
                $"MaxHp: {MaxHp}, " +
                $"Level: {Level}, " +
                $"Alive: {Alive}" +
                $"]";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (null == obj)
            {
                return false;
            }

            return obj is FullUnit other &&
                Type.Equals(other.Type) &&
                Team.Equals(other.Team) &&
                TeamID.Equals(other.TeamID) &&
                X.Equals(other.X) &&
                Y.Equals(other.Y) &&
                Hp.Equals(other.Hp) &&
                MaxHp.Equals(other.MaxHp) &&
                Level.Equals(other.Level) &&
                Alive.Equals(other.Alive);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 157469321;
            hashCode = hashCode * -1521134295 + (Type?.GetHashCode() ?? 0);
            hashCode = hashCode * -1521134295 + (Team?.GetHashCode() ?? 0);
            hashCode = hashCode * -1521134295 + TeamID.GetHashCode();
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Hp.GetHashCode();
            hashCode = hashCode * -1521134295 + MaxHp.GetHashCode();
            hashCode = hashCode * -1521134295 + Level.GetHashCode();
            hashCode = hashCode * -1521134295 + Alive.GetHashCode();
            return hashCode;
        }
    }
}
