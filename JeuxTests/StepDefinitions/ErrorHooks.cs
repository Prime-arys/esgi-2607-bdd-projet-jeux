namespace JeuxBase.StepDefinitions
{
        [Binding]
    public sealed class ErrorHooks
    {
        private readonly GameContext _context;
        private readonly ScenarioContext _scenarioContext;

        public ErrorHooks(GameContext context, ScenarioContext scenarioContext)
        {
            _context = context;
            _scenarioContext = scenarioContext;
        }

        [AfterScenario]
        public void FailOnUnassertedError()
        {
            // Si le scénario a déjà échoué, on laisse sa propre erreur parler.
            if (_scenarioContext.TestError is not null)
            {
                return;
            }

            if (_context.HasUnassertedError)
            {
                Assert.Fail(
                    $"Une action du scénario a été refusée sans qu'aucun step ne le vérifie : {_context.LastError!.Message}");
            }
        }
    }
}
