using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stratego.AITournament;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestAIPlayers()
        {
            var tourney = new AITournament<RandomAIPlayer, RandomAIPlayer>();
            Assert.Inconclusive();
        }
    }
}
