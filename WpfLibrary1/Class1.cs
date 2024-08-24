using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfLibrary1
{
    public class Matrix
    {
        ObservableCollection<MyClass> Matr;

        public Matrix(ObservableCollection<MyClass> Matr)
        {
            this.Matr = Matr;
        }

        public static ObservableCollection<MyClass> Copy(ObservableCollection<MyClass> collection)
        {
            return Copy(collection, false);
        }

        public static ObservableCollection<MyClass> Copy(ObservableCollection<MyClass> collection, bool _null)
        {
            ObservableCollection<MyClass> copy = new ObservableCollection<MyClass>();
            for (int i = 0; i < collection.Count; i++)
            {
                ObservableCollection<string> tmp_MValues = new ObservableCollection<string>();
                for (int j = 0; j < collection.Count; j++)
                    tmp_MValues.Add(_null ? null : collection[i].MValues[j]);

                copy.Add(new MyClass() { MValues = tmp_MValues });
            }
            return copy;
        }

        public static ObservableCollection<MyClass> operator +(Matrix a, Matrix b)
        {
            ObservableCollection<MyClass> Result = Copy(b.Matr);
            try
            {
                for (int i = 0; i < a.Matr.Count; i++)
                    for (int j = 0; j < a.Matr.Count; j++)
                        Result[i].MValues[j] = (double.Parse(a.Matr[i].MValues[j]) + double.Parse(b.Matr[i].MValues[j])).ToString();
                return Result;
            }
            catch
            {
                return Copy(b.Matr, true);
            }
        }

        public static ObservableCollection<MyClass> operator -(Matrix a, Matrix b)
        {
            ObservableCollection<MyClass> Result = Copy(b.Matr);
            try
            {
                for (int i = 0; i < a.Matr.Count; i++)
                    for (int j = 0; j < a.Matr.Count; j++)
                        Result[i].MValues[j] = (double.Parse(a.Matr[i].MValues[j]) - double.Parse(b.Matr[i].MValues[j])).ToString();
                return Result;
            }
            catch
            {
                return Copy(b.Matr, true);
            }
        }

        public static ObservableCollection<MyClass> operator *(Matrix a, Matrix b)
        {
            ObservableCollection<MyClass> Result = Copy(b.Matr);
            try
            {
                for (int i = 0; i < a.Matr.Count; i++)
                    for (int j = 0; j < a.Matr.Count; j++)
                    {
                        Result[i].MValues[j] = "0";
                        for (int k = 0; k < a.Matr.Count; k++)
                            Result[i].MValues[j] = (double.Parse(Result[i].MValues[j]) + double.Parse(a.Matr[i].MValues[k]) * double.Parse(b.Matr[k].MValues[j])).ToString();
                    }
                        
                return Result;
            }
            catch
            {
                return Copy(b.Matr, true);
            }
        }

        public static void Transp(ObservableCollection<MyClass> Matr)
        {
            string tmp;
            for (int i = 0; i < Matr.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    tmp = Matr[i].MValues[j];
                    Matr[i].MValues[j] = Matr[j].MValues[i];
                    Matr[j].MValues[i] = tmp;
                }
            }
        }
    }

    public class MyClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private ObservableCollection<string> _mValues;
        public MyClass()
        {
            _mValues = new ObservableCollection<string>();
            MValues.CollectionChanged += MValues_CollectionChanged;
        }
        public ObservableCollection<string> MValues
        {
            get
            {
                return _mValues;
            }
            set
            {
                _mValues = value;
                MValues.CollectionChanged += MValues_CollectionChanged;
                OnPropertyChanged("MValues");
            }
        }

        private void MValues_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("MValues");
        }
    }
}
