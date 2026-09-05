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
    }
}
