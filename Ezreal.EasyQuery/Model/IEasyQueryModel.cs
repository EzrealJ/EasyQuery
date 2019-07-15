using System;
using System.Collections.Generic;
using System.Text;

namespace Ezreal.EasyQuery.Model
{
    public interface IEasyQueryModel
    {
        void InitializeFromJsonObjectString(string jsonObjectString);
    }
}
