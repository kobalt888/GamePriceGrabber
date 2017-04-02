using HtmlAgilityPack;
using MinimumEditDistance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePriceGrabber
{
    class GameStopSearcher
    {

        private static string X_PATH_GAMECONDITION = "//*[@id='mainContentPlaceHolder_dynamicContent_ctl00_RepeaterResultFoundTemplate_ResultFoundPlaceHolder_1_ctl00_1_ctl00_1_StandardPlaceHolderTop_2_ctl00_2_CartridgeCatalogPanel_2']/div[3]/div[{0}]/div[3]/h4/strong";
        private static string X_PATH_CONSOLE = "//*[@id='mainContentPlaceHolder_dynamicContent_ctl00_RepeaterResultFoundTemplate_ResultFoundPlaceHolder_1_ctl00_1_ctl00_1_StandardPlaceHolderTop_2_ctl00_2_CartridgeCatalogPanel_2']/div[3]/div{0}/div[2]/h3/strong";
        private static string X_PATH_TITLE = "//*[@id='mainContentPlaceHolder_dynamicContent_ctl00_RepeaterResultFoundTemplate_ResultFoundPlaceHolder_1_ctl00_1_ctl00_1_StandardPlaceHolderTop_2_ctl00_2_rptResults_2_res_0_hypTitle_{0}']";
        private static string X_PATH_PRICE = "//*[@id='mainContentPlaceHolder_dynamicContent_ctl00_RepeaterResultFoundTemplate_ResultFoundPlaceHolder_1_ctl00_1_ctl00_1_StandardPlaceHolderTop_2_ctl00_2_CartridgeCatalogPanel_2']/div[3]/div[2]/div[3]/p[1]";



        private static string X_PATH_PRICEALT1 = "//*[@id='mainContentPlaceHolder_dynamicContent_ctl00_RepeaterResultFoundTemplate_ResultFoundPlaceHolder_1_ctl00_1_ctl00_1_StandardPlaceHolderTop_3_ctl00_3_CartridgeCatalogPanel_3']/div[3]/div[2]/div[3]/p[1]";
        private static string X_PATH_TITLEALT1 = "//*[@id='mainContentPlaceHolder_dynamicContent_ctl00_RepeaterResultFoundTemplate_ResultFoundPlaceHolder_1_ctl00_1_ctl00_1_StandardPlaceHolderTop_3_ctl00_3_rptResults_3_res_0_hypTitle_0']";


 
        private static string X_PATH_DIRECTTITLE = "//*[@id='mainContentPlaceHolder_dynamicContent_ctl00_RepeaterRightColumnLayouts_RightColumnPlaceHolder_0_ctl00_0_LayoutStandardPanel_0']/div[1]/h1/text()";
        private static string X_PATH_DIRECTPRICE = "//*[@id='mainContentPlaceHolder_dynamicContent_ctl00_RepeaterRightColumnLayouts_RightColumnPlaceHolder_0_ctl00_0_LayoutStandardPanel_0']/div[5]/div[2]/div[2]/div[2]/h3/span";
        //All XPATHS stored above for ease of use



        private string searchQuery;
        //Declared vars to use between functions


        public GameStopSearcher(string inputQuery)
        {
            GameStopURLBuilder builder = new GameStopURLBuilder(inputQuery);
            this.searchQuery = builder.searchString;
            //TODO        
        }

        

        
        public Game getGameData()
        {
            //int distance=Levenshtein.CalculateDistance("Forza 4", "Forza 3", 3);
            //Console.WriteLine("The distance is " + distance);

            Game newGame = new Game();
            HtmlWeb gameStopPage = new HtmlWeb();
            gameStopPage.UserAgent = "'Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.90 Safari/537.36'";
            HtmlDocument gameStopPageDoc = gameStopPage.Load(searchQuery);
            Console.WriteLine("Looking up on gamestop: " + searchQuery);
            var gamePrice =
                    gameStopPageDoc.DocumentNode.SelectSingleNode(X_PATH_DIRECTPRICE);
            var gameTitle =
                    gameStopPageDoc.DocumentNode.SelectSingleNode(X_PATH_DIRECTTITLE);
            Console.WriteLine("Received Gamestop response in {0}ms.", gameStopPage.RequestDuration);


            if (gamePrice == null)
            {
                Console.WriteLine("Did not find direct link, attempting to iterate...");
                //If we get a null response, that means that the link created did not link directly to a product page, eg; a page with multiple results. We now must find 
                //the proper pre-owned game for the console.
                string iteratePathPrice = string.Format(X_PATH_PRICE, 2);
                string iteratePathTitle = string.Format(X_PATH_TITLE, 0);
                string iteratePathConsole = string.Format(X_PATH_CONSOLE, 2);
                string iteratepathGameCondition = string.Format(X_PATH_GAMECONDITION, 2);

                int iterator = 0;

                try
                {
                    gamePrice= gameStopPageDoc.DocumentNode.SelectSingleNode(iteratePathPrice);
                    gameTitle = gameStopPageDoc.DocumentNode.SelectSingleNode(iteratePathTitle);
                    newGame.price = gamePrice.InnerText;
                    newGame.title = gameTitle.InnerText;

                    //This checks if we have a dual price, gamestop will put their old price next to the current sometimes, and we need to have the lower price only.
                    if ((newGame.price.Split('$').Length - 1) > 1)
                    {
                        var seperatedString = newGame.price.Split('$');
                        newGame.price = seperatedString[seperatedString.Length - 1];
                    }
                    Console.WriteLine("\n\nGame:{0} \nPrice:{1}\n\n", newGame.title, newGame.price);
                }
                catch(NullReferenceException ex)
                {
                    //IF we fail to find a link, more than likely its doing some trickery and we will need to use different xPathing to find the elements we want. For some reason, if there is only one result, 
                    //It doesnt work with the iterative xpathing...
                    try
                    {
                        gamePrice = gameStopPageDoc.DocumentNode.SelectSingleNode(X_PATH_PRICEALT1);
                        gameTitle = gameStopPageDoc.DocumentNode.SelectSingleNode(X_PATH_TITLEALT1);
                        newGame.price = gamePrice.InnerText;
                        newGame.title = gameTitle.InnerText;
                        if ((newGame.price.Split('$').Length - 1) > 1)
                        {
                            var seperatedString = newGame.price.Split('$');
                            newGame.price = seperatedString[seperatedString.Length - 1];
                        }
                        Console.WriteLine("\n\nGame:{0} \nPrice:{1}\n\n", newGame.title, newGame.price);
                    }
                    catch
                    {

                        Debug.WriteLine(ex.InnerException);
                        Console.WriteLine("Failed to find game through UPC!");
                    }
                }

            }

            else
            {

                Console.WriteLine("Found direct link.");
                newGame.price = gamePrice.InnerText;
                newGame.title = gameTitle.InnerText;
                
                //This checks if we have a dual price, gamestop will put their old price next to the current sometimes, and we need to have the lower price only.
                if ((newGame.price.Split('$').Length - 1) > 1)
                {
                    var seperatedString = newGame.price.Split('$');
                    newGame.price = seperatedString[seperatedString.Length - 1];
                }
                Console.WriteLine(newGame.price + " " + newGame.title);
                return newGame;
            }

            return newGame;
        }
    }
}
