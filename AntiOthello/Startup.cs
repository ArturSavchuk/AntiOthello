using System;
using AntiOthello.Model;
using AntiOthello.View;
using AntiOthello.Controller;


namespace AntiOthello
{
    class Startup
    {
        static void Main(string[] args)
        {
            GameProcessor gameProcessor = new GameProcessor();
            //controller
            ConsoleOutput ConsoleOutput = new ConsoleOutput();

            //view
            ConsoleInput ConsoleInput = new ConsoleInput(gameProcessor);



            ConsoleOutput.ListenTo(gameProcessor);
            ConsoleInput.StartGame();
        }
    }
}
