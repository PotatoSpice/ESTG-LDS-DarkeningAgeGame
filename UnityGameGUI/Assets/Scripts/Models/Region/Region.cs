using System.Collections.Generic;
public class Region
{
    public string name { get; set; }
    public string owner { get; set; }
    public string terrain { get; set; }
    public float size { get; set; }
    public List<string> borders { get; set; }
    public RegionType type { get; set; }

    public Region(string name)
    {
        this.name = name;
        borders = new List<string>();
        type = new RegionType();
    }
}
