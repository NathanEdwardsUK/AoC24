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

List<string[]> deepCopy(List<string[]> _gates)
{
	List<string[]> newGates = [];
	
	foreach (string[] _gate in _gates)
	{
		string[] newGate = new string[4];
		Array.Copy(_gate, newGate, 4);
		newGates.Add(newGate);
	}
	
	return newGates;
}

List<string[]> gatesBackup = new(gates);

string binInputX = "";
string binInputY = "";

foreach (string wire in wires.Keys)
{
	if (wire.ElementAt(0) == 'x')
	{
		binInputX = wires[wire] + binInputX;
	}
	else if (wire.ElementAt(0) == 'y')
	{
		binInputY = wires[wire] + binInputY;
	}
}

long inputX = Convert.ToInt64(binInputX, 2);
long inputY = Convert.ToInt64(binInputY, 2);

/* 
--------------- Part 1 --------------- 
Continuosly iterate through all gates checking if inputs are complete, if so processing output and putting
complete wire in wires dict
*/

string wireName(string prefix, int i)
{
	if (i < 10)
	{
		return prefix + "0" + i;
	}
	else
	{
		return prefix + i;
	}
}

string run(string strX, string strY, List<string[]> _gates)
{
	// _gates = new(_gates);
	_gates = deepCopy(_gates);
	if (strX.Length < 45)
	{
		strX = String.Join("", Enumerable.Repeat(0, 45 - strX.Length)) + strX;
	}
	
	if (strY.Length < 45)
	{
		strY = String.Join("", Enumerable.Repeat(0, 45 - strY.Length)) + strY;
	}
	
	char[] charArrX = strX.ToCharArray();
	char[] charArrY = strY.ToCharArray();
	Dictionary<string, int> _wires = [];
	
	for (int i = 0; i < 45; i++)
	{
		_wires[wireName("x", i)] = Int32.Parse("" + charArrX[45 - i - 1]);
		_wires[wireName("y", i)] = Int32.Parse("" + charArrY[45 - i - 1]);
	}
	
	while (_gates.Count > 0)
	{
		// If _gatesChanged remains false by the end of the iteration then return "" as the _gates 
		// do not correspond to a working program
		bool _gatesChanged = false;
		
		for (int i = _gates.Count - 1; i >= 0; i--)
		{
			string[] gate = _gates[i];
			string wire1 = gate[0];
			string wire2 = gate[2];
			
			if (!(_wires.ContainsKey(wire1) && _wires.ContainsKey(wire2)))
			{
				continue;
			}
			
			_gatesChanged = true;
			string op = gate[1];
			string wireOut = gate[3];
			
			if (op == "AND")
			{
				_wires[wireOut] = _wires[wire1] & _wires[wire2];
			}		
			else if (op == "XOR")
			{
				_wires[wireOut] = _wires[wire1] ^ _wires[wire2];
			}
			else if (op == "OR")
			{
				_wires[wireOut] = _wires[wire1] | _wires[wire2];
			}
			else
			{
				throw new Exception ("invalid op");
			}
			
			_gates.RemoveAt(i);
			// Console.WriteLine($"{wire1} {op} {wire2} {wireOut}");
		}
		
		if (!_gatesChanged)
		{
			return "";
		}
	}

	List<string> sortedWires = _wires.Keys.ToList();
	sortedWires.Sort(String.Compare);
	string _binOutput = "";

	foreach (string wire in sortedWires)
	{
		if (wire.ElementAt(0) == 'z')
		{
			_binOutput = _wires[wire] + _binOutput;
		}

	}
	
	return _binOutput;
}

string binOutput = run(binInputX, binInputY, gates);

long output = Convert.ToInt64(binOutput, 2);

Console.WriteLine("binary x = " + binInputX + ", decimal x = " + inputX);
Console.WriteLine("binary y = " + binInputY + ", decimal y = " + inputY);
Console.WriteLine("binary o = " + binOutput + ", decimal o = " + output);
Console.WriteLine("Solution 1 = " + output);
Console.WriteLine(inputX + inputY);

/* 
--------------- Part 2 --------------- 
I tried a lot of different approaches to solve this purely programmatically, all failed. In the end I solved this by ordering and grouping 
the gates based on which output bit they are needed to calculate. I was able to find a structure in the boolean logic which allowed me 
to manually check each group and find the groups the had incorrect output wires. In particular note that each group (after first 2) has 5
gates, 2 XOR, 2 AND and one OR. Upon finding the suspect groups I was able to find the specific incorrect gate by further analysing the
structure. In particular Z wires should always output a XOR gate, one of the inputs of that XOR gate should always be a x__ AND y__ gate etc...

After find all the offending gates I just sorted them and joined by commas. Not a beautiful solution but it worked.

first few groups example: 
x00 XOR y00 z00

pgc XOR tct z01
x00 AND y00 pgc
y01 XOR x01 tct

pfv XOR ndk z02
mwc OR qjs pfv
x01 AND y01 mwc
tct AND pgc qjs
x02 XOR y02 ndk

rnp XOR mbj z03
hwc OR mvv rnp
ndk AND pfv hwc
y02 AND x02 mvv
y03 XOR x03 mbj
*/

int gateIndex(string wireOut, List<string[]> _gates)
{
	for (int i = 0; i < _gates.Count; i++)
	{
		if (_gates[i][3].Equals(wireOut))
		{
			return i;
		} 
	}
	
	return -1;
}

List<string> orderedOutWires = [];
List<string[]> orderedGates = [];

for (int i = 0; i < 45; i++)
{
	string zWire = wireName("z", i);
	int idx = gateIndex(zWire, gates);
	string[] gate = gates[idx];
	
	orderedOutWires.Add(zWire);
	orderedGates.Add(gate);
	Console.WriteLine(String.Join(" ", gate));
	
	List<string> childWires = [gate[0], gate[2]];
	
	while (childWires.Count > 0)
	{
		string wire = childWires[0];
		
		if (!orderedOutWires.Contains(wire))
		{
			idx = gateIndex(wire, gates);
			
			if (idx == -1)
			{
				childWires.RemoveAt(0);
				continue;
			}
			
			gate = gates[gateIndex(wire, gates)];	
			orderedOutWires.Add(wire);
			orderedGates.Add(gate);
			childWires = [gate[0], gate[2], ..childWires];
			Console.WriteLine(String.Join(" ", gate));
		}
		else
		{
			childWires.RemoveAt(0);
		}
	}
	
	Console.WriteLine();
}

List<string> dummy = ["z06", "ksv", "kbs", "z20", "z39", "ckb", "tqq", "nbd"];
dummy.Sort(String.Compare);
Console.WriteLine("Solution 2 = " + String.Join(",", dummy));