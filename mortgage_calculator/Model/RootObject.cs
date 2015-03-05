using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mortgage_calculator.Model
{
    /// <summary>
    /// A wrapper object used to deserialize loan data and analytics 
    /// from JSON output from yield_calcs.py
    /// Generated using http://json2csharp.com/
    /// </summary>
    class RootObject
    {
        public double macaulay_dur { get; set; }
        public List<Cashflow> cashflows { get; set; }
        public double yield { get; set; }
        public double modified_dur { get; set; }
        public double wal { get; set; }
    }
}
