using System;
using System.Collections.Generic;

namespace GameWebServer.Entities.Game
{
	public class Region
	{
		public String name { get; set; }
		public String owner { get; set; }
		public String type { get; set; }
		public String terrain { get; set; }
		public int size { get; set; }
		public List<String> Borders { get; set; }
	}
}
