namespace Regulators
{
    public class PID
    {
        private double _p {get; set;}
        private double _i {get; set;}
        private double _d {get; set;}
        private double _err {get; set;}
        private double _y {get; set;}

        public PID(double p, double i, double d)
        {
            this._p = p;
            this._i = i;
            this._d = d;
            this._err = 0.0;
            this._y = 0.0;
        }
    }
}

