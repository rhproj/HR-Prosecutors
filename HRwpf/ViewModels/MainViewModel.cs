using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRwpf.ViewModels
{
    class MainViewModel
    {
        private int _test = 5;
        public int Test
        {
            get { return _test = 5; }
            set { _test = value; }
        }

    }
}
