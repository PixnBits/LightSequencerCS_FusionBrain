//////////////////////////////////////////////////////////////////////////////////
//	HardwareHandler.cs
//	Light Sequencer
//  ReWritten by Nick Oliver (http://www.UniquelyCommon.com/) as a go between
//  to accommodate other hardware (one file to edit) namely the Fusion Brain v3
//  (http://www.fusioncontrolcentre.com/) by Nicholas Vergunst and Tim Elmore
//  ((PWM supported in v4+, properties present but I don't have a board to test :)
//
//	Originally PhidgetHandler.cs, written by Brian Peek (http://www.brianpeek.com/)
//	for the Animated Holiday Lights article
//		at Coding4Fun (http://msdn.microsoft.com/coding4fun/)
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
//using Phidgets;
//using Phidgets.Events;
//using System.ComponentModel;
using FusionBrain;  //a namespace DLL with a FusionBrain3 class I wrote to simplify working
                    //with the FusionBrain board (handles the bitstream, detection, etc)
using System.Diagnostics;

namespace LightSequencer
{

    public static class HardwareHandler
    {

        // OLD hashtable mapping serial number to interface kit
        // OLD public static Dictionary<int, InterfaceKit> IFKits = new Dictionary<int,InterfaceKit>();

        private static lightChannel[] channels;
        private static FusionBrainCollection physicalBoards = new FusionBrainCollection(); //present, but untested

        // OLD private static Manager _phidgetsManager;

        // OLD public static event EventHandler PhidgetsChanged;
        public static event EventHandler HardwareChanged;

        public static void Init()
        {
            // OLD // create a new phidgets manager to find the devices connected
            //_phidgetsManager = new Manager();
            //_phidgetsManager.Attach += new AttachEventHandler(_phidgetsManager_Attach);
            //_phidgetsManager.Detach += new DetachEventHandler(_phidgetsManager_Detach);
            //_phidgetsManager.open();
            refreshHardware();
        }

        private static void refreshHardware()
        {
            // discover harware here, build a list of boards & channels
            //TODO

            // mockup, for simulation
            //physicalBoards = new FusionBrain3[1];
            physicalBoards.detectAll();
            //physicalBoards[0] = new FusionBrain3();
            channels = new lightChannel[physicalBoards.countOutputs()];

            for (int i = 0; i < channels.Length; i++)
            {
                channels[i] = new lightChannel();
                channels[i].setOutputChannel(i+1);
            }
        }

        public static void sync()
        {
            Debug.WriteLine("Send/Recieve(): "+physicalBoards.sendRecieve());
        }

        //public static int toggle(int channel)
        //{
        //    if ((channels.Length <= channel) || (channel < 0))
        //        return -1;
        //    Debug.WriteLine("toggling channel " + channel+" to something");
        //    //return toggle(channel, (physicalBoards[channels[channel].getBrainID()].getOutput(channels[channel].getOutputChannel()) == 0));
        //    return toggle(channel, !physicalBoards.getOutput(channel));
        //}

        public static int toggle(int channel, bool onOff)
        {
            if ((channels.Length <= channel) || (channel < 0))
            {
                Debug.WriteLine("attempt to toggle channel " + channel + " out of bounds (max " + channels.Length + ")");
                return -1;
            }
            Debug.WriteLine("toggling channel " + channel + "("+isOn(channel)+") to "+onOff);
            if (onOff)
            {
                return on(channel);
            }
            else
            {
                return off(channel);
            }
        }

        public static int on(int channel)
        {
            if ((channels.Length <= channel) || (channel < 0))
            {
                Debug.WriteLine("attempt to ON channel " + channel + ", out of bounds (max " + channels.Length + ")" + " with physical loc. of " + channels[channel].getOutputChannel());
                return -1;
            }
            Debug.WriteLine("turning on channel " + channel + " with physical loc. of " + channels[channel].getOutputChannel());
            //Debug.WriteLine("brainID " + channels[channel].getBrainID() +
            //    " | phys Chan " + channels[channel].getOutputChannel() +
            //    " | currently " + physicalBoards[channels[channel].getBrainID()].getOutput(channels[channel].getOutputChannel()));
            if (isOn(channel))
            {
                //Debug.WriteLine("successful ("+channel+")");
                return 1;
            }
            else
            {
                //Debug.WriteLine("failed (" + channel + ")");
                return physicalBoards.setOutputOn(channels[channel].getOutputChannel()) ? 1 : 0;
            }
        }

