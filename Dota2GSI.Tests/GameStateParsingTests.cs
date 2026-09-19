using Xunit;
using Newtonsoft.Json.Linq;
using Dota2GSI.Nodes;
using Dota2GSI.Nodes.EventsProvider;

namespace Dota2GSI.Tests
{
    public class GameStateParsingTests
    {
        [Fact]
        public void MapNameAndClockTime_ParseFromMinimalJson()
        {
            var json = JObject.Parse(@"{
                ""map"": {
                    ""name"": ""start"",
                    ""matchid"": ""1234"",
                    ""clock_time"": -1
                }
            }");

            var state = new GameState(json);

            Assert.Equal("start", state.Map.Name);
            Assert.Equal(-1, state.Map.ClockTime);
        }

        [Fact]
        public void GlyphAndScanCooldowns_ParseAsFloat_FromRealWireValues()
        {
            // Real spectate payload (match 8984841005): cooldowns arrive as
            // fractional floats (21.318603515625, 171.3519287109375). GetInt
            // truncates to whole seconds; GetFloat keeps the fraction.
            var json = JObject.Parse(@"{
                ""map"": {
                    ""radiant_glyph_cooldown"": 21.318603515625,
                    ""dire_glyph_cooldown"": 171.3519287109375,
                    ""radiant_scan_cooldown"": 13.75,
                    ""radiant_scan_charges"": 3,
                    ""dire_scan_cooldown"": 268.5697937011719,
                    ""dire_scan_charges"": 2
                }
            }");

            var state = new GameState(json);

            Assert.Equal(21.318603515625f, state.Map.RadiantGlyphCooldown);
            Assert.Equal(171.3519287109375f, state.Map.DireGlyphCooldown);
            Assert.Equal(13.75f, state.Map.RadiantScanCooldown);
            Assert.Equal(3, state.Map.RadiantScanCharges);
            Assert.Equal(268.5697937011719f, state.Map.DireScanCooldown);
            Assert.Equal(2, state.Map.DireScanCharges);
        }

        [Fact]
        public void GlyphAndScanCooldowns_DefaultToNegativeOne_WhenKeyAbsent()
        {
            // Cooldown keys are omitted from the payload when the ability is
            // off cooldown on the live wire (not sent as 0). The lib's numeric
            // defaults are -1 (same as GetInt), consistent across parsers.
            var json = JObject.Parse(@"{ ""map"": { } }");

            var state = new GameState(json);

            Assert.Equal(-1f, state.Map.RadiantGlyphCooldown);
            Assert.Equal(-1f, state.Map.DireScanCooldown);
        }

        [Fact]
        public void GlyphAndScanCooldowns_ParseIntValuesWithoutCrashing()
        {
            // Corpus contains whole-second cooldown hits stored as int BSON
            // (53 docs in match 8984806220). Numeric coercion must not throw.
            var json = JObject.Parse(@"{
                ""map"": {
                    ""radiant_glyph_cooldown"": 21,
                    ""dire_scan_cooldown"": 171
                }
            }");

            var state = new GameState(json);

            Assert.Equal(21f, state.Map.RadiantGlyphCooldown);
            Assert.Equal(171f, state.Map.DireScanCooldown);
        }

        [Fact]
        public void Wearables_TeamPlayers_ParseFromNestedTeamObjects()
        {
            // Regression: the player-id scan used to run against the root
            // object instead of the team object, so team wearables were lost
            // (137/153 captured paths unreachable).
            var json = JObject.Parse(@"{
                ""wearables"": {
                    ""team2"": {
                        ""player0"": { ""wearable0"": 1001, ""style0"": 2 },
                        ""player1"": { ""wearable0"": 1002 }
                    }
                }
            }");

            var state = new GameState(json);

            var radiant = state.Wearables.GetForTeam(PlayerTeam.Radiant);
            Assert.True(radiant.ContainsKey(0));
            Assert.True(radiant.ContainsKey(1));
            Assert.Equal(1001, radiant[0].Wearables[0].ID);
            Assert.Equal(2, radiant[0].Wearables[0].Style);
            Assert.Equal(1002, radiant[1].Wearables[0].ID);
        }

