using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Common.CustomExceptions
{
    public class CustomValidationException: Exception
    {
        public CustomValidationException(string message):base(message)
        {

        }
    }
}
