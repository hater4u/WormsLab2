using WormsLab.Interfaces;

namespace WormsLab.Generators;

public class SimpleWormNameGenerator: IWormNameGenerator
{
    private int _wormCount = 1;
    private List<String> _templates = new() {"John", "Mary", "Karl"};
    private Random _random;

    public SimpleWormNameGenerator(int? seed = null)
    {
        _random = seed == null ? new Random() : new Random((int)seed);
    }
    
    public string GenerateName()
    {
        return $"{_templates[_random.Next(0, _templates.Count)]}[{_wormCount++}]";
    }
}