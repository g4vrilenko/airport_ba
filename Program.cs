using System;

namespace Airport
{
	class Program
	{
		public static TableManager TableMngr { get; set; }
		public static CursorPosition TablePosition;
		public static CursorPosition MenuPosition;
		public static int MenuOffset = 1;
		public static CursorPosition ErrorMessagePos;
		public static CursorPosition InputPos;
		static void Main(string[] args)
		{
			SetupCosole();
			var fg = new FlightsGenerator();
			TableMngr = new TableManager(fg.GenerateArrivals(), fg.GenerateDepartures(), 16);
			TablePosition = new CursorPosition(5, 0);
			ErrorMessagePos = new CursorPosition(TablePosition.Left, TablePosition.Top + TableMngr.TableHeight + MenuOffset);
			MenuPosition = new CursorPosition(ErrorMessagePos.Left, ErrorMessagePos.Top + 2);
			TableMngr.PrintTables(TablePosition, DateTime.Now.AddHours(-1));
			MainMenu();
		}

		#region Menu
		private static void MainMenu()
		{
			var menuItems = new string[] { "Search", "Edit", "Reset tables" };
			PrintMenu("Main menu", MenuPosition, menuItems);
			var key = Console.ReadKey(true);
			ClearInputError();
			switch (key.Key)
			{
				case ConsoleKey.D1://to search menu
					SearchMenu();
					break;
				case ConsoleKey.NumPad1://to search menu
					SearchMenu();
					break;
				case ConsoleKey.NumPad2://Edit
					EditMenu();
					break;
				case ConsoleKey.NumPad3://reset table
					ResetTables();
					MainMenu();
					break;
				case ConsoleKey.Escape:
					return;
				default:
					DisplayInputError("Input Error! -> Not a command! Try again)");
					MainMenu();
					break;
			}
		}

		private static void SearchMenu()
		{
			var menuItems = new string[] { "By number", "By city", "By time", "Back to main menu", "Reset tables"  };
			PrintMenu("Search menu", MenuPosition, menuItems);
			var keyInfo = Console.ReadKey(true);
			ClearInputError();
			switch (keyInfo.Key)
			{
				case ConsoleKey.D1://search by number
					SearchByNumber();
					SearchMenu();
					break;
				case ConsoleKey.NumPad1://search by number
					SearchByNumber();
					SearchMenu();
					break;
				case ConsoleKey.NumPad2://search by city
					SearchByCity();
					SearchMenu();
					break;
				case ConsoleKey.NumPad3://search by time
					SearchByTime();
					SearchMenu();
					break;
				case ConsoleKey.NumPad4://Back to main menu
					MainMenu();
					break;
				case ConsoleKey.NumPad5://Reset tables
					ResetTables();
					SearchMenu();
					break;
				case ConsoleKey.Escape:
					return;
				default:
					DisplayInputError("Input Error! -> Not a command! Try again)");
					SearchMenu();
					break;
			}

		}

		private static void EditMenu()
		{
			var menuItems = new string[] { "Update city", "Update terminal", "Cancel flight", "Back to main menu", "Reset tables" };
			PrintMenu("Edit menu", MenuPosition, menuItems);
			var keyInfo = Console.ReadKey(true);
			ClearInputError();
			switch (keyInfo.Key)
			{
				case ConsoleKey.NumPad1://Update city
					UpdateCity();
					EditMenu();
					break;
				case ConsoleKey.NumPad2://Update terminal
					UpdateTerminal();
					EditMenu();
					break;
				case ConsoleKey.NumPad3://Cancel flight
					CancelFlight();
					EditMenu();
					break;
				case ConsoleKey.NumPad4://Back to main menu
					MainMenu();
					break;
				case ConsoleKey.NumPad5://Reset tables
					ResetTables();
					EditMenu();
					break;
				case ConsoleKey.Escape:
					return;
				default:
					DisplayInputError("Input Error! -> Not a command! Try again)");
					SearchMenu();
					break;
			}
		}
		private static void PrintMenu(string menuName, CursorPosition startPos, string[] menuItems)
		{
			ClearConsole(startPos.Top, 25);
			PrintMessage($"{menuName}: \n", new CursorPosition(startPos.Left, startPos.Top), ConsoleColor.Blue);
			PrintMessage($"{new string('-', 10)}", new CursorPosition(startPos.Left, startPos.Top + 1));
			for (int i = 0; i < menuItems.Length; i++)
				PrintMessage($"\t{i + 1}. {menuItems[i]}", new CursorPosition(startPos.Left, startPos.Top + 2 + i));

			Console.Write("\n\t");
			InputPos = new CursorPosition(startPos.Left, startPos.Top + menuItems.Length + 2);
		}

		static void ResetTables()
		{
			TableMngr.PrintTables(TablePosition, DateTime.Now.AddHours(-1));
		}
		#endregion

		#region Edit functionality
		static int EditHelper()
		{
			var number = 0;
			Console.Write("\n\tEnter flight number:\n\t");
			if (!int.TryParse(Console.ReadLine(), out number))
			{
				DisplayInputError("Input Error! -> Not a number. Try again)");
				EditMenu();
				return 0;
			}
			return number;
		}
		static void UpdateCity()
		{
			var flightNumber =  EditHelper();
			int index;
			Flight[] srcArray;
			if (!TableMngr.FindFlightByNumber(flightNumber, out index, out srcArray))
			{
				DisplayInputError($"No flights with specified number ({flightNumber}) has been found.");
				EditMenu();
			}
			Console.Write("\n\tEnter new city:\n\t");
			var city = Console.ReadLine();
			if (city.Length > 14)
				city = city.Substring(0, 14);
			srcArray[index].City = city;
			ResetTables();
		}

