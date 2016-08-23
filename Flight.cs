using System;

namespace Airport
{
	struct Flight
	{
		public DateTime Time { get; set; }
		public int Number { get; set; }
		public string City { get; set; }
		public string Airline { get; set; }
		public int Terminal { get; set; }
		public FlightStatus FlightStatus { get; set; }

		public override string ToString()
		{
			return $"| {Time.ToString(@"t"), -5} | {Number, -3} | {City, -14} | {Terminal, -3} | {FlightStatus, -10} |";
		}

		public void Print()
		{
			Console.Write($"| {Time.ToString(@"t"),-5} | {Number,-3} | {City,-14} | {Terminal,-4} | ");
			var fColor = Console.ForegroundColor;
			if (FlightStatus == FlightStatus.Canceled)
				Console.ForegroundColor = ConsoleColor.Red;
			else if(FlightStatus == FlightStatus.OnTime)
				Console.ForegroundColor = ConsoleColor.Green;
			else if(FlightStatus == FlightStatus.Arrived || FlightStatus == FlightStatus.Departed)
				Console.ForegroundColor = ConsoleColor.Gray;
			else if(FlightStatus == FlightStatus.Delayed)
				Console.ForegroundColor = ConsoleColor.Yellow;
			else if (FlightStatus == FlightStatus.Landing || FlightStatus == FlightStatus.Boarding)
				Console.ForegroundColor = ConsoleColor.DarkGreen;

			Console.Write($"{FlightStatus,-10}");
			Console.ForegroundColor = fColor;
			Console.Write(" |");
		}
	}

	enum FlightStatus
	{
		CheckIn,
		GateClosed,
		Arrived,
		Departed,
		Unknown,
		Canceled,
		Landed,
		Expected,
		OnTime,
		Boarding,
		Delayed,
		Landing
	}
}
