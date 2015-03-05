using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mortgage_calculator.Model
{
    class Cashflow
    {
        public double period { get; set; }
        public double prepayment { get; set; }
        public double total_prin { get; set; }
        public double reg_prin { get; set; }
        public double total_pmt { get; set; }
        public double interest { get; set; }
        public double balance { get; set; }
        public double sched_pmt { get; set; }
    }
}
