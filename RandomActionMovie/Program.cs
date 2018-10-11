using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RandomActionMovie
{
	class Program
	{
		static Random random = new Random();
		const string baseUrl = "https://en.wikipedia.org/wiki/List_of_action_films_of_the_19";
		const string url70s = baseUrl + "70s";
		const string url80s = baseUrl + "80s";
		const string url90s = baseUrl + "90s";

		static void Main(string[] args)
		{
			var names = new List<HtmlAgilityPack.HtmlNodeCollection>();
			if (Confirm("Do you want to include 70s movies?", "Including 70s movies", "Not including 70s movies"))
			{
				names.AddRange(GetNames(url70s));
			}
			if (Confirm("Do you want to include 80s movies?", "Including 80s movies", "Not including 80s movies"))
			{
				names.AddRange(GetNames(url80s));
			}
			if (Confirm("Do you want to include 90s movies?", "Including 90s movies", "Not including 90s movies"))
			{
				names.AddRange(GetNames(url90s));
			}

			if (names.Count() == 0)
			{
				Console.WriteLine("Not showing any movies, press any key to exit");
				Console.ReadKey();
			}
			else
			{
				bool next = true;
				Console.WriteLine("The first movie is:");
				while (next)
				{
					int randomMovieIndex = random.Next(0, names.Count);
					string name = names.ElementAt(randomMovieIndex)[1].InnerText;

					while (IsYear(name) || name == "Title")
					{
						randomMovieIndex = random.Next(0, names.Count);
						name = names.ElementAt(randomMovieIndex)[1].InnerText;
					}

					string year = GetYear(names, randomMovieIndex);
					Console.WriteLine($"{name} ({year})");
					next = Confirm("Do you want another movie?", "The next movie is: ", "Ok, exiting");
				}
			}
		}

		private static string GetYear(IEnumerable<HtmlAgilityPack.HtmlNodeCollection> movies, int movieIndex)
		{
			string year = string.Empty;
			for (int i = movieIndex; i >= 0; i--)
			{
				string text = movies.ElementAt(i)[1].InnerText;
				if (IsYear(text))
				{
					year = text.Substring(0, 4);
					break;
				}
			}
			return year;
		}

		private static bool IsYear(string text)
		{
			Regex yearRegex = new Regex(@"^(19)[0-9]{2}\n?$");
			return yearRegex.IsMatch(text);
		}

		private static List<HtmlAgilityPack.HtmlNodeCollection> GetNames(string wikipediaUrl)
		{
			var web = new HtmlAgilityPack.HtmlWeb();
			var doc = web.Load(wikipediaUrl);
			var table = doc.DocumentNode.SelectNodes("/html/body/div[3]/div[3]/div[4]/div/table[2]/tbody").Descendants(); ;
			var tableRows = table.Where(d => d.Name == "tr");
			return tableRows.Select(t => t.ChildNodes).ToList();
		}

		private static bool Confirm(string question, string positiveAnswer, string negativeAnswer)
		{
			bool confirm()
			{
				Console.Write(question + " (y/n): ");
				switch (Console.ReadLine().ToLower())
				{
					case "y":
						return true;
					case "n":
						return false;
					default:
						return confirm();
				}
			}
			bool confirmed = confirm();
			Console.WriteLine(confirmed ? positiveAnswer : negativeAnswer);
			return confirmed;
		}
	}
}