using FrameWork.Core;
using FrameWork.Core.Data;

namespace Rainds.Service
{
    internal class DbService
    {
        /// <summary>
        /// MySql数据库
        /// </summary>
        public static IDbProvider MySqlProvider
        {
            get
            {
                return Locator.Get<IDbFactory>()
                    .GetDbProvider<EfDbContext>();
            }
        }
    }
}