using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThingMagic;
using System.Threading;
using System.Diagnostics;
using System.Windows.Threading;

namespace Em4325GUIdemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        int xInt = 0;
        int yInt = 0;
        int zInt = 0;


        String bufferX;
        String bufferY;
        String bufferZ;
        private static System.Timers.Timer aTimer;


        public MainWindow()
        {
            InitializeComponent();
           
        }

    

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            startButton.Content = "Connected";
            defaultSettings();
            
        }

        public void defaultSettings()
        {
            
            String x;
            String y;
            String z;


            int[] antennaList = null;
            TagFilter filter0;
            filter0 = new TagData("AA");
            Reader r = Reader.Create("tmr:///com25");
           
                r.Connect();
                
                TagOp op = new Gen2.ReadData(Gen2.Bank.USER, 0x104, 3);
                SimpleReadPlan plan = new SimpleReadPlan(antennaList, TagProtocol.GEN2, filter0, op, true, 150);

            r.ParamSet("/reader/gen2/blf", Gen2.LinkFrequency.LINK640KHZ);
            //  r.ParamSet("/reader/gen2/target", Gen2.Target.AB);
            //  MultiReadPlan testMultiReadPlan = new MultiReadPlan(readPlans);
            //  r.ParamSet("/reader/read/plan", testMultiReadPlan);
            //r.ParamSet("/reader/read/asyncOnTime", 25000);
            //r.ParamSet("/reader/read/asyncOffTime", 200);
            r.ParamSet("/reader/region/id", Reader.Region.EU3);
            Gen2.BAPParameters bap = (Gen2.BAPParameters)r.ParamGet("/reader/gen2/bap");
            bap.FREQUENCYHOPOFFTIME = 20;
            bap.POWERUPDELAY = 3;
            r.ParamSet("/reader/gen2/bap", bap);
            // r.ParamSet("/reader/baudRate", 921600);

            r.ParamSet("/reader/read/plan", plan);


          
            r.TagRead += delegate (Object sender, TagReadDataEventArgs e)
                {
                    

                    //Debug.Print("I am here");
                    String userMem = ByteFormat.ToHex(e.TagReadData.Data).ToString();  // convert the input into a hex string
                   // Debug.Print(userMem);
                    String abc = e.TagReadData.EpcString;
                    Debug.Print(abc);
                    userMem.Trim();

                    int length = 4;
                    int substringposition = 2;


                    if (abc == "AAA2")
                    {
                        while (substringposition < 3)  // split the string into small chunks per value and convert to INT
                        {
                            Debug.Print("I am in this loop");
                            x = userMem.Substring(substringposition, length);
                           
                            xInt = Convert.ToInt32(x, 16);
                            
                            substringposition = substringposition + length;
                            bufferX = xInt.ToString();
                            Debug.Print(bufferX);
                            y = userMem.Substring(substringposition, length);
                            yInt = Convert.ToInt32(y, 16);
                            substringposition = substringposition + length;
                            bufferY = yInt.ToString();

                            z = userMem.Substring(substringposition, length);
                            zInt = Convert.ToInt32(z, 16);
                            substringposition = substringposition + length;
                            bufferZ = zInt.ToString();
                            //updateText(xInt, yInt, zInt);
                            Debug.Print(x + " " +  y + " " +  z);
                           // updateProgressBars(xInt, yInt, zInt);

                           
                        }
                    }

                    Action action = delegate ()
                    {
                        updateText();
                        updateProgressBars();

                    };
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
                };

                r.StartReading();

                //   Console.WriteLine("\r\n<Do other work here>\r\n");
               // Thread.Sleep(50000000);

              //  r.Dispose();

            

            
       
        }


        private void updateText()
        {
           
            x1.Content = bufferX;
            y1.Content = bufferY;
            z1.Content = bufferZ;


        }



        public void updateProgressBars()
        {






                xBar.Value = xInt;
                yBar.Value = yInt;
                zBar.Value = zInt;
            
                
            
            

        }

        private void Button_Click_Stop(object sender, RoutedEventArgs e)
        {

        }


        private static void r_ReadException(object sender, ReaderExceptionEventArgs e)
        {
            Console.WriteLine("Error: " + e.ReaderException.Message);
        }


        #region ParseAntennaList

        private static int[] ParseAntennaList(IList<string> args, int argPosition)
        {
            int[] antennaList = null;
            try
            {
                string str = args[argPosition + 1];
                antennaList = Array.ConvertAll<string, int>(str.Split(','), int.Parse);
                if (antennaList.Length == 0)
                {
                    antennaList = null;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Missing argument after args[{0:d}] \"{1}\"", argPosition, args[argPosition]);

            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}\"{1}\"", ex.Message, args[argPosition + 1]);

            }
            return antennaList;
        }

        #endregion
    }

    public static class ExtensionMethods
    {
        public static decimal Map(this decimal value, decimal fromSource, decimal toSource, decimal fromTarget, decimal toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }
    }
}
