namespace DataModel.Responses.BaseCms;

public class CmsPaginationResponse
{
    public int Page { get; set; }

    public int PageSize { get; set; }

    public int PageCount { get; set; }

    public int Total { get; set; }
}
