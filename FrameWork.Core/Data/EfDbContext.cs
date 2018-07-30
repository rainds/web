namespace FrameWork.Core.Data
{
    public class EfDbContext : BaseDbContext
    {
        public EfDbContext() : base("DefaultConnection")
        {
        }
    }
}