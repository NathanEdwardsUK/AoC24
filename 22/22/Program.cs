using System.Numerics;
using System.Text.RegularExpressions;

Console.WriteLine($"Parsing data");
var sr = new StreamReader("/Users/nathanedwards/Dev/AoC/22/22/input.txt");
string input = sr.ReadToEnd();
string[] initialSecrets = input.Split("\n");


/* 
--------------- Part 1 --------------- 
I'm just going to try solving this in a straight forward way since the number of calculations doesnt seem too great
*/

long mix(long a, long b) => a ^ b;

long prune(long a) => a % 16777216;

long evolve(long _secret)
{
	long _result = _secret * 64;
	_secret = mix(_secret, _result);
	_secret = prune(_secret);
	
	_result = _secret /32;
	_secret = mix(_secret, _result);
	_secret = prune(_secret);
	
	_result = _secret * 2048;
	_secret = mix(_secret, _result);
	_secret = prune(_secret);
	
	return _secret;
}

long solution1 = 0;

foreach (string str in initialSecrets)
{
	long secret = Int64.Parse(str);
	for (int _ = 0; _ < 2000; _++)
	{
		secret = evolve(secret);	
	}
	// Console.WriteLine(str + ": " + sec);
	solution1 += secret;
}

Console.WriteLine("Solution1 = " + solution1);

/* 
--------------- Part 2 --------------- 
I will try constructing a dictionary with key = all 4 digit combinations that show up in practice and values 
as the points scored across all secrets. There are 19^4 possible combinations of 4 digits so should be doable 
*/

Dictionary<string, long> sequenceTotalScore = [];

foreach (string str in initialSecrets)
{
	HashSet<string> presentSequences = [];
	long secret = Int64.Parse(str);
	long prevLastDigit = secret % 10;
	long lastDigit;
	List<long> sequence = [];

	for (int _ = 0; _ < 2000; _++)
	{
		secret = evolve(secret);
		lastDigit = secret % 10;	
		sequence.Add(lastDigit - prevLastDigit);
		prevLastDigit = lastDigit;
		
		if (_ > 3)
		{
			sequence.RemoveAt(0);
			string sequenceStr = String.Join("", sequence);
			
			if (!presentSequences.Contains(sequenceStr))
			{
				presentSequences.Add(sequenceStr);
				
				if (!sequenceTotalScore.ContainsKey(sequenceStr))
				{
					sequenceTotalScore[sequenceStr] = lastDigit;
				}
				else
				{
					sequenceTotalScore[sequenceStr] += lastDigit;
				}
			}
		}
	}
}

long maxValue = sequenceTotalScore.Values.Max();
foreach (string key in sequenceTotalScore.Keys)
{
	if (sequenceTotalScore[key] == maxValue)
	{
		Console.WriteLine("maxValue = " + maxValue);
		Console.WriteLine("Solution 2 = " + key);
		break;
	}
}

