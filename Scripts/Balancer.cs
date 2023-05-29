using Regulators;

namespace Balancer
{
    public class Balancer
    {
        //
        private const int _x_dimmension = 337;
        private const int _y_dimmension = 269;
        //Current position
        private int _x_pos {get; set;}
        private int _y_pos {get; set;}

        //Desired position of ball 
        private int _x_setpoint {get; set;}
        private int _y_setpoint {get; set;}

        //Regulators
        private PID _reg1;
        private PID _reg2;

        public int XDim { get { return _x_dimmension; }}
        public int YDim { get { return _y_dimmension; }} 
    }
}


