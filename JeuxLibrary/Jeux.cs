
using JeuxLibrary.Commun;

namespace JeuxLibrary;

public class Jeux
{
    public GameType Type { get; set; }
    public Manager? GameManager { get; set; }

    public Jeux(GameType type)
    {
        Type = type;
    }

    public void CreateGame(List<Player> players)
    {
        GameManager = new Manager(Type);
        GameManager.SetPlayers(players);
        GameManager.StartGame();
    }
}