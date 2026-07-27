using JeuxLibrary;
using JeuxLibrary.Commun;

namespace JeuxBase.StepDefinitions
{
    /// État partagé par les classes de steps d'un même scénario.
    /// Reqnroll en crée une instance par scénario et l'injecte dans les constructeurs.
    /// Les joueurs vivent ici (et pas dans une classe de steps) parce qu'ils sont
    /// construits par un step générique et relus par les steps propres à un jeu.
    public class GameContext
    {
        public Jeux Target { get; set; } = null!;
        public List<Player> Players { get; set; } = new();
    }
}
