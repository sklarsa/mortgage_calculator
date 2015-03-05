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

        byte[] _imageBytes;
        public byte[] ImageBytes
        {
            get { return _imageBytes; }
            set
            {
                _imageBytes = value;
                NotifyPropertyChanged("ImageBytes");
            }
        }

        public ObservableCollection<Cashflow> Cashflows { get; protected set; }

        public virtual void Calculate()
        {
            string tmpFile = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());

            StringBuilder sb = new StringBuilder();
            sb.Append("yield_calcs.py ");
            sb.Append(this.Notional.ToString() + " ");
            sb.Append(this.Rate.ToString() + " ");
            sb.Append(this.Months.ToString() + " ");
            sb.Append(this.Price.ToString() + " ");
            sb.Append('\"' + tmpFile + '\"');

            if(string.IsNullOrEmpty(this.SpeedType) &&
                this.SpeedAmount.HasValue)
            {
                sb.Append("-speed_type " + this.SpeedType);
                sb.Append(" -speed_amt" + this.SpeedAmount.ToString());
            }
            
            Process proc = new Process();
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.WorkingDirectory = Path.Combine(Environment.CurrentDirectory, "Python");
            proc.StartInfo.FileName = "python.exe";
            proc.StartInfo.Arguments = sb.ToString();
            proc.EnableRaisingEvents = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
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

                    //todo: Grab image from matplotlib output and add data to ImageBytes property

                    File.Delete(tmpFile);
                }

            };

            proc.Start();
            proc.WaitForExit();
            Debug.Write(proc.StandardOutput.ReadToEnd());
            Debug.Write(proc.StandardError.ReadToEnd());
            Debug.Write("Process exited with code " + proc.ExitCode);
            proc.Close();

          
        }

        public virtual void ExportToExcel()
        {
            //todo: Implement this method
        }

        public virtual void ExportToCSV()
        {
            //todo: Implement this method
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
