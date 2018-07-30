using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using FrameWork.Core;
using FrameWork.Core.Data;
using FrameWork.IocService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FrameWork.CoreTest.Data
{
    public class A
    {
        public int AID { get; set; }
        public string Class { get; set; }
    }

    public class B
    {
        public int BID { get; set; }
        public string BName { get; set; }
        public int AID { get; set; }
    }

    [TestClass]
    public class TestOrm
    {
        [TestInitialize]
        public void InitOrm()
        {
            new AutofacInitializer().Init();
            Locator.Get<IDbFactory>().RegisterEntities(Assembly.GetExecutingAssembly());
        }

        private static Core.Data.IDbProvider DbProvider
        {
            get
            {
                return Locator.Get<Core.Data.IDbFactory>()
                    .GetDbProvider<EfDbContext>();
            }
        }

        private static Core.Data.IDbProvider DbProvider1
        {
            get
            {
                return Locator.Get<Core.Data.IDbFactory>()
                   .GetDbProvider<EfDbContext>();
            }
        }

        [TestMethod]
        public void TestDal()
        {
            //var thread = new Thread(() =>
            //{
            var watch = new Stopwatch();
            for (var i = 0; i < 3; i++)
            {
                watch.Restart();
                using (var provider = DbProvider)
                {
                    var repository = provider.Repository<ProductEntity>();
                    repository.Insert(new ProductEntity { ProductCode = "001", ProductName = "TestName" });

                    repository.Update(new ProductEntity { ProductCode = "001", ProductDesc = "TestDesc" });
                    var model = repository.Find(e => e.ProductCode == "001");
                    if (model != null)
                    {
                        repository.Delete(model);
                    }
                }
                watch.Stop();
                Debug.WriteLine("总共时间：" + watch.ElapsedMilliseconds);
            }
            //});
            //thread.Start();
            //thread.Join();
        }

        [TestMethod]
        public void TestBatch1()
        {
            using (var provider = DbProvider)
            {
                var repository = provider.Repository<ProductEntity>();
                repository.Insert(new ProductEntity { ProductCode = "001", ProductName = "TestName" });
                repository.Update(e => e.ProductCode == "001",
                    m => new ProductEntity { ProductName = "TestBatchName" });
                repository.Delete(e => e.ProductCode == "001");
            }
        }

        [TestMethod]
        public void TestBatch2()
        {
            for (var i = 0; i < 10; i++)
            {
                using (var provider = DbProvider)
                {
                    //使用计算公式批量更新所有符合特定条件的实体
                    var subRepository = provider.Repository<OrderSubEntity>();
                    subRepository.Insert(new OrderSubEntity { ProductCode = "001", Price = 100, Qty = 3 });
                    subRepository.Update(e => e.ProductCode == "001", u => new OrderSubEntity { Qty = u.Qty + 1 });
                }
            }
        }

        [TestMethod]
        public void TestJoin()
        {
            using (var provider = DbProvider)
            {
                var query = from order in provider.Repository<OrderEntity>().Query
                            join orderSub in provider.Repository<OrderSubEntity>().Query on order.OrderId equals
                                orderSub.OrderSubId
                            where orderSub.ProductCode == "001"
                            select new
                            {
                                order.OrderId,
                                order.CreateUser,
                                orderSub.ProductCode
                            };
                var count = query.Count();
                var pagedList = query.OrderBy(m => m.OrderId).Skip(3).Take(10).ToList();
                var list = query.ToList();
                Assert.IsTrue(list.Count != 0);
            }
        }

        [TestMethod]
        public void TestLeftJoin()
        {
            List<A> A = new List<A>()
            {
                new A(){ AID = 1, Class="班级1" },
                new A(){ AID = 2, Class="班级2" },
                 new A(){ AID = 3, Class="班级3" },
            };

            List<B> B = new List<B>()
            {
                new B(){ BID = 1 , BName = "学生1", AID=1 },
                new B(){ BID = 2 , BName = "学生2", AID=2 },
                new B(){ BID = 3 , BName = "学生3", AID=1 },
                new B(){ BID = 4 , BName = "学生4", AID=2 },
                 new B(){ BID = 5 , BName = "学生5", AID=4 },
            };

            var innerResult = from a in A
                              join b in B
                              on a.AID equals b.AID
                              select new
                              {
                                  CLASS = a.Class,
                                  Name = b.BName,
                              };

            var leftResult = from a in A
                             join b in B
                             on a.AID equals b.AID into ab

                             select new
                             {
                                 CLASS = a.Class,
                                 Name = ab,
                             };

            var leftResult1 = from a in A
                              join b in B
                              on a.AID equals b.AID into ab
                              from v in ab.DefaultIfEmpty()
                              select new
                              {
                                  CLASS = a.Class,
                                  Name = ab,
                                  t = v
                              };

            var leftResult3 = from b in B
                              join a in A
                              on b.AID equals a.AID into ab
                              from v in ab.DefaultIfEmpty()
                              select new
                              {
                                  CLASS = b.BName,
                                  Name = ab,
                                  t = v
                              };

            var leftResult4 = from b in B
                              join a in A
                              on b.AID equals a.AID
                              select new
                              {
                                  b,
                                  a
                              };

            var leftResult2 = from a in A
                              join b in B
                              on a.AID equals b.AID into ab
                              from v in ab.DefaultIfEmpty(new B { AID = 0, BID = 0, BName = "" })
                              select new
                              {
                                  CLASS = a.Class,
                                  Name = ab,
                                  t = v
                              };
            var list = innerResult.ToList();
            var list1 = leftResult.ToList();
            var list2 = leftResult1.ToList();
            var list3 = leftResult2.ToList();
            var list4 = leftResult3.ToList();
            var list5 = leftResult4.ToList();
        }

        [TestMethod]
        public void TestTran()
        {
            var thread = new Thread(() =>
            {
                for (var i = 0; i < 10; i++)
                {
                    SaveOrder("User001", false);
                }
            });
            thread.Start();
            thread.Join();
        }

        [TestMethod]
        public void TestTran2()
        {
            for (var i = 0; i < 10; i++)
            {
                SaveOrder("User002", true);
            }
        }

        private static void SaveOrder(string usercode, bool commitTran)
        {
            using (var provider = DbProvider)
            {
                var orderRepository = provider.Repository<OrderEntity>();
                var subRepository = provider.Repository<OrderSubEntity>();

                provider.Begin();
                try
                {
                    var orderEntity = new OrderEntity { CreateUser = usercode, CreateTime = DateTime.Now };
                    orderRepository.Insert(orderEntity);
                    subRepository.Insert(new OrderSubEntity
                    {
                        OrderId = orderEntity.OrderId,
                        Price = 3m,
                        Qty = 1,
                        ProductCode = "001"
                    });
                    if (commitTran) provider.Commit();
                }
                catch (Exception)
                {
                    provider.Rollback();
                }
            }
        }
    }
}