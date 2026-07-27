namespace JeuxLibrary.Commun;

/// Base commune à tous les jeux : ce que le Manager doit pouvoir faire
/// sans rien savoir du jeu concret.
/// Les règles propres à un jeu (plateau, coups, ...) restent sur son type concret,
/// et s'atteignent via Manager.GetGame&lt;TicTacToe&gt;().
public abstract class Game
{
    protected readonly Manager manager;
    public GameStatus status { get; protected set; } = GameStatus.NotStarted;

    protected Game(Manager manager)
    {
        this.manager = manager;
    }

    public void ForceEndGame()
    {
        status = GameStatus.Finished;
    }

    public void AddScore(int score)
    {
        manager.CurrentPlayer.Score += score;
    }

    public abstract void StartGame();
    public abstract Player? GetWinner();
}
