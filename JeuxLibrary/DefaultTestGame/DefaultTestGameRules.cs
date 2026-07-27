using JeuxLibrary.Commun;

namespace JeuxLibrary.DefaultTestGame;

public class DefaultTestGameRules : Game
{
    public DefaultTestGameRules(Manager manager) : base(manager)
    {
    }

    public override void StartGame()
    {
        // Logique de démarrage du jeu
        // Par exemple, initialiser les scores des joueurs à zéro
        foreach (var player in manager.players)
        {
            player.Score = 0;
        }
    }
    
 
    public override Player? GetWinner()
    {
        if (manager.GetGame<DefaultTestGameRules>().status == GameStatus.Finished)
        {
            // jouer avec le score le plus élevé gagne
            return manager.players.OrderByDescending(p => p.Score).FirstOrDefault();
        }
        return null;
    }
}