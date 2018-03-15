using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceGenerique
{
    public class Time
    {
        private short _Heure = 0;
        private short _Minute = 0;
        private short _Second = 0;

        public Time()
        {
        }

        public Time(short Heure, short Minute)
        {
            this.Heure = Heure;
            this.Minute = Minute;
        }

        public Time(short Heure, short Minute, short second)
        {
            this.Heure = Heure;
            this.Minute = Minute;
            this.Second = second;
        }

        public Time(double time)
        {
        }

        public Time(DateTime time)
        {
            this.Heure = (short)time.Hour;
            this.Minute = (short)time.Minute;
            this.Second = (short)time.Second;
        }

        public short Heure
        {
            get
            {
                return _Heure;
            }
            set
            {
                if (value < 24 && value >= 0)
                    this._Heure = value;
                else
                    throw new Exception();
            }
        }

        public short Minute
        {
            get
            {
                return _Minute;
            }
            set
            {
                if (value < 60 && value >= 0)
                    this._Minute = value;
                else
                    throw new Exception();
            }
        }

        public short Second
        {
            get
            {
                return _Second;
            }
            set
            {
                if (value < 60 && value >= 0)
                    this._Second = value;
                else
                    throw new Exception();
            }
        }



        public static int operator -(Time c1, Time c2)
        {
            if (c1._Heure > c2._Heure)
                return 60 * 60 * ((24 - c1._Heure) + c2._Heure) + 60 * (c2._Minute - c1._Minute) + (c2.Second - c1.Second);
            else
                return 60 * 60 * (c2._Heure - c1._Heure) + 60 * (c2._Minute - c1._Minute) + (c2.Second - c1.Second);
        }


        public static int operator +(int c1, Time c2)
        {
            return c1 + (60 * 60 * c2._Heure) + (60 * c2._Minute) + c2._Second;
        }


        public static int operator -(Time c1, DateTime c2)
        {
            return c1 - new Time(c2);
        }

        public static int operator -(DateTime c1, Time c2)
        {
            return new Time(c1) - c2;
        }

        public override int GetHashCode()
        {
            return (this._Heure.ToString("d2") + this._Minute.ToString("d2") + this._Second.ToString("d2")).GetHashCode();
        }


        public override bool Equals(object obj)
        {
            if (obj is Time)
                return (this._Heure == ((Time)obj).Heure) && (this._Minute < ((Time)obj).Minute) && (this._Second == ((Time)obj).Second);
            else
                return false;
        }

        public static bool operator <(Time c1, Time c2)
        {
            return (c1._Heure < c2._Heure) || ((c1._Heure == c2._Heure) && (c1._Minute < c2._Minute)) || ((c1._Heure == c2._Heure) && (c1._Minute == c2._Minute) && (c1._Second < c2._Second));
        }

        public static bool operator <(Time c1, DateTime c2)
        {
            return c1 < new Time(c2);
        }

        public static bool operator <=(Time c1, Time c2)
        {
            return (c1 < c2) || (c1 == c2);
        }

        public static bool operator <=(Time c1, DateTime c2)
        {
            return c1 <= new Time(c2);
        }

        public static bool operator >=(Time c1, Time c2)
        {
            return (c1._Heure > c2._Heure) || ((c1._Heure == c2._Heure) && (c1._Minute > c2._Minute)) || ((c1._Heure == c2._Heure) && (c1._Minute == c2._Minute) && (c1._Second > c2._Second));
        }

        public static bool operator >=(Time c1, DateTime c2)
        {
            return c1 >= new Time(c2);
        }

        public static bool operator >(Time c1, Time c2)
        {
            return (c1._Heure > c2._Heure) || ((c1._Heure == c2._Heure) && (c1._Minute > c2._Minute));
        }

        public static bool operator >(Time c1, DateTime c2)
        {
            return c1 > new Time(c2);
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", Heure.ToString("d2"), Minute.ToString("d2"));
        }

    }
}
