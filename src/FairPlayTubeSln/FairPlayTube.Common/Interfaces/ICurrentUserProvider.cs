namespace FairPlayTube.Common.Interfaces
{
    public interface ICurrentUserProvider
    {
        string GetUsername();
        string GetObjectId();
    }
}
