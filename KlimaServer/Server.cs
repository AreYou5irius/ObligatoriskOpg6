using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FanOutputLibrary;
using Newtonsoft.Json;

namespace KlimaServer
{
    class Server
    {
        private static int clientNr = 1;

        //statisk liste med data.
        private static List<FanOutput> data = new List<FanOutput>()
        {
            new FanOutput(1001, "hej", 23, 45),
            new FanOutput(1002, "med", 25, 30)

        };

        public static void Start()
        {
            Int32 port = 4646;
            IPAddress localAddress = IPAddress.Loopback;

            //her laves vores socket der skal "lytte" efter requests med IP adresse og Port nr
            TcpListener socket = new TcpListener(localAddress, port);

            //her starter vi vores socket der siger: at så længe der er minimum en client der er forbunndet, er vores socket tændt
            //vores socket acceptere tcp clients og at der bliver lavet en ny task 
            //når en ny client opretter forbindelse til vores socket på IP og port nr. 

            socket.Start();
            do
            {

                TcpClient client = socket.AcceptTcpClient();

                Console.WriteLine("server activated");


                Task.Run(() => { DoClient(client, clientNr++); });
                if (clientNr < 1)
                {
                    break;
                }

            } while (clientNr > 0);

            socket.Stop();
            Console.WriteLine("server stopped");
        }

        public static void DoClient(TcpClient client, int clientNr)
        {
            int Nr = clientNr;

            //her laves forbindelsen
            NetworkStream stream = client.GetStream();
            //hvor der både kan læses fra clienten og skrives til clienten
            StreamReader sr = new StreamReader(stream);
            StreamWriter sw = new StreamWriter(stream);
            sw.AutoFlush = true;


            while (true)
            {


                Console.WriteLine("Hvilken Request ønsker du at bruge? HentAlle, Hent eller Gem");
                string message = sr.ReadLine();

                if (message.ToLower().Contains("luk"))
                {
                    break;
                }

                switch (message.ToLower())
                {


                    //socketen serialisere listen og initialisere en var variabel
                    //så skriver/sender den variablen til clienten og udskriver til consollen
                    case "hentalle":

                        var json = JsonConvert.SerializeObject(data);
                        sw.WriteLine(json);
                        Console.WriteLine(json);
                        break;



                    // socketen aflæser Id og initialisere en string variabel
                    //den bliver conventeret fra en string til en int
                    //efterfølgende finder socketen listen med matchende id og serialisere listen og initerialiserer en var variable
                    //så skriver/sender den til clienten og udskriver til contollen 
                    case "hent":
                        Console.WriteLine("hvilken måling ønsker du at hente? indtast id: ");
                        string id = sr.ReadLine();
                        int GetId = Int32.Parse(id);
                        var json2 = JsonConvert.SerializeObject(data.Find(i => GetId == i.Id));
                        sw.WriteLine(json2);
                        Console.WriteLine($"dette er dataen serveren har hentet fra listen på Id nr: " + json2);

                        break;

                    //socketen aflæser den data clienten har indtastet og gemmer dem i en ny listen dog hvor den tager hensyn til at conventere til rigtig værditype
                    //bagefter tilføjes den nye liste til vores samling af lister (data)
                    case "gem":
                        Console.WriteLine("indtast: Id, Navn, Temp, Fugt adskilt med mellemrum");
                        string newData = sr.ReadLine();
                        FanOutput input = new FanOutput(Convert.ToInt32(newData.Split(" ")[0]), newData.Split(" ")[1],
                            Convert.ToDouble(newData.Split(" ")[2]), Convert.ToDouble(newData.Split(" ")[3]));

                        data.Add(input);

                        break;

                }

            }

            stream.Close();
            client.Close();
            Console.WriteLine("connection to client closed");

        }
    }
}
