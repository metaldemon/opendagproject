using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opendagproject.Game.RSL.Functions
{
    class FunctionHandler
    {
        public static List<Function> functionList = new List<Function>();

        public static void executeFunction(string name)
        {
            if (functionList.Exists(x => x.name == name))
            {
                functionList.First(x => x.name == name).execute();
            }
            else
            {

            }
        }
    }
}
