using JeuxLibrary;
using JeuxLibrary.Commun;
using JeuxLibrary.DefaultTestGame;

namespace JeuxBase.StepDefinitions
{
    [Binding]
    public sealed class JeuxStepDefinitions
    {
        // For additional details on Reqnroll step definitions see https://go.reqnroll.net/doc-stepdef

        private readonly GameContext _context;

        public JeuxStepDefinitions(GameContext context)
        {
            _context = context;
        }

        private Jeux _target => _context.Target;

        [Given(@"the following players:")]
        public void GivenTheFollowingPlayers(Table table)
        {
            _context.Players = new List<Player>();
            foreach (var row in table.Rows)
            {
                _context.Players.Add(new Player { Name = row["Name"] });
            }
        }

        [When(@"I create a new game ""(.*)""")]
        public void WhenICreateANewGame(GameType gameType)
        {
            _context.Target = new Jeux(gameType);
            _target.CreateGame(_context.Players);
        }

        [When(@"the turn is ended")]
        public void WhenTheTurnIsEnded()
        {
            _target.GameManager?.NextPlayer();
        }

        [When(@"I update the score of the current player by (.*)")]
        public void WhenIUpdateTheScoreOfTheCurrentPlayerBy(int score)
        {
            _target.GameManager?.GetGame().AddScore(score);
        }

        [When(@"the game is ended")]
        public void WhenTheGameIsEnded()
        {
            _target.GameManager?.GetGame().ForceEndGame();
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
            Assert.IsTrue(_target.GameManager?.GetGame().status == GameStatus.Finished, "The game is not over.");
        }

        [Then(@"the winner should be (.*)")]
        public void ThenTheWinnerShouldBe(string expectedWinnerName)
        {
            var winner = _target.GameManager?.GetGame().GetWinner();
            Assert.IsNotNull(winner, "No winner found.");
            Assert.AreEqual(expectedWinnerName, winner.Name);
        }
        
    }
}
