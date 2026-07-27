using JeuxLibrary;
using JeuxLibrary.Commun;

namespace JeuxBase.StepDefinitions
{
    [Binding]
    public sealed class JeuxStepDefinitions
    {
        // For additional details on Reqnroll step definitions see https://go.reqnroll.net/doc-stepdef

        private Jeux _target = null!;
        private List<Player> _players = new();

        [Given(@"the following players:")]
        public void GivenTheFollowingPlayers(Table table)
        {
            _players = new List<Player>();
            foreach (var row in table.Rows)
            {
                _players.Add(new Player { Name = row["Name"] });
            }
        }

        [When(@"I create a new game ""(.*)""")]
        public void WhenICreateANewGame(GameType gameType)
        {
            _target = new Jeux(gameType);
            _target.CreateGame(_players);
        }

        [When(@"the turn is ended")]
        public void WhenTheTurnIsEnded()
        {
            _target.GameManager?.NextPlayer();
        }

        [When(@"I update the score of the current player by (.*)")]
        public void WhenIUpdateTheScoreOfTheCurrentPlayerBy(int score)
        {
            _target.GameManager?.AddScore(score);
        }

        [When(@"the game is ended")]
        public void WhenTheGameIsEnded()
        {
            _target.GameManager?.EndGame();
        }

        [Then(@"the score of (.*) should be (.*)")]
        public void ThenTheScoreOfShouldBe(string playerName, int expectedScore)
        {
            var player = _target.GameManager?.players.FirstOrDefault(p => p.Name == playerName);
            Assert.IsNotNull(player, $"Player '{playerName}' not found.");
            Assert.AreEqual(expectedScore, player.Score);
        }

        [Then(@"the current player should be (.*)")]
        public void ThenTheCurrentPlayerShouldBe(string expectedPlayerName)
        {
            var actualPlayerName = _target.GameManager?.CurrentPlayer.Name;
            Assert.AreEqual(expectedPlayerName, actualPlayerName);
        }

        [Then(@"the game should be created with (.*) players")]
        public void ThenTheGameShouldBeCreatedWithPlayers(int expectedPlayerCount)
        {
            var actualPlayerCount = _target.GameManager?.players.Count ?? 0;
            Assert.AreEqual(expectedPlayerCount, actualPlayerCount);
        }

        [Then(@"the game should be over")]
        public void ThenTheGameShouldBeOver()
        {
            Assert.IsTrue(_target.GameManager?.IsGameOver ?? false);
        }

        [Then(@"the winner should be (.*)")]
        public void ThenTheWinnerShouldBe(string expectedWinnerName)
        {
            var winner = _target.GameManager?._winner;
            Assert.IsNotNull(winner, "No winner found.");
            Assert.AreEqual(expectedWinnerName, winner.Name);
        }
        
    }
}
