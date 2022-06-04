using SimulationFramework.Desktop;

namespace Asteroids;

static class Program
{
    private static void Main()
    {
        var game = new AsteroidsGame();
        game.RunDesktop();
    }
}