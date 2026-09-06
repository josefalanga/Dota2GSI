using Xunit;
using Newtonsoft.Json.Linq;

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
    }
}
