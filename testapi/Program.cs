using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace testapi
{
    internal class Program
    {
        internal class Visiteur
        {
            private string _MinCP;
            public string Id { get; set; }
            public string Nom { get; set; }
            public string Prenom { get; set; }
            public string Adresse { get; set; }
            public string Cp { get; set; }
            public string Ville { get; set; }
            public string IDAPI { get; set; }
            public string MinCP
            {
                get
                {
                    string PremierCp = Cp[0] + "";
                    string DeuxCP = Cp[1] + "";
                    return PremierCp + DeuxCP;
                }
                set => _MinCP = value;
            }
            public override string ToString() => $"{Nom.ToUpper()}\t{Prenom}\n{Adresse}\n{Cp}\t{Ville}\n";
        }
        public static List<string> RecupCP( Visiteur visiteur, List<string> vs )
        {
            if (!vs.Contains(visiteur.MinCP))
            {
                vs.Add(visiteur.MinCP);
                vs.Sort();
            }
            return vs;
        }

        private static void Main( string[] args )
        {

            HttpWebRequest webRequest = WebRequest.Create("https://bridge.buddyweb.fr/api/visiteurgsbfrais/visiteur") as HttpWebRequest;
            List<string> ListCP = new List<string>();
            Dictionary<string, Dictionary<string, Visiteur>> DicoFinal = new Dictionary<string, Dictionary<string, Visiteur>>();
            if (webRequest == null)
            {
                return;
            }

            webRequest.ContentType = "application/json";
            webRequest.UserAgent = "Nothing";

            using (Stream s = webRequest.GetResponse().GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    string contributorsAsJson = sr.ReadToEnd();
                    List<Visiteur> contributors = JsonConvert.DeserializeObject<List<Visiteur>>(contributorsAsJson);
                    foreach (Visiteur visiteur in contributors)
                    {
                        ListCP = RecupCP(visiteur, ListCP);
                    }

                    foreach (string item in ListCP)
                    {
                        Dictionary<string, Visiteur> dictionaryaa = contributors.Where(x => x.MinCP == item).ToDictionary(x => x.IDAPI);
                        DicoFinal.Add(item, dictionaryaa);
                    }

                    foreach (KeyValuePair<string, Dictionary<string, Visiteur>> item in DicoFinal)
                    {
                        Console.WriteLine("Pour le département " + item.Key);
                        foreach (KeyValuePair<string, Visiteur> item2 in item.Value)
                        {
                            Console.WriteLine(item2.Value);
                        }
                        Console.ReadLine();
                    }
                }
                Console.ReadLine();
            }
        }
    }
}
