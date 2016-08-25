using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Airport
{
	class FlightsGenerator
	{
		private string[] _cities;
		private string[] _airlines;
		public static int[] Terminals = { 5, 7, 12, 6, 9, 11, 1, 3 };
		private readonly DateTime _currentTime;
		private Random _rand = new Random();
		public FlightsGenerator()
		{
			LoadData();
			_currentTime = DateTime.Now;
		}

		public Flight[] GenerateArrivals()
		{
			var arrivals = GenerateFlights(false);
			var randVal = 0;
			for (var i = 0; i < arrivals.Length; i++)
			{
				randVal = _rand.Next(1, 101);
				if (randVal <= 10)
				{
					arrivals[i].FlightStatus = FlightStatus.Canceled;
					continue;
				}
				if (_currentTime < arrivals[i].Time && _currentTime.AddMinutes(10) > arrivals[i].Time)
				{
					arrivals[i].FlightStatus = FlightStatus.Landing;
					continue;
				}
				if (_currentTime < arrivals[i].Time)
					arrivals[i].FlightStatus = (randVal <= 20) ? FlightStatus.Delayed : FlightStatus.OnTime;
				else
					arrivals[i].FlightStatus = FlightStatus.Arrived;
			}
			return arrivals;
		}

		public Flight[] GenerateDepartures()
		{
			var departures = GenerateFlights(true);
			var randVal = 0;
			for (var i = 0; i < departures.Length; i++)
			{
				randVal = _rand.Next(1, 101);
				if (randVal <= 10)
				{
					departures[i].FlightStatus = FlightStatus.Canceled;
					continue;
				}
				if(_currentTime > departures[i].Time.AddMinutes(-20) && _currentTime < departures[i].Time)
					departures[i].FlightStatus = (randVal <= 15) ? FlightStatus.Delayed : FlightStatus.Boarding;
				else if(_currentTime > departures[i].Time)
					departures[i].FlightStatus = FlightStatus.Departed;
				else
					departures[i].FlightStatus = FlightStatus.OnTime;
			}

			return departures;
		}

		private Flight[] GenerateFlights(bool isDepartures)
		{
			if (isDepartures)
			{
				new Random().Shuffle(_cities);
			}

			var flights = new Flight[24 * 2];
			var time = _currentTime;
			time = time.AddHours(-12);
			for (int i = 0; i < flights.Length; i++)
			{
				flights[i] = new Flight()
				{
					Airline = GetNext(_airlines, i),
					City = GetNext(_cities, i),
					Terminal = GetNext(Terminals, i),
					Number = (isDepartures) ? DateTime.Now.Minute + i + 50 : DateTime.Now.Minute + i,
					Time = time
				};
				time = (isDepartures)? time.AddMinutes(22) : time.AddMinutes(27);
			}

			return flights;
		}

		private void LoadData()
		{
			try
			{
				using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(ConfigurationManager.AppSettings["airlines"])))
				{
					_airlines = reader.ReadToEnd().Split(';');

				}
				using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(ConfigurationManager.AppSettings["cities"])))
				{
					_cities = reader.ReadToEnd().Split(';');
				}
			}
			catch (Exception e)
			{
				Console.WriteLine($"Load data ERROR! Info: {e.Message}");
			}
		}

		private T GetNext<T>(T[] arr, int index)
		{
			if (index >= arr.Length)
				index = index - (index / arr.Length) * arr.Length;

			return arr[index];
		}
	}

	static class RandomExtensions
	{
		public static void Shuffle<T>(this Random rng, T[] array)
		{
			int n = array.Length;
			while (n > 1)
			{
				int k = rng.Next(n--);
				T temp = array[n];
				array[n] = array[k];
				array[k] = temp;
			}
		}
	}
}
