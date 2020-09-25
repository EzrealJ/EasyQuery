using Ezreal.EasyQuery.Test.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ezreal.EasyQuery.Test.Data
{
   public class User
    {
        /// <summary>
        /// 用户唯一识别
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 关联的部门Id
        /// </summary>
        public Guid? DepartmentId { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 学号
        /// </summary>
        public string EmpId { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        public string IdCard { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int? Sex { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public int? Age { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 用户状态
        /// </summary>
        public EnumUserState State { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 照片
        /// </summary>
        public string Photo { get; set; }
        /// <summary>
        /// 消费密码
        /// </summary>
        public string PayKey { get; set; }

    }
}
