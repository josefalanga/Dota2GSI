using Dota2GSI.EventMessages;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace Dota2GSI
{
    /// <summary>
    /// Delegate for handing new game state changes.
    /// </summary>
    /// <param name="gamestate">The new game state.</param>
    public delegate void NewGameStateHandler(GameState gamestate);

    /// <summary>
    /// Delegate for handing game events.
    /// </summary>
    /// <param name="game_event">The new game event.</param>
    public delegate void GameEventHandler(DotaGameEvent game_event);

    /// <summary>
    /// 
    /// </summary>
    public class GameStateListener : Dota2EventsInterface, IDisposable
    {
        /// <summary>
        /// The previous game state.
        /// </summary>
        public GameState PreviousGameState
        {
            get
            {
                return _previous_game_state;
            }
        }

        /// <summary>
        /// The current game state.
        /// </summary>
        public GameState CurrentGameState
        {
            get
            {
                lock (gamestate_lock)
                {
                    return _current_game_state;
                }
            }
            private set
            {
                lock (gamestate_lock)
                {
                    if (_current_game_state.Equals(value))
                    {
                        return;
                    }

                    _previous_game_state = _current_game_state;
                    _current_game_state = value;
                    RaiseOnNewGameState(ref _current_game_state);
<<<<<<< HEAD
                    if (value.Added != null && value.Added.HasValues)
                    {
                        foreach (var prop in value.Added.Properties())
                        {
                            if (prop.Value is JObject leafObject)
                            {
                                RaiseOnLeafChanged(prop.Name, leafObject);
                            }
                        }
                    }
=======
>>>>>>> fix/listener-dispose
                }
            }
        }

        /// <summary>
        /// Gets the port that is being listened.
        /// </summary>
        public int Port => _port;

        /// <summary>
        /// Gets the URI that is being listened.
        /// </summary>
        public string URI => _uri;

        /// <summary>
        /// Returns whether or not the listener is running.
        /// </summary>
        public bool Running { get { return _is_running; } }
        // Disposal tracking for idempotent Dispose().
<<<<<<< HEAD
        private bool _disposed = false;
=======
        private bool _disposed = false;
>>>>>>> fix/listener-dispose

        /// <summary>
        /// Event for handing a newly received game state.
        /// </summary>
        public event NewGameStateHandler NewGameState = delegate { };

<<<<<<< HEAD
        /// <summary>
        /// Event for handing per-leaf changes from the Added block.
        /// </summary>
        public event Action<string, JObject> LeafChanged = delegate { };

=======
>>>>>>> fix/listener-dispose
        private readonly object gamestate_lock = new object();

        private bool _is_running = false;
        private int _port;
        private string _uri;
        private HttpListener _http_listener;
        private AutoResetEvent _wait_for_connection = new AutoResetEvent(false);
        private GameState _previous_game_state = new GameState();
        private GameState _current_game_state = new GameState();

        // Dispatcher for game events.
<<<<<<< HEAD
        private EventDispatcher<DotaGameEvent> _dispatcher = new EventDispatcher<DotaGameEvent>();
=======
        private static EventDispatcher<DotaGameEvent> _dispatcher = new EventDispatcher<DotaGameEvent>();
>>>>>>> fix/listener-dispose

        // Game State handlers.
        private AbilitiesHandler _abilities_handler = new AbilitiesHandler(ref _dispatcher);
        private AuthHandler _auth_handler = new AuthHandler(ref _dispatcher);
        private BuildingsHandler _buildings_handler = new BuildingsHandler(ref _dispatcher);
        private CouriersHandler _couriers_handler = new CouriersHandler(ref _dispatcher);
        private DraftHandler _draft_handler = new DraftHandler(ref _dispatcher);
        private GameplayEventsHandler _gameplay_events_handler = new GameplayEventsHandler(ref _dispatcher);
        private HeroHandler _hero_handler = new HeroHandler(ref _dispatcher);
        private ItemsHandler _items_handler = new ItemsHandler(ref _dispatcher);
        private LeagueHandler _league_handler = new LeagueHandler(ref _dispatcher);
        private MapHandler _map_handler = new MapHandler(ref _dispatcher);
        private MinimapHandler _minimap_handler = new MinimapHandler(ref _dispatcher);
        private NeutralItemsHandler _neutral_items_handler = new NeutralItemsHandler(ref _dispatcher);
        private PlayerHandler _player_handler = new PlayerHandler(ref _dispatcher);
        private ProviderHandler _provider_handler = new ProviderHandler(ref _dispatcher);
        private RoshanHandler _roshan_handler = new RoshanHandler(ref _dispatcher);
        private WearablesHandler _wearables_handler = new WearablesHandler(ref _dispatcher);

        // Custom handlers.
        private FullDetailsHandler _full_details_handler = new FullDetailsHandler(ref _dispatcher);

        // Overall GameState handler.
        private GameStateHandler _game_state_handler = new GameStateHandler(ref _dispatcher);

        /// <summary>
        /// Default constructor.
        /// </summary>
        private GameStateListener()
        {
            _dispatcher.GameEvent += OnNewGameEvent;
        }

        /// <summary>
        /// A GameStateListener that listens for connections on http://localhost:<c>port</c>/.
        /// </summary>
        /// <param name="port">The port to listen on.</param>
        public GameStateListener(int port) : this()
        {
            _port = port;
            _uri = $"http://localhost:{_port}/";
            _http_listener = new HttpListener();
            _http_listener.Prefixes.Add(_uri);
        }

        /// <summary>
        /// A GameStateListener that listens for connections to the specified URI.
        /// </summary>
        /// <param name="URI">The URI to listen to.</param>
        public GameStateListener(string URI) : this()
        {
            if (!URI.EndsWith("/"))
            {
                URI += "/";
            }

            Regex URIPattern = new Regex("^https?:\\/\\/.+:([0-9]*)\\/$", RegexOptions.IgnoreCase);
            Match PortMatch = URIPattern.Match(URI);

            if (!PortMatch.Success)
            {
                throw new ArgumentException("Not a valid URI: " + URI);
            }

            _port = Convert.ToInt32(PortMatch.Groups[1].Value);
            _uri = URI;
            _http_listener = new HttpListener();
            _http_listener.Prefixes.Add(URI);
        }

        /// <summary>
        /// Attempts to create a Game State Integration configuraion file.
        /// </summary>
        /// <param name="name">The name of your integration.</param>
        /// <returns>Returns true on success, false otherwise.</returns>
        public bool GenerateGSIConfigFile(string name)
        {
            return Dota2GSIFile.CreateFile(name, _uri);
        }

        /// <summary>
        /// Starts listening for GameState requests.
        /// </summary>
        public bool Start()
        {
            if (!_is_running)
            {
                Thread ListenerThread = new Thread(new ThreadStart(Run));
                try
                {
                    _http_listener.Start();
                }
                catch (HttpListenerException)
                {
                    return false;
                }
                _is_running = true;

                // Set this to true, so when the program wants to terminate,
                // this thread won't stop the program from exiting.
                ListenerThread.IsBackground = true;

                ListenerThread.Start();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Stops listening for GameState requests.
        /// </summary>
        public void Stop()
        {
            _is_running = false;
            try { _http_listener.Stop(); } catch { }
        }

        private void Run()
        {
            while (_is_running)
            {
                _http_listener.BeginGetContext(ReceiveGameState, _http_listener);
                if (!_wait_for_connection.WaitOne(100))
                {
                    continue;
                }
                _wait_for_connection.Reset();
            }
            _http_listener.Stop();
        }

        private void ReceiveGameState(IAsyncResult result)
        {
            try
            {
                HttpListenerContext context = _http_listener.EndGetContext(result);
                HttpListenerRequest request = context.Request;
                string json_data;

                _wait_for_connection.Set();

                using (Stream inputStream = request.InputStream)
                {
                    using (StreamReader sr = new StreamReader(inputStream))
                    {
                        json_data = sr.ReadToEnd();
                    }
                }
                using (HttpListenerResponse response = context.Response)
                {
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.StatusDescription = "OK";
                    response.Close();
                }

<<<<<<< HEAD
                JObject parsed_data;
                try
                {
                    parsed_data = JObject.Parse(json_data);
                }
                catch (Exception)
                {
                    // Malformed top-level JSON; ignore this tick and keep listening.
                    return;
                }

                CurrentGameState = new GameState(parsed_data);
            }
            catch (Exception)
            {
                // Never let an exception escape the tick handler.
=======
                CurrentGameState = new GameState(JObject.Parse(json_data));
>>>>>>> fix/listener-dispose
            }
            catch (ObjectDisposedException)
            {
                // Intentionally left blank, when the Listener is closed.
            }
        }

        private void RaiseOnNewGameState(ref GameState game_state)
        {
            RaiseEvent(NewGameState, game_state);

            _game_state_handler.OnNewGameState(game_state);
        }

<<<<<<< HEAD
        private void RaiseOnLeafChanged(string leafName, JObject leafValue)
        {
            LeafChanged?.Invoke(leafName, leafValue);
        }

=======
>>>>>>> fix/listener-dispose
        /// <summary>
        /// Stops the listener and frees up resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            Stop();
            _wait_for_connection.Dispose();
            try { _http_listener.Close(); } catch { }
        }
    }
}
