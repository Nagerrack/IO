using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IO_final
{
    class TResultDataStructure
    {
        public int A { get; set; }
        public int B { get; set; }
    }


    class Zadanie12i13i14
    {
        public static Task<bool> Zadanie13(bool Z2)
        {
            return Task.Run(
                () =>
                {
                    Z2 = true;
                    return Z2;
                });
        }

        public static async Task<XmlDocument> Zadanie14(string url)
        {
            WebClient webClient = new WebClient();
            string xmlContent = await webClient.DownloadStringTaskAsync(new Uri(url));
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlContent);
            return doc;
        }

        public void Start()
        {
            Console.WriteLine(Zadanie14("http://www.feedforall.com/sample.xml").Result.InnerText);
            Console.ReadKey();
        }
    }

  
}
