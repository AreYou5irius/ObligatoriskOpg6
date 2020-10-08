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
    {   //statisk count der holder styr på klient nr.
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

            //her laves vores socket med IP adresse og Port nr, der skal "lytte" efter requests
            TcpListener socket = new TcpListener(localAddress, port);

            //her starter vi vores socket
            socket.Start();

            Console.WriteLine("waiting for connection..");

            //vores socket acceptere tcp clients og at der bliver lavet en ny task 
            //når en ny client opretter forbindelse til vores socket på IP og port nr.

            //her siger vi så længe while condition er true så køres loopet. 
            //i loopet venter socket på at en client forbindes
            //en task bliver oprettet som kører funktionen DoClient
            while (true)
            {

                TcpClient client = socket.AcceptTcpClient();
                Console.WriteLine("Connected");

                Task.Run(() => { DoClient(client, clientNr++); });

            }

        }


        public static void DoClient(TcpClient client, int clientNr)
        {
            int nr = clientNr;

            //her laves et stream objekt der tilladere at skrive til client og læse fra client
            NetworkStream stream = client.GetStream();

            StreamReader sr = new StreamReader(stream);
            StreamWriter sw = new StreamWriter(stream);
            sw.AutoFlush = true;

            //Her kan klienten skrive hvilken forspørgelse de ønsker og få svar retur
            while (true)
            {
                Console.WriteLine($"Velkommen til Klient: {nr}. Hvilken Request ønsker du at bruge? HentAlle, Hent eller Gem");
                string message = sr.ReadLine();

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

                if (message.ToLower().Contains("luk"))
                {
                    Server.clientNr--;
                    break;
                }

            }

            stream.Close();
            client.Close();
            Console.WriteLine("connection to client closed");

        }
    }
}
