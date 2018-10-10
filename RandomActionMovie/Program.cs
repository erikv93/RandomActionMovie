using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RandomActionMovie
{
    class Program
    {
		static Random random = new Random();
		const string url80s = "https://en.wikipedia.org/wiki/List_of_action_films_of_the_1980s";
		const string url90s = "https://en.wikipedia.org/wiki/List_of_action_films_of_the_1990s";

		static void Main(string[] args)
        {
			var names = GetNames(url80s);
			names.AddRange(GetNames(url90s));

			bool next = true;
			while (next)
			{
				int randomMovieIndex = random.Next(0, names.Count);
				string name = names.ElementAt(randomMovieIndex)[1].InnerText;

				while(IsYear(name) || name == "Title")
				{
					randomMovieIndex = random.Next(0, names.Count);
					name = names.ElementAt(randomMovieIndex)[1].InnerText;
				}

				string year = GetYear(names, randomMovieIndex);
				Console.WriteLine($"{name} ({year})\npress Enter for another movie, press any other key to quit.");
				next = Console.ReadKey().Key == ConsoleKey.Enter;
			}
		}

		private static string GetYear(IEnumerable<HtmlAgilityPack.HtmlNodeCollection> movies, int movieIndex)
		{
			string year = string.Empty;
			for(int i = movieIndex; i >= 0; i--)
			{
				string text = movies.ElementAt(i)[1].InnerText;
				if (IsYear(text))
				{
					year = text.Substring(0,4);
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
	}
}