using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GamePriceGrabber
{
    class UPCLookup
    {

        private string  UPC;
        private string baseUrl = "https://www.amazon.com/s/search-alias%3Daps&field-keywords=";
        public UPCLookup(string aUpc)
        {
            this.UPC = aUpc;
            baseUrl += UPC;//append the UPC to the url to have a callable site 
        }

        
        public string getGameTitle()
        {
            string result = null;
            HtmlWeb upcPage = new HtmlWeb();
            upcPage.UserAgent = "'Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.90 Safari/537.36'";
            HtmlDocument upcPageDoc = upcPage.Load(baseUrl);
            var gameTitleArea =
                    upcPageDoc.DocumentNode.SelectSingleNode("//*[@id='result_0']/div/div/div/div[2]/div[1]/div[1]/a/h2");
                                
            if (gameTitleArea == null)
            {
                Console.WriteLine("Failed to find game with UPC!\n");
            }
            else
            {
                Console.WriteLine("Got game '{0}' from amazon in {1}ms.", gameTitleArea.InnerText, upcPage.RequestDuration);
                return gameTitleArea.InnerText;
            }

            return result;
        }
    }
}
