using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using FrameWork.Core;
using FrameWork.Core.Data;
using FrameWork.DataService;
using FrameWork.IocService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FrameWork.CoreTest.Data
{
    [TestClass]
    public class TestBusiness
    {
        #region 初始化

        [TestInitialize]
        public void InitOrm()
        {
            new AutofacInitializer().Init();
            new EfInitializer().Init();
            Locator.Get<IDbFactory>().RegisterEntities(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// 新表
        /// </summary>
        private static IDbProvider SiteProvider
        {
            get
            {
                return Locator.Get<IDbFactory>()
                    .GetDbProvider("Supplier.Site");
            }
        }

        #endregion 初始化

        [TestMethod]
        public void CertifyTest()
        {
            using (var provider = SiteProvider)
            {
                var keyword = "访问发";

                var t = new ProductCategoriesEntity()
                {
                    Id = 1,
                    Name = keyword,
                    SortNum = "fafaw1",
                    State = 0,
                    Code = "faewa"
                };
                provider.Repository<ProductCategoriesEntity>().Insert(t);

                provider.Repository<ProductCategoriesEntity>().Delete(m => m.Id == 3);

                var query = from item in provider.Repository<ProductCategoriesEntity>().Query
                            where item.Name.Contains(keyword)
                            select new CategoriesModel
                            {
                                CategoriesId = item.Id,
                                CategoriesName = item.Name,
                                CategoryId = item.ProductCategory
                            };

                var list = query.ToList();

                Assert.IsTrue(list.Count != 0);
            }
        }
    }

    /// <summary>
    /// 小分类模型
    /// </summary>
    public class CategoriesModel
    {
        /// <summary>
        /// 小类Id
        /// </summary>
        public int CategoriesId { get; set; }

        /// <summary>
        /// 小类别名
        /// </summary>
        public string CategoriesName { get; set; }

        /// <summary>
        /// 大类Id
        /// </summary>
        public int CategoryId { get; set; }
    }

    /// <summary>
    /// 小分类
    /// </summary>
    public class ProductCategoriesEntity : IEntity
    {
        /// <summary>
        /// 小类Id
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 大类别ID
        /// </summary>
        public int ProductCategory { get; set; }

        /// <summary>
        /// 小类别名
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [Column("code")]
        public string Code { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Column("sortnum")]
        public string SortNum { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Column("state")]
        public int State { get; set; }
    }
}