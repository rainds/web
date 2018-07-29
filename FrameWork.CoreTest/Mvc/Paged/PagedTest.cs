using System.Collections.Generic;
using FrameWork.Core.Mvc.Paged;
using FrameWork.Core.Mvc.Result;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FrameWork.CoreTest.Mvc.Paged
{
    public class UserModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; }
    }

    [TestClass]
    public class PagedTest
    {
        [TestMethod]
        public void PagedListTest()
        {
            var userList = new List<UserModel>() {
                new UserModel{ UserId=1,UserName="123"},
                new UserModel{ UserId=2,UserName="2123"},
                new UserModel{ UserId=3,UserName="3123"},
                new UserModel{ UserId=4,UserName="4123"},
                new UserModel{ UserId=5,UserName="5123"},
                new UserModel{ UserId=6,UserName="6123"},
                new UserModel{ UserId=7,UserName="7123"},
                new UserModel{ UserId=8,UserName="8123"},
                new UserModel{ UserId=9,UserName="9123"},
                new UserModel{ UserId=10,UserName="11123"},
                new UserModel{ UserId=11,UserName="12123"},
                new UserModel{ UserId=12,UserName="13123"},
                new UserModel{ UserId=13,UserName="15123"},
                new UserModel{ UserId=14,UserName="16123"},
            };

            var pagedList = userList.ToPagedList();

            var result = new OperationResult<IPagedList<UserModel>>(pagedList);
        }
    }
}