using JeuxLibrary.Commun;
using JeuxLibrary.Darts;

namespace JeuxBase.StepDefinitions
{
    [Binding]
    public sealed class DartsStepDefinitions
    {
        private readonly GameContext _context;
        private ResultatLancer? _dernierResultat;

        public DartsStepDefinitions(GameContext context)
        {
            _context = context;
        }

        private DartsGame Game => _context.Target.GameManager!.GetGame<DartsGame>();

        [StepDefinition(@"each player starts at (\d+)")]
        public void EachPlayerStartsAt(int scoreDeDepart)
        {
            _context.Execute(() => Game.ChoisirScoreDeDepart(scoreDeDepart));
        }

        [When(@"the current player throws (.*)")]
        public void WhenTheCurrentPlayerThrows(string volee)
        {
            _context.Execute(() =>
            {
                foreach (var notation in volee.Split(','))
                {
                    _dernierResultat = Game.Lancer(Dart.Analyser(notation));

                    if (_dernierResultat != ResultatLancer.Decompte)
                    {
                        break;
                    }
                }
            });
        }

        [Then(@"the remaining score of (.*) should be (\d+)")]
        public void ThenTheRemainingScoreOfShouldBe(string playerName, int expectedScore)
        {
            var player = TrouverJoueur(playerName);
            Assert.AreEqual(expectedScore, Game.ScoreRestant(player), $"Remaining score of '{playerName}'.");
        }

        [Then(@"the current player should have (\d+) darts? left in the volley")]
        public void ThenTheCurrentPlayerShouldHaveDartsLeft(int expectedDarts)
        {
            Assert.AreEqual(expectedDarts, Game.FlechettesRestantesDansLaVolee);
        }

        [Then(@"the volley should be a bust")]
        public void ThenTheVolleyShouldBeABust()
        {
            Assert.AreEqual(ResultatLancer.Depassement, _dernierResultat, "Last throw of the volley.");
        }

        [Then(@"the throw should win the leg")]
        public void ThenTheThrowShouldWinTheLeg()
        {
            Assert.AreEqual(ResultatLancer.Victoire, _dernierResultat, "Last throw of the volley.");
        }

        [Then(@"the throw should be rejected")]
        public void ThenTheThrowShouldBeRejected()
        {
            _context.AssertRejected("Lancer");
        }

        private Player TrouverJoueur(string playerName)
        {
            var player = _context.Target.GameManager?.players.FirstOrDefault(p => p.Name == playerName);
            Assert.IsNotNull(player, $"Player '{playerName}' not found.");
            return player;
        }
    }
}
