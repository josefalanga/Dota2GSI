using Dota2GSI.Nodes;
using Dota2GSI.Nodes.Helpers;
using Newtonsoft.Json.Linq;
using System;

namespace Dota2GSI
{
    /// <summary>
    /// A class representing various information pertaining to Game State Integration of Dota 2.
    /// </summary>
    public class GameState : Node
    {
        /// <summary>
        /// Information about GSI authentication.<br/>
        /// Enabled by including <code>"auth" "1"</code> in the game state cfg file.
        /// </summary>
        public readonly Auth Auth;

        /// <summary>
        /// Information about the provider of this GameState.<br/>
        /// Enabled by including <code>"provider" "1"</code> in the game state cfg file.
        /// </summary>
        public readonly Provider Provider;

        /// <summary>
        /// Information about the current map.<br/>
        /// Enabled by including <code>"map" "1"</code> in the game state cfg file.
        /// </summary>
        public readonly Map Map;

        /// <summary>
        /// Information about the local player or team players when spectating.<br/>
        /// Enabled by including <code>"player" "1"</code> in the game state cfg file.
        /// </summary>
        public readonly Player Player;

        /// <summary>
        /// Information about the local player's hero or team players heroes when spectating.<br/>
        /// Enabled by including <code>"hero" "1"</code> in the game state cfg file.
        /// </summary>
        public readonly Hero Hero;

        /// <summary>
        /// Information about the local player's hero abilities or team players abilities when spectating.<br/>
        /// Enabled by including <code>"abilities" "1"</code> in the game state cfg file.
        /// </summary>
        public readonly Abilities Abilities;

        /// <summary>
        /// Information about the local player's hero items or team players items when spectating.<br/>
        /// Enabled by including <code>"items" "1"</code> in the game state cfg file.
        /// </summary>
        public readonly Items Items;

        /// <summary>
        /// Information about game events.<br/>
        /// Enabled by including <code>"events" "1"</code> in the game state cfg file.
        /// </summary>
        public readonly Events Events;

        /// <summary>
        /// Information about the buildings on the map.<br/>
        /// Enabled by including <code>"buildings" "1"</code> in the game state cfg file.
        /// </summary>
        public readonly Buildings Buildings;

        /// <summary>
        /// Information about the current league (or game configuration).<br/>
        /// Enabled by including <code>"league" "1"</code> in the game state cfg file.
        /// </summary>
        public readonly League League;

        /// <summary>
        /// Information about the draft. (TOURNAMENT ONLY)<br/>
        /// Enabled by including <code>"draft" "1"</code> in the game state cfg file.
        /// </summary>
        public readonly Draft Draft;

        /// <summary>
        /// Information about the local player's wearable items or team players wearable items when spectating.<br/>
        /// Enabled by including <code>"wearables" "1"</code> in the game state cfg file.
        /// </summary>
        public readonly Wearables Wearables;

        /// <summary>
        /// Information about the minimap.<br/>
        /// Enabled by including <code>"minimap" "1"</code> in the game state cfg file.
        /// </summary>
        public readonly Minimap Minimap;

        /// <summary>
        /// Information about Roshan. (SPECTATOR ONLY)<br/>
        /// Enabled by including <code>"roshan" "1"</code> in the game state cfg file.
        /// </summary>
        public readonly Roshan Roshan;

        /// <summary>
        /// Information about couriers. (SPECTATOR ONLY)<br/>
        /// Enabled by including <code>"couriers" "1"</code> in the game state cfg file.
        /// </summary>
        public readonly Couriers Couriers;

        /// <summary>
        /// Information about neutral items. (SPECTATOR ONLY)<br/>
        /// Enabled by including <code>"neutralitems" "1"</code> in the game state cfg file.
        /// </summary>
        public readonly NeutralItems NeutralItems;

        /// <summary>
        /// A previous GameState.
        /// </summary>
        public GameState Previously
        {
            get
            {
                if (_previous_game_state == null)
                {
                    JObject previously = GetJObject("previously");

                    if (previously == null)
                    {
                        // No "previously" block in this payload (first tick).
                        // Do not cache an empty state: return a transient one so a
                        // later payload containing "previously" is parsed correctly.
                        return new GameState();
                    }

                    _previous_game_state = new GameState(previously);
                }

                return _previous_game_state;
            }
        }
        /// <summary>
        /// The original parsed JObject.
        /// </summary>
        public JObject RawJson => _ParsedData;
        /// <summary>
        /// The added block containing newly-added sections since the last tick.
        /// </summary>
        public JObject Added => _added;

