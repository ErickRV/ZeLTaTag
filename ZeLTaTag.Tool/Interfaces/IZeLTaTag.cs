using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeLTaTag.Tool.Interfaces
{
    public interface IZeLTaTag
    {
        event EventHandler<string> OnTagReaded;
        public void ClearPreviousRead();
        public bool isCurrentlyReading();
    }
}
