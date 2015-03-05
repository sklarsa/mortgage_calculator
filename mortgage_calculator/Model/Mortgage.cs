using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace mortgage_calculator.Model
{
    class Mortgage : INotifyPropertyChanged
    {

        #region Constructor(s)

        public Mortgage()
        {
            this.SpeedType = "CPR";
            this.Cashflows = new ObservableCollection<Cashflow>();
            this.Price = 100;
            this.Notional = 1000000;
            this.Rate = 4;
            this.Months = 360;
        }

        #endregion

        double _notional;
        public double Notional 
        { 
            get { return _notional; } 
            set 
            {
                _notional = value;
                NotifyPropertyChanged("Notional");
            } 
        }

        double _rate;
        public double Rate
        {
            get { return _rate; }
            set
            {
                _rate = value;
                NotifyPropertyChanged("Rate");
            }
        }

        int _months;
        public int Months
        {
            get { return _months; }
            set
            {
                _months = value;
                NotifyPropertyChanged("Months");
            }
        }

        string _speedType;
        public string SpeedType
        {
            get { return _speedType; }
            set 
            {
                _speedType = value;
                NotifyPropertyChanged("SpeedType");
            }
        }

        double? _speedAmount;
        public double? SpeedAmount 
        {
            get { return _speedAmount; }
            set
            {
                _speedAmount = value;
                NotifyPropertyChanged("SpeedAmount");
            }
        }

        double _price;
        public double Price
        {
            get{return _price;}
            set
            {
                _price = value;
                NotifyPropertyChanged("Price");
            }
        }

        double? _yield;
        public double? Yield 
        { 
            get { return _yield; }
            set
            {
                _yield = value;
                NotifyPropertyChanged("Yield");
            }
        }

        double? _wal;
        public double? Wal
        {
            get { return _wal; }
            set
            {
                _wal = value;
                NotifyPropertyChanged("Wal");
            }
        }
        
        double? _macaulayDuration;
        public double? MacaulayDuration
        {
            get { return _macaulayDuration; }
            set
            {
                _macaulayDuration = value;
                NotifyPropertyChanged("MacaulayDuration");
            }
        }
        
        double? _modifiedDuration;
        public double? ModifiedDuration
        {
            get { return _modifiedDuration; }
            set
            {
                _modifiedDuration = value;
                NotifyPropertyChanged("ModifiedDuration");
            }
        }

        ObservableCollection<Cashflow> _cashflows;
        public ObservableCollection<Cashflow> Cashflows
        {
            get { return _cashflows; }
            set 
            { 
                _cashflows = value;
                NotifyPropertyChanged("Cashflows");
            }
        }

        public virtual void Calculate()
        {
            string tmpFile = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());

            StringBuilder sb = new StringBuilder();
            sb.Append(CreateCoreArgs());
            sb.Append('\"' + tmpFile + '\"');


            Process proc = CreateProcess(sb.ToString());

            proc.Exited += (s, e) =>
            {
                if (File.Exists(tmpFile))
                {
                    string buffer;
                    using (StreamReader r = new StreamReader(tmpFile))
                    {
                        buffer = r.ReadToEnd();
                    }

                    RootObject obj = JsonConvert.DeserializeObject<RootObject>(buffer);

                    if (obj != null)
                    {
                        this.Yield = obj.yield;
                        this.Wal = obj.wal;
                        this.MacaulayDuration = obj.macaulay_dur;
                        this.ModifiedDuration = obj.modified_dur;
                        this.Cashflows = new ObservableCollection<Cashflow>(obj.cashflows);
                    }
                    else
                    {
                        this.Yield = null;
                        this.Wal = null;
                        this.MacaulayDuration = null;
                        this.ModifiedDuration = null;
                        this.Cashflows = new ObservableCollection<Cashflow>();
                    }

                    File.Delete(tmpFile);
                }

            };

            RunProcess(proc);
          
        }

        public virtual void ExportToExcel(string path)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(CreateCoreArgs());
            sb.Append('\"' + path + '\"');
            sb.Append(" -output_format xlsx");

            Process proc = CreateProcess(sb.ToString());

            RunProcess(proc);
        }

        public virtual void ExportToCSV(string path)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(CreateCoreArgs());
            sb.Append('\"' + path + '\"');
            sb.Append(" -output_format csv");

            Process proc = CreateProcess(sb.ToString());

            RunProcess(proc);
        }

        private Process CreateProcess(string args)
        {

            Process proc = new Process();
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.WorkingDirectory = Path.Combine(Environment.CurrentDirectory, "Python");
            proc.StartInfo.FileName = "python.exe";
            proc.StartInfo.Arguments = args;
            proc.EnableRaisingEvents = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;

            return proc;
        }

        private int RunProcess(Process proc)
        {
            int exitCode;

            proc.Start();
            proc.WaitForExit();
            Debug.Write(proc.StandardOutput.ReadToEnd());
            Debug.Write(proc.StandardError.ReadToEnd());
            exitCode = proc.ExitCode;
            Debug.Write("Process exited with code " + proc.ExitCode);
            proc.Close();

            return exitCode;

        }

        private string CreateCoreArgs()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("yield_calcs.py ");
            sb.Append(this.Notional.ToString() + " ");
            sb.Append(this.Rate.ToString() + " ");
            sb.Append(this.Months.ToString() + " ");
            sb.Append(this.Price.ToString() + " ");

            if (!string.IsNullOrEmpty(this.SpeedType) &&
                this.SpeedAmount.HasValue)
            {
                sb.Append(" -speed_type " + this.SpeedType.ToLower());
                sb.Append(" -speed_amt " + this.SpeedAmount.ToString());
            }

            return sb.ToString();

        }

        #region INotifyPropertyChanged Implementation

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
