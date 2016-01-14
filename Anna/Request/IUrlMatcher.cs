namespace Anna.Request
{
    interface IUrlMatcher
    {
        bool Matches(RequestContext ctx);
    }
}