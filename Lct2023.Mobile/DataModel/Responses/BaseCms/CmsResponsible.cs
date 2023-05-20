namespace DataModel.Responses.BaseCms
{
    public struct CmsResponsible<TResult>
    {
        public TResult Data { get; set; }
        
        public CmsMetaResponse Meta { get; set; }
        
        public CmsErrorResponse Error { get; set; }
    }
}