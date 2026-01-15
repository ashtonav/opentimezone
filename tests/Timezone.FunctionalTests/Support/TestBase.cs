namespace Timezone.FunctionalTests.Support;

public class TestBase
{
    protected ScenarioContext Context { get; set; }

    protected TestBase(ScenarioContext context) => Context = context;
}
