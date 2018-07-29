namespace FrameWork.Core.Principal
{
    public interface IPrincipal
    {
        TokenInfo TokenInfo { get; set; }
    }
}