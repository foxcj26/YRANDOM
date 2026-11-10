using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using ClassIsland.YRAND.Models;

namespace ClassIsland.YRAND.Services;

public static class YrandRosterService
{
	public static List<string> Parse(string? text, bool mergeDuplicates)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return new List<string>();
		}
		IEnumerable<string> source = from x in text.Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			where x.Length > 0 && !x.StartsWith('#')
			select x;
		if (mergeDuplicates)
		{
			source = source.Distinct<string>(StringComparer.Ordinal);
		}
		return source.ToList();
	}

	public static string? Pick(YrandSettings settings, IReadOnlyList<string> names)
	{
		if (names.Count == 0)
		{
			return null;
		}
		if (names.Count == 1)
		{
			settings.LastPicked = names[0];
			return names[0];
		}
		List<string> list = BuildCandidates(settings, names);
		string text = list[RandomNumberGenerator.GetInt32(list.Count)];
		if (settings.NonRepeating)
		{
			settings.DrawnThisRound.Add(text);
		}
		settings.LastPicked = text;
		return text;
	}

	private static List<string> BuildCandidates(YrandSettings settings, IReadOnlyList<string> names)
	{
		YrandSettings settings2 = settings;
		if (settings2.NonRepeating)
		{
			List<string> list = names.Where((string x) => !settings2.DrawnThisRound.Contains<string>(x, StringComparer.Ordinal)).ToList();
			if (list.Count == 0)
			{
				settings2.DrawnThisRound.Clear();
				list = new List<string>(names);
			}
			if (list.Count > 1)
			{
				string last = settings2.LastPicked;
				if (last != null)
				{
					list.RemoveAll((string x) => string.Equals(x, last, StringComparison.Ordinal));
				}
			}
			return list;
		}
		List<string> list2 = new List<string>(names);
		string previous = settings2.LastPicked;
		if (previous != null)
		{
			list2.RemoveAll((string x) => string.Equals(x, previous, StringComparison.Ordinal));
		}
		if (list2.Count <= 0)
		{
			return new List<string>(names);
		}
		return list2;
	}

	public static void ResetRound(YrandSettings settings)
	{
		settings.DrawnThisRound.Clear();
	}
}