        [Fact]
        public void PreviouslyEvents_SingleWrappedEvent_ParsesFromObjectShape()
        {
            // The "previously" delta block emits events as {"event": {...}}
            // instead of a bare array; the parser must accept both.
            var json = JObject.Parse(@"{
                ""previously"": {
                    ""events"": {
                        ""event"": {
                            ""game_time"": 136,
                            ""event_type"": ""generic_event"",
                            ""data"": ""{\""type\"":\""CHAT_MESSAGE_INTHEBAG\"",\""playerid1\"":0}""
                        }
                    }
                }
            }");

            var state = new GameState(json);

            Assert.Equal(1, state.Previously.Events.Count);
            Assert.Equal(136, state.Previously.Events[0].GameTime);
            Assert.Equal(GenericEventType.Inthebag, state.Previously.Events[0].Data.GenericType);
        }

        [Fact]
        public void MinimapElement_WatcherTeamFive_ParsesAsWatcher()
        {
            // Watcher units (npc_dota_lantern) report team 5, previously an
            // unnamed enum member that fell through to Undefined.
            var json = JObject.Parse(@"{
                ""minimap"": {
                    ""o100"": { ""xpos"": 940, ""ypos"": 80, ""team"": 5 }
                }
            }");

            var state = new GameState(json);

            var watcher = state.Minimap.GetForTeam(PlayerTeam.Watcher);
            Assert.True(watcher.Count > 0);
        }

        [Fact]
        public void NeutralItems_BothSlots_ParseWithoutOverwrite()
        {
            // Both neutral0 and neutral1 are emitted; neutral1 used to
            // overwrite neutral0, losing the primary neutral item.
            var json = JObject.Parse(@"{
                ""items"": {
                    ""local"": {
                    },
                    ""team2"": {
                        ""player0"": {
                            ""neutral0"": { ""name"": ""item_story_walker"" },
                            ""neutral1"": { ""name"": ""item_lance_of_avernus"" }
                        }
                    }
                }
            }");

            var state = new GameState(json);

            var player = state.Items.GetForTeam(PlayerTeam.Radiant)[0];
            Assert.Equal("item_story_walker", player.Neutral.Name);
            Assert.Equal("item_lance_of_avernus", player.Neutral1.Name);
        }

        [Fact]
        public void GenericEvent_Value3Overflow_ParsesAsLong()
        {
            // STREAK_KILL broadcasts carry uint-max (-1) in value3, which
            // overflows int parsing and collapsed to 0.
            var json = JObject.Parse(@"{
                ""events"": [
                    {
                        ""game_time"": 300,
                        ""event_type"": ""generic_event"",
                        ""data"": ""{\""type\"":\""CHAT_MESSAGE_STREAK_KILL\"",\""value\"":121,\""value2\"":0,\""value3\"":4294967295,\""playerid1\"":8}""
                    }
                ]
            }");

            var state = new GameState(json);

            Assert.Equal(1, state.Events.Count);
            Assert.Equal(GenericEventType.Streak_kill, state.Events[0].Data.GenericType);
            Assert.Equal(4294967295L, state.Events[0].Data.Value3);
        }

        [Fact]
        public void GenericEvent_ChatMessageRandom_ParsesHeroIdAndSlot()
        {
            // A hero randomed during the pick phase: playerid1 is the randomed
            // hero id and value is the player's slot (0-4 radiant, 5-9 dire).
            // Untyped before, the raw payload was unusable for the lineup.
            var json = JObject.Parse(@"{
                ""events"": [
                    {
                        ""game_time"": 55,
                        ""event_type"": ""generic_event"",
                        ""data"": ""{\""type\"":\""CHAT_MESSAGE_RANDOM\"",\""playerid1\"":72,\""value\"":6,\""playerid2\"":-1,\""playerid3\"":-1,\""time\"":-22.8}""
                    }
                ]
            }");

            var state = new GameState(json);

            Assert.Equal(1, state.Events.Count);
            Assert.Equal(GenericEventType.Random, state.Events[0].Data.GenericType);
            Assert.Equal(72, state.Events[0].Data.HeroId);
            Assert.Equal(72, state.Events[0].Data.PlayerID1);
            Assert.Equal(6, state.Events[0].Data.Value);
        }

        [Fact]
        public void Yaw_FloatValues_ParseWithoutCrashing()
        {
            // yaw arrives as whole ints today but can be fractional; int
            // parsing ("135.5") throws, float parsing must not.
            var json = JObject.Parse(@"{
                ""roshan"": { ""yaw"": 135.5 },
                ""couriers"": { ""courier0"": { ""yaw"": 75.25 } },
                ""minimap"": { ""o5"": { ""yaw"": 12.75 } }
            }");

            var state = new GameState(json);

            Assert.Equal(135, state.Roshan.Rotation);
            Assert.Equal(75, state.Couriers.CouriersMap[0].Rotation);
            Assert.Equal(12, state.Minimap.Elements[5].Rotation);
        }
    }
}
