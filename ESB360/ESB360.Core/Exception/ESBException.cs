using System;
using System.Collections.Generic;
using System.Text;

namespace ESB360.Core
{
    public class ESBException : Exception
    {
        public ESBException(string exception):base(exception)
        {

        }
    }
}