		static void UpdateTerminal()
		{
			var flightNumber = EditHelper();
			int index;
			Flight[] srcArray;
			if (!TableMngr.FindFlightByNumber(flightNumber, out index, out srcArray))
			{
				DisplayInputError($"No flights with specified number ({flightNumber}) has been found.");
				EditMenu();
			}
			Console.Write("\n\tEnter new terminal number (availables: ");
			for (int i = 0; i < FlightsGenerator.Terminals.Length; i++)
				Console.Write($"{FlightsGenerator.Terminals[i]} ");
			Console.Write("):\n\t");

			int terminal;
			if (!int.TryParse(Console.ReadLine(), out terminal))
			{
				DisplayInputError("Input Error! -> Not a number. Try again)");
				EditMenu();
				return;
			}
			if(Array.IndexOf(FlightsGenerator.Terminals, terminal) < 0)
			{
				DisplayInputError($"Input Error! -> Terminal {terminal} don't exist. Try again)");
				EditMenu();
				return;
			}
			srcArray[index].Terminal = terminal;
			ResetTables();
		}

		static void CancelFlight()
		{
			var flightNumber = EditHelper();
			int index;
			Flight[] srcArray;
			if (!TableMngr.FindFlightByNumber(flightNumber, out index, out srcArray))
			{
				DisplayInputError($"No flights with specified number ({flightNumber}) has been found.");
				EditMenu();
			}
			srcArray[index].FlightStatus = FlightStatus.Canceled;
			ResetTables();
		}

		#endregion

		#region Search functionality
		static void SearchByNumber()
		{
			Console.Write("\n\tEnter flight number:\n\t");
			var number = 0;
			var line = Console.ReadLine();
			if (!int.TryParse(line, out number))
				DisplayInputError("Input Error! -> Not a number. Try again)");

			var arrivels = new Flight[1];
			var departures = new Flight[1];
			var arrivelsExist = true;
			var departuresExist = true;
			if (!TableMngr.SearchByNumber(TableMngr.Arrivals, number, out arrivels[0]))
			{
				arrivels = null;
				arrivelsExist = false;
			}
			if (!TableMngr.SearchByNumber(TableMngr.Departures, number, out departures[0]))
			{
				departures = null;
				departuresExist = false;
			}
			if (!arrivelsExist && !departuresExist)
				DisplayInputError($"No flights with specified number ({number}) has been found.");
			else
			{
				ClearConsole(TablePosition.Top, TableMngr.TableHeight);
				TableMngr.PrintTables(TablePosition, arrivels, departures);
			}
		}

		static void SearchByCity()
		{
			Console.Write("\n\tEnter city:\n\t");
			var city = Console.ReadLine();
			var arrivels = TableMngr.SearchByCity(TableMngr.Arrivals, city);
			var departures = TableMngr.SearchByCity(TableMngr.Departures, city);
			if (arrivels == null && departures == null)
				DisplayInputError($"No flights with specified city ({city}) has been found.");
			else
			{
				ClearConsole(TablePosition.Top, TableMngr.TableHeight);
				TableMngr.PrintTables(TablePosition, arrivels, departures);
			}
		}

		static void SearchByTime()
		{
			Console.Write("\n\tEnter time (Example: 14:50)\n\t");
			var line = Console.ReadLine();
			DateTime dt;
			if (!DateTime.TryParse(line, out dt))
			{
				DisplayInputError("Input Error! -> Time string not valid! Try again)");
				SearchMenu();
			}
			var arrivels = TableMngr.SearchByTime(TableMngr.Arrivals, dt, dt.AddHours(1));
			var departures = TableMngr.SearchByTime(TableMngr.Departures, dt, dt.AddHours(1));
			if (arrivels == null && departures == null)
				DisplayInputError($"No flights in specified time ({dt.ToString("t")}) has been found.");
			else
			{
				ClearConsole(TablePosition.Top, TableMngr.TableHeight);
				TableMngr.PrintTables(TablePosition, arrivels, departures);
			}
		}

		#endregion

		#region Helper methods
		private static void DisplayInputError(string message)
		{
			PrintMessage(message, ErrorMessagePos, ConsoleColor.Red);
		}

		private static void ClearInputError()
		{
			ClearConsole(ErrorMessagePos.Top, 1);
			Console.SetCursorPosition(InputPos.Left, InputPos.Top);
		}

		static void ClearConsole(int startLine, int lineCount)
		{
			var endLine = startLine + lineCount;
			if (startLine > Console.BufferHeight)
				return;
			if (endLine > Console.BufferHeight)
				endLine = Console.BufferHeight;

			for (int i = startLine; i < endLine; i++)
			{
				Console.SetCursorPosition(0, i);
				Console.Write(new string(' ', Console.BufferWidth));
			}
		}

		static void PrintMessage(string message, CursorPosition pos, ConsoleColor foregroundColor = ConsoleColor.Black, ConsoleColor backgroundColor = ConsoleColor.White)
		{
			var currentBeckgroundColor = Console.BackgroundColor;
			var currentForegrounfColor = Console.ForegroundColor;
			Console.SetCursorPosition(pos.Left, pos.Top);
			Console.BackgroundColor = backgroundColor;
			Console.ForegroundColor = foregroundColor;
			Console.Write(message);
			Console.BackgroundColor = currentBeckgroundColor;
			Console.ForegroundColor = currentForegrounfColor;
		}
		static void SetupCosole()
		{
			Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
			Console.BackgroundColor = ConsoleColor.White;
			Console.Clear();
			Console.ForegroundColor = ConsoleColor.Black;
		}
		#endregion
	}
}