        /// <summary>
        /// Helper variable,<br/>
        /// Local player details derived from this game state.
        /// </summary>
        public FullPlayerDetails LocalPlayer
        {
            get
            {
                if (_local_player_details == null)
                {
                    _local_player_details = new FullPlayerDetails(this);
                }

                return _local_player_details;
            }
        }

        /// <summary>
        /// Helper variable,<br/>
        /// Radiant team details derived from this game state.
        /// </summary>
        public FullTeamDetails RadiantTeamDetails
        {
            get
            {
                if (_radiant_team_details == null)
                {
                    _radiant_team_details = new FullTeamDetails(PlayerTeam.Radiant, this);
                }

                return _radiant_team_details;
            }
        }

        /// <summary>
        /// Helper variable,<br/>
        /// Dire team details derived from this game state.
        /// </summary>
        public FullTeamDetails DireTeamDetails
        {
            get
            {
                if (_dire_team_details == null)
                {
                    _dire_team_details = new FullTeamDetails(PlayerTeam.Dire, this);
                }

                return _dire_team_details;
            }
        }

        /// <summary>
        /// Helper variable,<br/>
        /// Neutral team details derived from this game state.
        /// </summary>
        public FullTeamDetails NeutralTeamDetails
        {
            get
            {
                if (_neutral_team_details == null)
                {
                    _neutral_team_details = new FullTeamDetails(PlayerTeam.Neutrals, this);
                }

                return _neutral_team_details;
            }
        }

        /// <summary>
        /// Helper variable,<br/>
        /// Is the game client spectating a game?
        /// True if spectating, false otherwise.
        /// </summary>
        public bool IsSpectating
        {
            get
            {
                return Player.IsValid() && !Player.LocalPlayer.IsValid() && (Player.Teams.Count > 0);
            }
        }

        /// <summary>
        /// Helper variable,<br/>
        /// Is the game client playing a game?
        /// True if local player is playing a game, false otherwise.
        /// </summary>
        public bool IsLocalPlayer
        {
            get
            {
                return Player.IsValid() && Player.LocalPlayer.IsValid() && (Player.Teams.Count == 0);
            }
        }

        private GameState _previous_game_state;

        // Helpers

        private FullPlayerDetails _local_player_details;
        private FullTeamDetails _radiant_team_details;
        private FullTeamDetails _dire_team_details;
        private FullTeamDetails _neutral_team_details;
        private JObject _added;

        /// <summary>
        /// Creates a GameState instance based on the given json data.
        /// </summary>
        /// <param name="parsed_data">The parsed json data.</param>
        public GameState(JObject parsed_data = null) : base(parsed_data)
        {
            _added = parsed_data?.Value<JObject>("added");
            Auth = SafeCreate(() => new Auth(GetJObject("auth")), new Auth());
            Provider = SafeCreate(() => new Provider(GetJObject("provider")), new Provider());
            Map = SafeCreate(() => new Map(GetJObject("map")), new Map());
            Player = SafeCreate(() => new Player(GetJObject("player")), new Player());
            Hero = SafeCreate(() => new Hero(GetJObject("hero")), new Hero());
            Abilities = SafeCreate(() => new Abilities(GetJObject("abilities")), new Abilities());
            Items = SafeCreate(() => new Items(GetJObject("items")), new Items());
            Events = SafeCreate(() => new Events(GetJArray("events")), new Events());
            Buildings = SafeCreate(() => new Buildings(GetJObject("buildings")), new Buildings());
            League = SafeCreate(() => new League(GetJObject("league")), new League());
            Draft = SafeCreate(() => new Draft(GetJObject("draft")), new Draft());
            Wearables = SafeCreate(() => new Wearables(GetJObject("wearables")), new Wearables());
            Minimap = SafeCreate(() => new Minimap(GetJObject("minimap")), new Minimap());
            Roshan = SafeCreate(() => new Roshan(GetJObject("roshan")), new Roshan());
            Couriers = SafeCreate(() => new Couriers(GetJObject("couriers")), new Couriers());
            NeutralItems = SafeCreate(() => new NeutralItems(GetJObject("neutralitems")), new NeutralItems());
        }
    }
        private static T SafeCreate<T>(Func<T> create, T fallback)
        {
            try
            {
                return create();
            }
            catch (Exception)
            {
                // Malformed data for this node only; leave it as an absent node.
                return fallback;
            }
        }

}