        public static int off(int channel)
        {
            if ((channels.Length <= channel) || (channel < 0))
            {
                Debug.WriteLine("attempt to OFF channel " + channel + ", out of bounds (max " + channels.Length + ")" + " with physical loc. of " + channels[channel].getOutputChannel());
                return -1;
            }
            Debug.WriteLine("turning off channel " + channel + " with physical loc. of " + channels[channel].getOutputChannel());
            //Debug.WriteLine("brainID " + channels[channel].getBrainID() +
            //    " | phys Chan " + channels[channel].getOutputChannel()+
            //    " | currently " + physicalBoards[channels[channel].getBrainID()].getOutput(channels[channel].getOutputChannel()));
            if (!isOn(channel))
            {
                //Debug.WriteLine("successful (" + channel + ")");
                return 1;
            }
            else
            {
                //Debug.WriteLine("failed (" + channel + ")");
                return physicalBoards.setOutputOff(channels[channel].getOutputChannel()) ? 1 : 0;
            }
        }

        public static void allOn()
        {
            for (int i = 0; i < channels.Length; i++)
            {
                on(i);
            }
        }

        public static void allOff()
        {
            for (int i = 0; i < channels.Length; i++)
            {
                off(i);
            }

        }

        public static int numOfChannels()
        {
            return channels.Length;
        }

        public static bool isOn(int channel)
        {
            if ((channels.Length <= channel) || (channel < 0))
            {
                Debug.WriteLine("attempt to check channel " + channel + ", out of bounds (max " + channels.Length + ")");
                return false;
            }
            //Debug.WriteLine("channel " + channel + "(" + channels[channel].getOutputChannel() + ") is at " + physicalBoards.outputPortLocation(channels[channel].getOutputChannel())[0] + "." + physicalBoards.outputPortLocation(channels[channel].getOutputChannel())[1]);
            Debug.WriteLine("Length of fb3List: " + physicalBoards.fb3List.Count);
            return physicalBoards.getOutput(channels[channel].getOutputChannel());
            //return (physicalBoards[channels[channel].getBrainID()].getOutput(channels[channel].getOutputChannel()) > 0);
        }
    }

