using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePriceGrabber
{
    class Program
    {
        static void Main(string[] args)
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine("Enter a UPC:");
                string UPC = Console.ReadLine();
                UPCLookup lookup = new UPCLookup(UPC);
                string gameTitle = lookup.getGameTitle();
                if (gameTitle == null)
                {
                    
                }
                else
                {
                    GameStopSearcher gsSearcher = new GameStopSearcher(gameTitle);
                    gsSearcher.getGameData();
                }
                
            }
            
            
        }
    }
}
