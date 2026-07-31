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

        public Exception? LastError { get; private set; }

        private bool _lastErrorAsserted;

        public void Execute(Action action)
        {
            try
            {
                action();
            }
            catch (Exception erreur) when (erreur is CoupInvalideException
                                                or ArgumentException
                                                or InvalidOperationException)
            {
                LastError = erreur;
                _lastErrorAsserted = false;
            }
        }

        /// Vérifie que la dernière action a bien été refusée par les règles.
        public void AssertRejected(string action)
        {
            Assert.IsNotNull(LastError, $"{action} : les règles ont accepté l'action alors qu'elles auraient dû la refuser.");
            _lastErrorAsserted = true;
        }

        /// Un refus jamais vérifié est une erreur avalée, donc un test faussement vert.
        public bool HasUnassertedError => LastError is not null && !_lastErrorAsserted;
    }
}