    /*
	public static class HardwareHandlerOLD
	{

        // OLD hashtable mapping serial number to interface kit
        // OLD public static Dictionary<int, InterfaceKit> IFKits = new Dictionary<int,InterfaceKit>();

        private static lightChannel[] channels;
        //private static FusionBrainCollection physicalBoards; //present, but untested
        private static FusionBrain3[] physicalBoards; //shortcut for time :S

        // OLD private static Manager _phidgetsManager;

        // OLD public static event EventHandler PhidgetsChanged;
        public static event EventHandler HardwareChanged;

        public static void Init()
		{
            // OLD // create a new phidgets manager to find the devices connected
            //_phidgetsManager = new Manager();
            //_phidgetsManager.Attach += new AttachEventHandler(_phidgetsManager_Attach);
            //_phidgetsManager.Detach += new DetachEventHandler(_phidgetsManager_Detach);
            //_phidgetsManager.open();
            refreshHardware();
		}

        private static void refreshHardware()
        {
            // discover harware here, build a list of boards & channels
            //TODO

            // mockup, for simulation
            physicalBoards = new FusionBrain3[1];
            physicalBoards[0] = new FusionBrain3();
            channels = new lightChannel[12];
            
            for (int i = 0; i < channels.Length; i++)
            {
                channels[i] = new lightChannel();
                channels[i].setOutputChannel(i);
            }
        }

        public static void sync()
        {
            foreach (FusionBrain3 board in physicalBoards)
            {
                board.SendRecieve();
            }
        }

        public static int toggle(int channel)
        {
            if((channels.Length < channel) || (channel < 0))
                   return -1;
            //Debug.WriteLine("toggling channel " + channel+" to something");
            return toggle(channel, (physicalBoards[channels[channel].getBrainID()].getOutput(channels[channel].getOutputChannel()) == 0));
        }

        public static int toggle(int channel, bool onOff)
        {
            if ((channels.Length < channel) || (channel < 0))
            {
                Debug.WriteLine("attempt to toggle channel " + channel + " out of bounds (max "+channels.Length+")");
                return -1;
            }
            //Debug.WriteLine("toggling channel " + channel + " to "+onOff);
            if (onOff)
            {
                return on(channel);
            }
            else
            {
                return off(channel);
            }
        }

        public static int on(int channel)
        {
            if ((channels.Length < channel) || (channel < 0))
            {
                Debug.WriteLine("attempt to ON channel " + channel + ", out of bounds (max " + channels.Length + ")");
                return -1;
            }
            //Debug.WriteLine("turning on channel " + channel);
            //Debug.WriteLine("brainID " + channels[channel].getBrainID() +
            //    " | phys Chan " + channels[channel].getOutputChannel() +
            //    " | currently " + physicalBoards[channels[channel].getBrainID()].getOutput(channels[channel].getOutputChannel()));
            if (physicalBoards[channels[channel].getBrainID()].setOutputOn(channels[channel].getOutputChannel()) >= 0)
            {
                //Debug.WriteLine("successful ("+channel+")");
                return 1;
            }
            else
            {
                //Debug.WriteLine("failed (" + channel + ")");
                return 0;
            }
        }

        public static int off(int channel)
        {
            if ((channels.Length < channel) || (channel < 0))
            {
                Debug.WriteLine("attempt to OFF channel " + channel + ", out of bounds (max " + channels.Length + ")");
                return -1;
            }
            //Debug.WriteLine("turning off channel " + channel);
            //Debug.WriteLine("brainID " + channels[channel].getBrainID() +
            //    " | phys Chan " + channels[channel].getOutputChannel()+
            //    " | currently " + physicalBoards[channels[channel].getBrainID()].getOutput(channels[channel].getOutputChannel()));
            if (physicalBoards[channels[channel].getBrainID()].setOutputOff(channels[channel].getOutputChannel()) != 0)
            {
                //Debug.WriteLine("successful (" + channel + ")");
                return 1;
            }
            else
            {
                //Debug.WriteLine("failed (" + channel + ")");
                return 0;
            }
        }

        public static void allOn()
        {
            for (int i = 0; i < channels.Length; i++)
            {
                on(i);
            }
        }

        public static void allOff()
        {
            for (int i = 0; i < channels.Length; i++)
            {
                off(i);
            }
            
        }

        public static int numOfChannels()
        {
            return channels.Length;
        }

        public static bool isOn(int channel)
        {
            if ((channels.Length < channel) || (channel < 0))
            {
                Debug.WriteLine("attempt to check channel " + channel + ", out of bounds (max " + channels.Length + ")");
                return false;
            }
            return (physicalBoards[channels[channel].getBrainID()].getOutput(channels[channel].getOutputChannel()) > 0);
        }
    }
    //*/

    public class lightChannel
    {
        protected bool  hasPWM  = false;
        protected int   PWM     = 0;
        protected int   maxPWM  = 1023;
        protected int   minPWM  = 0;
        
        // Hardware specific variables
        //protected FuBrain hardware = new FuBrain();
        private int brainID = 0;// hardware.brainID
        protected int outputChannel = 0; //channel on the board

        public void setOutputChannel(int physicalChannel)
        {
            this.outputChannel = physicalChannel;
        }

        public int getOutputChannel()
        {
            return this.outputChannel;
        }

        public void setOutputHardwareID(int physicalID)
        {
            //this.hardware.brainID = physicalID;
            this.brainID = physicalID;
        }

        public int getBrainID()
        {
            return this.brainID;
        }

        public void setPWM(int level)
        {
            this.PWM = level;
            checkPWM();
        }

        public int setPWM(float percent)
        {
            // returns the integer value 
            setPWM((int)(percent * this.maxPWM));
            return this.PWM;
        }

        public void enablePWM()
        {
            this.hasPWM = true;
            checkPWM();
        }

        public void disablePWM()
        {
            this.hasPWM = false;
        }

        public void setMaxPWM(int maxFrequency)
        {
            this.maxPWM = maxFrequency;
            checkPWM();
        }

        public void setMinPWM(int minFrequency)
        {
            this.minPWM = minFrequency;
            checkPWM();
        }

        private void checkPWM()
        {
            // ensure that PWM level remains between min/max
            if (this.PWM > this.maxPWM)
                this.PWM = this.maxPWM;
            if (this.PWM < this.minPWM)
                this.PWM = this.minPWM;
            // set the physical level
            //this.hardware.setPWM(this.PWM);
        }
    }
}
