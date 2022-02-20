using SimulationFramework.Desktop;

namespace Asteroids;

static class Program
{
    private static void Main()
    {
        using var game = new AsteroidsGame();
        game.RunWindowed("Asteroids", 1920, 1080);
    }
}