using System;
using DataModel.Responses.BaseCms;
using DataModel.Responses.Media;

namespace DataModel.Responses.Stories;

public class StoryQuizResponse
{
    public string Title { get; set; }

    public string A { get; set; }

    public string B { get; set; }

    public string C { get; set; }

    public string Answer { get; set; }

    public string Question { get; set; }

    public DateTime CreatedAt { get; set; }

    public CmsResponse<CmsItemResponse<CommonMediaItem>> Cover { get; set; }
}
