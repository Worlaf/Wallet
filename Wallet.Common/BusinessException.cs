using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Common
{
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message)
        {

        }
    }
}
