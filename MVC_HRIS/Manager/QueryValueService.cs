namespace MVC_HRIS.Manager
{
    public class QueryValueService
    {
      private readonly IHttpContextAccessor _httpContextAccessor;

    public QueryValueService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

        public string GetValue()
        {
            string Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiMSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvdmVyc2lvbiI6IlYzLjUiLCJuYmYiOjE2NzI4NTM2NTQsImV4cCI6MTY4MTQ5MzY1NCwiaWF0IjoxNjcyODUzNjU0fQ.Ca-_dhC0Y0MTH3IFlgKLQ2z26UJEX-EBYSXrPNKCTaA";
            return Token;
        }
    }
}
