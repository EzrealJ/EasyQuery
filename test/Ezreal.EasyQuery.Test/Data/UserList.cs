using System;
using System.Collections.Generic;
using System.Text;

namespace Ezreal.EasyQuery.Test.Data
{
    public class UserList : List<User>
    {
        public UserList()
        {
            Add(new User() { Name = "用户一", Id = Guid.NewGuid(), CreateTime = DateTime.Now, Phone = "123-456-789", State = Enums.EnumUserState.Loss });
            Add(new User() { Name = "用户er", Id = Guid.NewGuid(), CreateTime = DateTime.Now.AddDays(-1), Phone = "123-4776-789", State = Enums.EnumUserState.Enable });
            Add(new User() { Name = "用户3", Id = Guid.NewGuid(), CreateTime = DateTime.Now.AddDays(1), Phone = "123-458-789", State = Enums.EnumUserState.Logout });
            Add(new User() { Name = "用户四", Id = Guid.NewGuid(), CreateTime = DateTime.Now.AddMonths(-2), Phone = "124-456-759", State = Enums.EnumUserState.Loss });
            Add(new User() { Name = "用户一", Id = Guid.NewGuid(), CreateTime = DateTime.Now, Phone = "123-456-789", State = Enums.EnumUserState.UnRegister });
            Add(new User() { Name = "S用户一", Id = Guid.NewGuid(), CreateTime = DateTime.Now, Phone = "123-456-789", State = Enums.EnumUserState.UnRegister });
        }
    }
}
