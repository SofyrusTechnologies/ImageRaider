using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageRaider.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> imageUrlsToSearch = new List<string>();
            ImageSearch search = new ImageSearch();
            search.ApiKey = "7cb5d41ac8b4d8fd293672d8731cab6f";
            search.Expires = 1200;
            search.UID = 6569;
            imageUrlsToSearch.Add("http://54.69.197.64/images/55b2316cb48f31.jpg");
            //imageUrlsToSearch.Add("http://animalia-life.com/data_images/dog/dog1.jpg");
            search.Images = imageUrlsToSearch;

            var searchTask = Task.Factory.StartNew(() => {

                var searchResult = search.SearchImages().Result;
                foreach(var result in searchResult)
                {
                    System.Console.WriteLine("Domain - {0}", result.Key);
                    foreach (var page in result.Value.pages)
                    {
                        foreach(var image in page.Images)
                        {
                            System.Console.WriteLine("Image detail- {0} - (height X width) {1} {2}", image.usageimage,image.usageheight,image.usagewidth);
                        }

                    }
                }

            });
            searchTask.Wait();
            System.Console.ReadLine();
        }
    }
}
