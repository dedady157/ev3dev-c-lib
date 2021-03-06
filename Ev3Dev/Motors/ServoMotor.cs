﻿using System;
using System.Threading;

namespace Ev3DevLib.Motors
{
    //Last Updated on 5.4.2018 (DD/MM/YYYY)
    public enum ServoMotor_Commands
    {
        unknown = 0,//deffult
        rm,
        _float,
    }
    public enum ServoMotor_Polarity
    {
        normal,
        inversed,
    }
    public enum ServoMotor_States
    {
        running,
    }
    public enum ServoMotor_Args
    {
        rate_sp,
        position_sp,
        min_pulse_sp,
        mid_pulse_sp,
        max_pulse_sp,
    }
    //--------------------------------

    public class ServoMotor : OutPort
    {
        //values
        //my Rule not matter what all of the values must be updated inside UPDATE
        //unless there NON-READ ie 'onlywriteable'(w)
        public string Address                       { get { try { return ReadVar("address"); } catch { return "N/A"; } } }
        public ServoMotor_Commands LastCommand      { get; private set; }//NON-READ
        public string DriverName                    { get { try { return ReadVar("driver_name"); } catch { return "N/A"; } } }
        public int MaxPulse                         { get { try { return int.Parse(ReadVar("max_pulse_sp")); } catch { return -1; } } }
        public int MidPulse                         { get { try { return int.Parse(ReadVar("mid_pulse_sp")); } catch { return -1; } } }
        public int MinPulse                         { get { try { return int.Parse(ReadVar("min_pulse_sp")); } catch { return -1; } } }
        public ServoMotor_Polarity Polarity         { get { try { return String_To_ServoMotor_Polarity(ReadVar("polarity")); } catch { return ServoMotor_Polarity.normal; } } }
        public int Rate                             { get { try { return int.Parse(ReadVar("rate_sp")); } catch { return -1; } } }
        public bool RateSupported                   { get { try { return (ReadVar("rate_sp") == "-EOPNOTSUPP") ? false : true; } catch { return false; } } }
        public int Position                         { get { try { return int.Parse(ReadVar("position_sp")); } catch { return 0; } } }
        public ServoMotor_States State              { get { try { return String_To_ServoMotor_States(ReadVar("state")); } catch { return ServoMotor_States.running; } } }
        

        //my header info
        public string RootToDir { get; private set; }
        public string MountPoint { get; private set; }
        public string[] _Options;
        public override string[] Options => _Options;

        //base functions
        private string ReadVar(string var)
        {
            return IO.ReadValue(RootToDir + "/" + var);
        }
        private void WriteVar(string var, string value)
        {
            IO.WriteValue(RootToDir + "/" + var, value);
        }

        //constructorpublic
        public ServoMotor(Device dev) : base(dev)
        {
            if (dev._type != DeviceType.servo_motor)
                throw new InvalidOperationException("this device is not a tachno motor");

            RootToDir = dev.RootToDir;
            if (RootToDir.Contains("servo-motor"))
                MountPoint = "??";//ReadVar("address");
            else if (RootToDir.Contains(":"))
                MountPoint = RootToDir;
            else throw new InvalidOperationException("this uses the wrong class please re initulize the device and then try agen");
            _Options = new string[] {
                    "MoveTo", "FLOAT", "Address", "LastCommand", "DriverName", "MaxPulse", "MidPulse", "MinPulse", "Polarity", "Rate", "RateSupported", "Position", "State"
                };
        }

        //helpers
        public string ServoMotor_Commands_To_String(ServoMotor_Commands x)
        {
            switch(x)
            {
                case (ServoMotor_Commands.rm):
                    return "rm";
                case (ServoMotor_Commands._float):
                    return "float";
                default:
                    return "INVALID";
            }
        }
        public string ServoMotor_Polarity_To_String(ServoMotor_Polarity x)
        {
            switch (x)
            {
                case (ServoMotor_Polarity.inversed):
                    return "inversed";
                case (ServoMotor_Polarity.normal):
                    return "normal";
                default:
                    return "INVALID";
            }
        }
        public string ServoMotor_States_To_String(ServoMotor_States x)
        {
            switch (x)
            {
                case (ServoMotor_States.running):
                    return "running";
                default:
                    return "INVALID";
            }
        }
        public string ServoMotor_Args_To_String(ServoMotor_Args x)
        {
            switch (x)
            {
                case (ServoMotor_Args.max_pulse_sp):
                    return "max_pulse_sp";
                case (ServoMotor_Args.mid_pulse_sp):
                    return "mid_pulse_sp";
                case (ServoMotor_Args.min_pulse_sp):
                    return "min_pulse_sp";
                case (ServoMotor_Args.position_sp):
                    return "position_sp";
                case (ServoMotor_Args.rate_sp):
                    return "rate_sp";
                default:
                    return "INVALID";
            }
        }

