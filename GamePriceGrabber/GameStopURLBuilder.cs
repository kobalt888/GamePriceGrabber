using MinimumEditDistance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePriceGrabber
{
    class GameStopURLBuilder
    {

        public string searchString;
        private string consoleParam;
        private string[] queryComponents;
        private string baseUrl = "http://www.gamestop.com/browse/games";
        //Declared vars to use between functions

        public GameStopURLBuilder(string stringToSearch)
        {
            this.searchString = stringToSearch;
            consoleParam = getConsole();
            formatInput();
            
        }

        private string getConsole()
        {
            queryComponents = searchString.Split('-');
            queryComponents[0] = queryComponents[0].Replace(':', ' ');
            queryComponents[0] = queryComponents[0].Replace('(', ' ');
            queryComponents[0] = queryComponents[0].Replace(')', ' ');
            queryComponents[0] = queryComponents[0].Replace('\'', ' ');
            queryComponents[0] = queryComponents[0].Replace(' ', '+');

            

            string consoleData = queryComponents[queryComponents.Length - 1];
            if (Levenshtein.CalculateDistance(consoleData, "Xbox One", 1) < 3)
            {
                return "/xbox-one?";
            }
            else if ((Levenshtein.CalculateDistance(consoleData, "Xbox 360", 1) < 3))
            {
                return "/xbox-360?";
            }
            else if ((Levenshtein.CalculateDistance(consoleData, "Playstation 3", 1) < 3))
            {
                return "/playstation-3?";
            }
            else if ((Levenshtein.CalculateDistance(consoleData, "Playstation 4", 1) < 3) || (Levenshtein.CalculateDistance(consoleData, "PS4", 1) < 3))
            {
                return "/playstation-4?";
            }
            else if ((Levenshtein.CalculateDistance(consoleData, "Gamecube", 1) < 3))
            {
                return "/game-cube?";
            }
            else if ((Levenshtein.CalculateDistance(consoleData, "Nintendo DS", 1) < 3))
            {
                return "/nintendo-ds?";
            }
            else if ((Levenshtein.CalculateDistance(consoleData, "Nintendo 3DS", 1) < 3))
            {
                return "/nintendo-3ds?";
            }
            else
            {
                Nullable<int> closest=null;
                string closestConsole = "";
                Tuple<string, Nullable<int>>[] levData =
                    { new Tuple<string, Nullable<int>>("/xbox-one?", (Levenshtein.CalculateDistance(queryComponents[0], "Xbox One", 2))),
                      new Tuple<string, Nullable<int>>("/xbox-360?", (Levenshtein.CalculateDistance(queryComponents[0], "Xbox 360", 2))),
                      new Tuple<string, Nullable<int>>("/playstation-3?", (Levenshtein.CalculateDistance(queryComponents[0], "Playstation 3", 2))),
                      new Tuple<string, Nullable<int>>("/playstation-4?", (Levenshtein.CalculateDistance(queryComponents[0], "Playstation 4", 2))),
                      new Tuple<string, Nullable<int>>("/playstation-4?", (Levenshtein.CalculateDistance(queryComponents[0], "PS4", 2))),
                      new Tuple<string, Nullable<int>>("/game-cube?", (Levenshtein.CalculateDistance(queryComponents[0], "Gamecube", 2))),
                      new Tuple<string, Nullable<int>>("/nintendo-ds?", (Levenshtein.CalculateDistance(queryComponents[0], "Nintendo DS", 2))),
                      new Tuple<string, Nullable<int>>("/nintendo-3ds?", (Levenshtein.CalculateDistance(queryComponents[0], "Nintendo 3DS", 2)))
                    };


                foreach( var LevScore in levData)
                {
                    if(closest==null)
                    {
                        closest = LevScore.Item2;
                    }
                    if(LevScore.Item2<closest)
                    {
                        closest = Convert.ToInt32(LevScore.Item2);
                        closestConsole = LevScore.Item1;
                    }
                }
                return closestConsole;
            }

        }

        public void formatInput()
        {
            //int count = 0;

            /*foreach(string x in queryComponents)
            {
                queryComponents[count] = x.Replace(':',' ');
                queryComponents[count]= queryComponents[count].Replace(' ', '+');
                baseUrl += queryComponents[count];
                count++;
            }*/
            consoleParam = getConsole() + "nav=16k-3-";
            baseUrl += consoleParam + queryComponents[0];

            if (getConsole() == "/playstation-4?")
            {
                baseUrl += ",1350-17e-1dc-ffff2418";
            }

            else if (getConsole() == "/xbox-one?")
            {
                baseUrl += ",131e0-50-ffff2418";
            }

            else if (getConsole() == "/playstation-3?")
            {
                baseUrl += ",138d-50-ffff2418";
            }

            else if (getConsole() == "/xbox-360?")
            {
                baseUrl += ",1385-50-ffff2418";
            }
            else
            {
                baseUrl += ",-ffff2418";
            }

            searchString = baseUrl;
        }
    }
}