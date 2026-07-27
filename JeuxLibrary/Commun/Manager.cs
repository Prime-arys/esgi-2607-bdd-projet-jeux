using JeuxLibrary.TicTacToeGame;
using JeuxLibrary.DefaultTestGame;

namespace JeuxLibrary.Commun;

/// Gestion générale des jeux
/// Joueur, plateau, règles, score, etc.
public class Manager
{
    public List<Player> players = new List<Player>();
    private readonly Game _game;
    private int _currentPlayerIndex = 0;

    public Manager(GameType type, List<Player> players)
    {
        this.players = players;
        _game = type switch
        {
            GameType.TicTacToe => new TicTacToe(this),
            // GameType.Mastermind => new MastermindRules(this),
            // GameType.Darts => new DartsRules(this),
            GameType.DefaultTestGame => new DefaultTestGameRules(this),
            _ => throw new ArgumentException("Invalid game type")
        };
    }

    public Player CurrentPlayer => players[_currentPlayerIndex];

    public void NextPlayer()
    {
        _currentPlayerIndex = (_currentPlayerIndex + 1) % players.Count;
    }

    // public void AddScore(int score)
    // {
    //     CurrentPlayer.Score += score;
    // }

    public void StartGame()
    {
        _game.StartGame();
    }

    public Game GetGame()
    {
        return _game;
    }

    /// Accès typé au jeu en cours, pour atteindre les règles propres à ce jeu :
    /// GetGame&lt;TicTacToe&gt;().GetGameBoard()
    public TGame GetGame<TGame>() where TGame : Game
    {
        return _game as TGame
            ?? throw new InvalidOperationException(
                $"La partie en cours est un {_game.GetType().Name}, pas un {typeof(TGame).Name}.");
    }

}