        public ServoMotor_Commands String_To_ServoMotor_Commands(string x)
        {
            switch (x)
            {
                case ("rm"):
                    return ServoMotor_Commands.rm;
                case ("_float"):
                    return ServoMotor_Commands._float;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public ServoMotor_Polarity String_To_ServoMotor_Polarity(string x)
        {
            switch (x)
            {
                case ("inversed"):
                    return ServoMotor_Polarity.inversed;
                case ("normal"):
                    return ServoMotor_Polarity.normal;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public ServoMotor_States String_To_ServoMotor_States(string x)
        {
            switch (x)
            {
                case ("running"):
                    return ServoMotor_States.running;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public ServoMotor_Args String_To_ServoMotor_Args(string x)
        {
            switch (x)
            {
                case ("max_pulse_sp"):
                    return ServoMotor_Args.max_pulse_sp;
                case ("mid_pulse_sp"):
                    return ServoMotor_Args.mid_pulse_sp;
                case ("min_pulse_sp"):
                    return ServoMotor_Args.min_pulse_sp;
                case ("position_sp"):
                    return ServoMotor_Args.position_sp;
                case ("rate_sp"):
                    return ServoMotor_Args.rate_sp;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //functions
        //**i dont know how to use these motors so if anyone wants ot make some more easyer to use function please do so
        public void MoveTo(int Position)
        {
            if (TestArg(Position, ServoMotor_Args.position_sp))
            {
                WriteVar("position_sp", Position.ToString());
                WriteVar("command", "run");
            }
        }
        public void FLOAT()
        {
            WriteVar("command", "float");
        }

        //safty functions
        private bool TestArg(string value, ServoMotor_Args forArg)
        {
            return TestArg(int.Parse(value), forArg);
        }
        private bool TestArg(int value, ServoMotor_Args forArg)
        {
            switch (forArg)
            {
                case (ServoMotor_Args.max_pulse_sp):
                    if (value > 2700) return false;
                    if (value < 2300) return false;
                    return true;

                case (ServoMotor_Args.mid_pulse_sp):
                    if (value > 1700) return false;
                    if (value < 1300) return false;
                    return true;

                case (ServoMotor_Args.min_pulse_sp):
                    if (value > 700) return false;
                    if (value < 300) return false;
                    return true;

                case (ServoMotor_Args.position_sp):
                    if (value > 100) return false;
                    if (value < -100) return false;
                    return true;

                case (ServoMotor_Args.rate_sp):
                    if (value < 0) return false;
                    if (value > 1000) return false;
                    return true;
            }
            return false;//should never happen
        }

        //hands on for more advanced users
        public void SetCommand(ServoMotor_Commands x)
        {
            WriteVar("command", ServoMotor_Commands_To_String(x));
        }
        public void SetPolarity(ServoMotor_Polarity x)
        {
            WriteVar("polarity", ServoMotor_Polarity_To_String(x));
        }
        public void SetArg(int value, ServoMotor_Args x)
        {
            if(TestArg(value,x))
            {
                WriteVar(ServoMotor_Args_To_String(x), value.ToString());
            }
        }
        public void SetArg(string value, ServoMotor_Args x)
        {
            if (TestArg(value, x))
            {
                WriteVar(ServoMotor_Args_To_String(x), value);
            }
        }

        public override void ExecuteWriteOption(string Option, string[] Args)
        {
            switch (Option)
            {
                case ("MoveTo"):
                    MoveTo(int.Parse(Args[0]));
                    break;

                case ("FLOAT"):
                    FLOAT();
                    break;

                case ("Address"):
                    throw new InvalidOperationException("ReadOnly");

                case ("LastCommand"):
                    throw new InvalidOperationException("ReadOnly");

                case ("DriverName"):
                    throw new InvalidOperationException("ReadOnly");

                case ("MaxPulse"):
                    SetArg(int.Parse(Args[0]), ServoMotor_Args.max_pulse_sp);
                    break;

                case ("MidPulse"):
                    SetArg(int.Parse(Args[0]), ServoMotor_Args.mid_pulse_sp);
                    break;

                case ("MinPulse"):
                    SetArg(int.Parse(Args[0]), ServoMotor_Args.min_pulse_sp);
                    break;

                case ("Polarity"):
                    SetPolarity(String_To_ServoMotor_Polarity(Args[0]));
                    break;

                case ("Rate"):
                    SetArg(int.Parse(Args[0]), ServoMotor_Args.rate_sp);
                    break;

                case ("RateSupported"):
                    throw new InvalidOperationException("ReadOnly");

                case ("State"):
                    throw new InvalidOperationException("ReadOnly");

                case ("Command"):
                    SetCommand(String_To_ServoMotor_Commands(Args[0]));
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public override string ExecuteReadOption(string Option)
        {
            switch (Option)
            {
                case ("MoveTo"):
                    throw new InvalidOperationException("Executeable");

                case ("FLOAT"):
                    throw new InvalidOperationException("Executeable");

                case ("Address"):
                    throw new InvalidOperationException("ReadOnly");

                case ("LastCommand"):
                    return ServoMotor_Commands_To_String(LastCommand);

                case ("DriverName"):
                    return DriverName;

                case ("MaxPulse"):
                    return MaxPulse.ToString();

                case ("MidPulse"):
                    return MidPulse.ToString();
                    
                case ("MinPulse"):
                    return MinPulse.ToString();
                    
                case ("Polarity"):
                    return ServoMotor_Polarity_To_String(Polarity);

                case ("Rate"):
                    return Rate.ToString();
                    
                case ("RateSupported"):
                    return RateSupported ? "True" : "False";

                case ("State"):
                    return ServoMotor_States_To_String(State);

                case ("Command"):
                    throw new InvalidOperationException("WriteOnly");

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
