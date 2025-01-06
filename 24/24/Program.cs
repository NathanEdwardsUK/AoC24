Console.WriteLine($"Parsing data");
var sr = new StreamReader("/Users/nathanedwards/Dev/AoC/24/24/input.txt");
string input = sr.ReadToEnd();
string[] splitInput = input.Split("\n\n");

Dictionary<string, int> wires = [];
List<string[]> gates = [];

foreach (string line in splitInput[0].Split("\n"))
{
	string[] splitLine = line.Split(": ");
	wires[splitLine[0]] = Int32.Parse(splitLine[1]);
}

foreach (string line in splitInput[1].Split("\n"))
{
	string[] splitLine = line.Replace("-> ", "").Split(" ");
	gates.Add(splitLine);
}

/* 
--------------- Part 1 --------------- 
Continuosly iterate through all gates checking if inputs are complete, if so processing output and putting
complete wire in wires dict
*/

while (gates.Count > 0)
{
	for (int i = gates.Count - 1; i >= 0; i--)
	{
		string[] gate = gates[i];
		string wire1 = gate[0];
		string wire2 = gate[2];
		
		if (!(wires.ContainsKey(wire1) && wires.ContainsKey(wire2)))
		{
			continue;
		}
		
		string op = gate[1];
		string wireOut = gate[3];
		
		if (op == "AND")
		{
			wires[wireOut] = wires[wire1] & wires[wire2];
		}		
		else if (op == "XOR")
		{
			wires[wireOut] = wires[wire1] ^ wires[wire2];
		}
		else if (op == "OR")
		{
			wires[wireOut] = wires[wire1] | wires[wire2];
		}
		else
		{
			throw new Exception ("invalid op");
		}
		
		gates.RemoveAt(i);
	}
}

List<string> sortedWires = wires.Keys.ToList();
sortedWires.Sort(String.Compare);
string binOutput = "";

foreach (string wire in sortedWires)
{
	if (wire.ElementAt(0) != 'z')
	{
		continue;
	}
	
	else 
	{
		binOutput = wires[wire] + binOutput;
	}	
}

Console.WriteLine("binary output = " + binOutput);
Console.WriteLine("Solution 1 = " + Convert.ToInt64(binOutput, 2));