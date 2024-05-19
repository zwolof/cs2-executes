namespace ExecutesPlugin.Models;

public class MapConfig
{
    public List<Scenario> Scenarios { get; set; } = new();
    public List<Spawn> Spawns { get; set; } = new();
    public List<Grenade> Grenades { get; set; } = new();
}