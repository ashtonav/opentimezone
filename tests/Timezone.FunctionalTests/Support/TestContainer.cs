namespace Timezone.FunctionalTests.Support;

using DotNet.Testcontainers.Builders;

[Binding]
internal static class TestContainer
{
    internal static HttpClient Client { get; set; }

    [BeforeTestRun]
    internal static async Task BeforeTestRunInjection()
    {
        var solutionFolder = CommonDirectoryPath.GetSolutionDirectory();

        var image = new ImageFromDockerfileBuilder()
            .WithContextDirectory(solutionFolder.DirectoryPath)
            .WithDockerfileDirectory(solutionFolder, TestConstants.OpenTimezoneWebApiContainerPath)
            .Build();
        await image.CreateAsync();

        var container = new ContainerBuilder(image)
            .WithPortBinding(TestConstants.OpenTimezoneWebApiContainerPort, true)
            .Build();
        await container.StartAsync();

        Client = new HttpClient { BaseAddress = new Uri($"http://{container.Hostname}:{container.GetMappedPublicPort()}") };

        await WaitForTestContainerToBeReady();
    }

    private static async Task WaitForTestContainerToBeReady()
    {
        for (var i = 0; i < 100; i++)
        {
            try
            {
                var result = await Client.GetAsync("/");

                if (result.IsSuccessStatusCode)
                {
                    break;
                }
            }
            catch (Exception)
            {
                // ignored
            }
            await Task.Delay(100);
        }
    }
}
