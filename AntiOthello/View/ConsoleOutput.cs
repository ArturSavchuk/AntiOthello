using System;
using System.Collections.Generic;
using System.Text;
using AntiOthello.Model;

namespace AntiOthello.View
{
    public class ConsoleOutput
    {
        public void ListenTo(GameProcessor gameProcessor)
        { 
            gameProcessor.MovePassed += ShowPassMessage;
        }

        public void ShowPassMessage()
        {
            Console.WriteLine("pass");
        }
    }
}
