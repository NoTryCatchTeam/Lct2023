using System;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Feed;
using System.Collections.Generic;
using Lct2023.ViewModels.Feed;

namespace Lct2023.Definitions.VmLinks
{
	public class ArtDirectionFeedVmLink
	{
		public string ArtDirection { get; set; }

		public IEnumerable<CmsItemResponse<RubricResponse>> Rubrics { get; set; }
	}
}

