using System;
using System.Collections.Generic;

namespace Airport
{
	class TableManager
	{
		public int TableHeight { get; set; }

		public int RowsAmount
		{
			get
			{
				return _rowAmount;
			}
			set
			{
				_rowAmount = value;
				TableHeight = _rowAmount + 5;
			}
		}
		public DateTime StartTime;
		private int _rowAmount;
		public Flight[] Arrivals { get; set; }
		public Flight[] Departures { get; set; }

		private int _tableWidth;
		public TableManager(Flight[] arrivals, Flight[] departures, int rowsAmont)
		{
			Arrivals = arrivals;
			Departures = departures;
			RowsAmount = rowsAmont;
			_tableWidth = new Flight().ToString().Length;
			StartTime = DateTime.Now;
		}

		#region Search methods
		public bool SearchByNumber(Flight[] srcArr, int number, out Flight flight)
		{
			var success = false;
			flight = new Flight();
			foreach (var fl in srcArr)
			{
				if (fl.Number == number)
				{
					flight = fl;
					return !success;
				}
			}
			return success;
		}

		public Flight[] SearchByCity(Flight[] srcArr, string city)
		{
			city = city.Trim().ToLower();
			var flights = new List<Flight>();
			foreach (var fl in srcArr)
			{
				if (fl.City.ToLower() == city)
					flights.Add(fl);
			}
			return (flights.Count == 0) ? null : flights.ToArray();
		}

		public Flight[] SearchByTime(Flight[] srcArr, DateTime start, DateTime end)
		{
			var flights = new List<Flight>();
			foreach (var fl in srcArr)
			{
				if (fl.Time >= start && fl.Time <= end)
					flights.Add(fl);
			}
			return (flights.Count == 0) ? null : flights.ToArray();
		}

		public bool FindFlightByNumber(int number, out int index, out Flight[] srcArray)
		{
			index = -1;
			srcArray = null;
			index = FindFlightByNumber(number, Arrivals);
			if (index > 0)
			{
				srcArray = Arrivals;
				return true;
			}
			index = FindFlightByNumber(number, Departures);
			if (index > 0)
			{
				srcArray = Departures;
				return true;
			}
			return false;
		}

		private int FindFlightByNumber(int number, Flight[] arr)
		{
			var index = 0;
			for (int i = 0; i < arr.Length; i++)
			{
				if (arr[i].Number == number)
				{
					index = i;
					break;
				}
			}
			return index;
		}
		#endregion


		public void PrintTables(CursorPosition startPos, DateTime startDate)
		{
			PrintTables(startPos, GetFlights(Arrivals, startDate), GetFlights(Departures, startDate));
		}
		public void PrintTables(CursorPosition startPos, Flight[] arrivals, Flight[] departures)
		{
			PrintTable("Arrivals", arrivals, 0, startPos);
			PrintTable("Departures", departures, 0, new CursorPosition(startPos.Left + _tableWidth + 10, startPos.Top));
		}
		public void PrintTable(string name, Flight[] flights, int startIndex, CursorPosition startPos)
		{
			if (flights == null)
				return;
			if (startIndex >= flights.Length || startIndex < 0)
				throw new IndexOutOfRangeException();

			var endIndex = (flights.Length < startIndex + RowsAmount) ? flights.Length : startIndex + RowsAmount;
			Console.SetCursorPosition(startPos.Left, startPos.Top);
			Console.Write($"   {name}");
			Console.SetCursorPosition(startPos.Left, ++startPos.Top);
			Console.Write('|' + new string('-', _tableWidth - 1) + '|');
			Console.SetCursorPosition(startPos.Left, ++startPos.Top);
			Console.Write($"| {"Time",-5} | {"Num",-3} | {"City",-14} | {"Term",-4} | {"Status", -10} |");
			Console.SetCursorPosition(startPos.Left, ++startPos.Top);
			Console.Write('|' + new string('-', _tableWidth - 1) + '|');
			for (int i = startIndex; i < endIndex; i++)
			{
				Console.SetCursorPosition(startPos.Left, ++startPos.Top);
				flights[i].Print();
			}
			Console.SetCursorPosition(startPos.Left, ++startPos.Top);
			Console.Write('|' + new string('-', _tableWidth - 1) + '|');
		}

		private Flight[] GetFlights(Flight[] srcArr, DateTime startTime)
		{
			var flights = new List<Flight>();
			foreach (var flight in srcArr)
			{
				if (flight.Time >= startTime)
					flights.Add(flight);
			}

			return flights.ToArray();
		}
	}
	struct CursorPosition
	{
		public int Left { get; set; }
		public int Top { get; set; }

		public CursorPosition(int left, int top)
		{
			Left = left;
			Top = top;
		}
	}

}
