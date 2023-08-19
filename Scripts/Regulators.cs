public class PID
{
    public double P {get; set;}
    public double I {get; set;}
    public double D {get; set;}
    public double Error {get; set;}
    public double RegulatorOutput {get; set;}

    public PID(double p, double i, double d)
    {
        P = p;
        I = i;
        D = d;
        Error = 0.0;
        RegulatorOutput = 0.0;
    }
}


