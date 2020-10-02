using System;

namespace FanOutputLibrary
{
    public class FanOutput
    {
        private string _navn;
        private double _temp;
        private double _fugt;
        private int _id;

        public int Id
        {
            get => _id;
            set
            {
                CheckId(value);
                _id = value;
            }
        }

        public string Navn
        {
            get => _navn;
            set
            {
                CheckNavn(value);
                _navn = value;
            }
        }

        public double Temp
        {
            get => _temp;
            set
            {
                CheckTemp(value);
                _temp = value;
            }
        }

        public double Fugt
        {
            get => _fugt;
            set
            {
                CheckFugt(value);
                _fugt = value;
            }
        }

        public FanOutput(int id, string navn, double temp, double fugt)
        {
            Id = id;
            Navn = navn;
            Temp = temp;
            Fugt = fugt;
        }

        public void CheckId(int id)
        {
            if (id < 1)
            {
                throw new ArgumentException("Id skal være et positivt tal");
            }
        }

        public void CheckNavn(string navn)
        {
            if (navn.Length < 2)
            {
                throw new ArgumentException("Navn skal være på 2 eller flere bogstaver");
            }
        }


        public void CheckTemp(double temp)
        {
            if (temp < 15)
            {
                throw new ArgumentOutOfRangeException("temp", temp, "Temperaturen er for lav");

            }

            if (temp > 25)
            {
                throw new ArgumentOutOfRangeException("temp", temp, "Temperaturen ErrObject for høj");
            }
        }


        public void CheckFugt(double fugt)
        {
            if (fugt < 30)
            {
                throw new ArgumentOutOfRangeException("fugt", fugt, "for lav fugtighed");

            }

            if (fugt > 80)
            {
                throw new ArgumentOutOfRangeException("fugt", fugt, "for høj fugtighed");
            }
        }



        public override string ToString()
        {
            return $"Id: {Id}, Navn: {Navn}, Temp: {Temp}, Fugt: {Fugt}";
        }
    }
}
