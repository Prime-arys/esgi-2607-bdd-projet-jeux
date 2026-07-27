namespace JeuxLibrary.Commun;

/// Gestion générale des jeux
/// Joueur, plateau, règles, score, etc.
public class Manager
{
    public List<Player> players = new List<Player>();
    private IGame Game;
    private int _currentPlayerIndex = 0;
    public bool IsGameOver { get; private set; } = false;
    public Player? _winner {get; private set; } = null;

    public Manager(GameType type)
    {
        Game = type switch
        {
            // GameType.TicTacToe => new TicTacToeRules(this),
            // GameType.Mastermind => new MastermindRules(this),
            // GameType.Darts => new DartsRules(this),
            GameType.DefaultTestGame => new DefaultTestGameRules(this),
            _ => throw new ArgumentException("Invalid game type")
        };
    }

    public void SetPlayers(List<Player> players)
    {
        this.players = players;
    }

    public Player CurrentPlayer => players[_currentPlayerIndex];

    public void NextPlayer()
    {
        _currentPlayerIndex = (_currentPlayerIndex + 1) % players.Count;
    }

    public void AddScore(int score)
    {
        CurrentPlayer.Score += score;
    }

    public void StartGame()
    {
        Game.StartGame();
    }

    public void EndGame()
    {
        IsGameOver = true;
        _winner = Game.GetWinner();

    }

    public IGame GetGame()
    {
        return Game;
    }
    
}
