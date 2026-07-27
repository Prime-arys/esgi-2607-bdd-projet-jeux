using JeuxLibrary.Commun;

public class DefaultTestGameRules : IGame
{

    private Manager manager;
    public DefaultTestGameRules(Manager manager)
    {
        this.manager = manager;
    }

    public void StartGame()
    {
        // Logique de démarrage du jeu
        // Par exemple, initialiser les scores des joueurs à zéro
        foreach (var player in manager.players)
        {
            player.Score = 0;
        }
    }
    
 
    public Player? GetWinner()
    {
        if (manager.IsGameOver)
        {
            // jouer avec le score le plus élevé gagne
            return manager.players.OrderByDescending(p => p.Score).FirstOrDefault();
        }
        return null;
    }
}