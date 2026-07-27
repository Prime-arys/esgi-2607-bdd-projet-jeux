using JeuxLibrary;
using JeuxLibrary.Commun;
using JeuxLibrary.TicTacToeGame;

namespace JeuxBase.StepDefinitions
{
    /// Steps propres au morpion. Les steps génériques (joueurs, création de partie,
    /// tour courant) vivent dans JeuxStepDefinitions et sont partagés via GameContext.
    [Binding]
    public sealed class TicTacToeStepDefinitions
    {
        private readonly GameContext _context;

        public TicTacToeStepDefinitions(GameContext context)
        {
            _context = context;
        }

        /// Le jeu en cours vu comme un morpion : c'est ici, et pas sur Manager,
        /// que l'on connaît les règles propres au TicTacToe.
        private TicTacToe Game => _context.Target.GameManager!.GetGame<TicTacToe>();

        /// StepDefinition (et non Given) pour que le step se place indifféremment
        /// après un Given ou un When : le symbole ne se choisit qu'une fois la partie créée.
        [StepDefinition(@"with (.*) playing with marker (.*)")]
        public void WithPlayingWithSymbol(string playerName, Symbole symbole)
        {
            var player = _context.Players.FirstOrDefault(p => p.Name == playerName);
            Assert.IsNotNull(player, $"Player '{playerName}' not found.");
            Game.ChoseSymbole(player, symbole);
        }

        [When(@"the current player plays at position \((.*), (.*)\)")]
        public void WhenTheCurrentPlayerPlaysAtPosition(int ligne, string colonne)
        {
            Game.Play(Case.Analyser($"{colonne}{ligne}"));
        }

        [Then(@"the game board should look like:")]
        public void ThenTheGameBoardShouldLookLike(Table expectedBoard)
        {
            var actualBoard = Game.GetGameBoard();
            Assert.IsNotNull(actualBoard, "Game board is null.");

            // La première colonne du tableau ("row") porte le numéro de ligne,
            // les colonnes A à C suivent : la case (i, j) est en Rows[i][j + 1].
            for (int i = 0; i < expectedBoard.RowCount; i++)
            {
                for (int j = 0; j < Case.Taille; j++)
                {
                    var expectedValue = expectedBoard.Rows[i][j + 1];
                    var actualValue = Afficher(actualBoard[i, j]);
                    Assert.AreEqual(expectedValue, actualValue, $"Mismatch at position ({i}, {j}).");
                }
            }
        }

        /// Notation du plateau telle qu'écrite dans les scénarios : "." pour une case vide.
        private static string Afficher(Symbole symbole) =>
            symbole == Symbole.Vide ? "." : symbole.ToString();
    }
}
