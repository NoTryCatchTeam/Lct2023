using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataModel.Definitions.Enums;
using Lct2023.ViewModels.Map;

namespace Lct2023.Definitions.VmLinks
{
	public class LocationsMapVmLink
	{
		public Func<Task<IEnumerable<PlaceItemViewModel>>> PlacesFactory { get; set; }

        public LocationType LocationType { get; set; }
    }
}

