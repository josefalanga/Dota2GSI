# Dota2GSI Capture

Captures the **raw** GSI JSON payloads that Dota 2 posts to a game state
integration endpoint, one file per tick. These are the ground truth used to
build and verify typed nodes in the Dota2GSI library.

## Run

```
/home/falanga/.dotnet/dotnet run --project "Dota2GSI Capture program/Dota2GSI Capture program.csproj" [port] [outputDir]
```

- `port` — listen port (default `3000`). Overridable via `GSI_PORT` env var.
- `outputDir` — root output directory (default `<program dir>/captured`).

Each run creates a new timestamped directory:

```
captured/<run-timestamp>/<tick-sequence>.json
```

e.g. `captured/20260905-160000/00000001.json`. Sequence numbers are
zero-padded 8 digits, incrementing per received tick. Quit with ESC.

## Point Dota 2 at the listener

Create a file named

```
<steam-install>/steamapps/common/dota 2 beta/game/dota/cfg/gamestate_integration/gamestate_integration_capture.cfg
```

with these contents (adjust the port to match what you passed above):

```
"Dota2 GSI Capture"
{
    "uri"       "http://127.0.0.1:3000/"
    "timeout"   "5.0"
    "buffer"    "0.1"
    "throttle"  "0.5"
    "heartbeat" "30.0"
    "data"
    {
        "provider"  "1"
        "map"       "1"
        "player"    "1"
        "hero"      "1"
        "abilities" "1"
        "items"     "1"
        "events"    "1"
        "buildings" "1"
        "draft"     "1"
        "wearables" "1"
        "auth"      "1"
    }
}
```

Enable every block you want captured. Once the game is running, the listener
writes each raw payload to `captured/`. These files are the canonical source
for typed-node additions — contribute the exact JSON when proposing new nodes
(e.g. neutral item slots, `facetIndex`, couriers).