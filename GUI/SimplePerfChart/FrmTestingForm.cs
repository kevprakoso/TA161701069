using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using GMap.NET;
using TweetSharp;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.MapProviders;
using Newtonsoft.Json;
using System.IO;
using System.Globalization;

namespace SimplePerfChart
{
  

    public partial class FrmTestingForm : Form
    {
        private object valueGenSync = new object();
        private Random randGen = new Random();
        //public static decimal data = 0;
        private int valueGenFrom = -5;
        private int valueGenTo = 5;
        private int valueGenTimerFrom = 100;
        private int valueGenTimerTo = 1000;
        public static string data = "";
        public static double defx;
        public static double defy;
        public static double defz;
        public static string quake_status;

        public struct accel
        {
            public decimal accel_x;
            public decimal accel_y;
            public decimal accel_z;
        }
        public FrmTestingForm()
        {
            InitializeComponent();
            string key = "agOz9gCZATM58FI3ubCubyuot";
            string secret = "57m1Xh5SGyuogn4zttjFpz6SHwRYBA9mpJdPNJHbXPoisf9KGn";
            string token = "860367448268087296-yahVsCIdXOyDQzs7R9to8ITUurBaHHA";
            string tokenSecret = " wrGpbaIF98cGPXYuXZxMm0BYDLoyb7w4hq3GA2q5JzGhT";
          

            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.Font = SystemInformation.MenuFont;
            textBox4.Text = "";

            propGrid.SelectedObject = perfChart.PerfChartStyle;

            // Apply default Properties
            perfChart.TimerInterval = 1000;
            perfChart1.TimerInterval = 1000;
            perfChart2.TimerInterval = 1000;

            // Populate DrowDown Boxes
            foreach (String item in System.Enum.GetNames(typeof(Border3DStyle)))
            {
                cmbBxBorder.Items.Add(item);
            }
            foreach (String item in System.Enum.GetNames(typeof(SpPerfChart.ScaleMode)))
            {
                cmbBxScaleMode.Items.Add(item);
            }
            foreach (String item in System.Enum.GetNames(typeof(SpPerfChart.TimerMode)))
            {
                cmbBxTimerMode.Items.Add(item);
            }

            // Select default values
            cmbBxTimerMode.SelectedItem = perfChart.TimerMode.ToString();
            cmbBxTimerMode.SelectedItem = perfChart1.TimerMode.ToString();
            cmbBxTimerMode.SelectedItem = perfChart2.TimerMode.ToString();

            cmbBxScaleMode.SelectedItem = perfChart.ScaleMode.ToString();
            cmbBxScaleMode.SelectedItem = perfChart1.ScaleMode.ToString();
            cmbBxScaleMode.SelectedItem = perfChart2.ScaleMode.ToString();

            cmbBxBorder.SelectedItem = perfChart.BorderStyle.ToString();
            cmbBxBorder.SelectedItem = perfChart1.BorderStyle.ToString();
            cmbBxBorder.SelectedItem = perfChart2.BorderStyle.ToString();


        }

        private void tabControl1_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            Font f;
            Brush backBrush;
            Brush foreBrush;

            if (e.Index == this.tabControl1.SelectedIndex)
            {
                f = new Font(e.Font, FontStyle.Italic | FontStyle.Bold);
                backBrush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.Blue, Color.Red, System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal);
                foreBrush = Brushes.PowderBlue;
            }
            else
            {
                f = e.Font;
                backBrush = new SolidBrush(e.BackColor);
                foreBrush = new SolidBrush(e.ForeColor);
            }

            string tabName = this.tabControl1.TabPages[e.Index].Text;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            e.Graphics.FillRectangle(backBrush, e.Bounds);
            Rectangle r = e.Bounds;
            r = new Rectangle(r.X, r.Y + 3, r.Width, r.Height - 3);
            e.Graphics.DrawString(tabName, f, foreBrush, r, sf);

            sf.Dispose();
            if (e.Index == this.tabControl1.SelectedIndex)
            {
                f.Dispose();
                backBrush.Dispose();
            }
            else
            {
                backBrush.Dispose();
                foreBrush.Dispose();
            }
        }



        private void chkBxTimerEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBxTimerEnabled.Checked && !bgWrkTimer.IsBusy)
            {
                RunTimer();
            }
            if (chkBxTimerEnabled.Checked && !backgroundWorker1.IsBusy)
            {
                RunTimer1();
            }
            if (chkBxTimerEnabled.Checked && !backgroundWorker2.IsBusy)
            {
                RunTimer2();
            }
            if (chkBxTimerEnabled.Checked && !backgroundWorker3.IsBusy)
            {
                RunTimer3();
            }
            if (chkBxTimerEnabled.Checked && !backgroundWorker4.IsBusy)
            {
                RunTimer4();
            }
            if (chkBxTimerEnabled.Checked && !backgroundWorker5.IsBusy)
            {
                RunTimer5();
            }
            if (chkBxTimerEnabled.Checked && !backgroundWorker6.IsBusy)
            {
                RunTimer6();
            }
        }

        private void RunTimer()
        {
            int waitFor = randGen.Next(valueGenTimerFrom, valueGenTimerTo);
            bgWrkTimer.RunWorkerAsync(waitFor);
        }

        private void RunTimer1()
        {
            int waitFor = randGen.Next(valueGenTimerFrom, valueGenTimerTo);
            backgroundWorker1.RunWorkerAsync(waitFor);                 
        }

        private void RunTimer2()
        {
            int waitFor = randGen.Next(valueGenTimerFrom, valueGenTimerTo);
            backgroundWorker2.RunWorkerAsync(waitFor);
        }

        private void RunTimer3()
        {
            int waitFor = randGen.Next(valueGenTimerFrom, valueGenTimerTo);
            backgroundWorker3.RunWorkerAsync(waitFor);
        }

        private void RunTimer4()
        {
            int waitFor = randGen.Next(valueGenTimerFrom, valueGenTimerTo);
            backgroundWorker4.RunWorkerAsync(waitFor);
        }

        private void RunTimer5()
        {
            int waitFor = randGen.Next(valueGenTimerFrom, valueGenTimerTo);
            backgroundWorker5.RunWorkerAsync(waitFor);
        }

        private void RunTimer6()
        {
            int waitFor = randGen.Next(valueGenTimerFrom, valueGenTimerTo);
            backgroundWorker6.RunWorkerAsync(waitFor);
        }
            

        private void perfchart_pick(data pick, string queue)
        {            
            switch (queue)
            {
                case "ecn":
                    perfchart_fill1(pick, queue);
                    break;
                case "ecn1":
                    perfchart_fill2(pick, queue);
                    break;
                case "ecn2":
                    perfchart_fill3(pick, queue);
                    break;
                case "ecn3":
                    perfchart_fill4(pick, queue);
                    break;
                case "ecn4":
                    perfchart_fill5(pick, queue);
                    break;
                case "ecn5":
                    perfchart_fill6(pick, queue);
                    break;

                default:
                    break;
            }
        }


        private void perfchart_fill1(data accelReport, string queue)
        {
            double resultx;
            double resulty;
            double resultz;
            string key = "agOz9gCZATM58FI3ubCubyuot";
            string secret = "57m1Xh5SGyuogn4zttjFpz6SHwRYBA9mpJdPNJHbXPoisf9KGn";
            string token = "860367448268087296-yahVsCIdXOyDQzs7R9to8ITUurBaHHA";
            string tokenSecret = " wrGpbaIF98cGPXYuXZxMm0BYDLoyb7w4hq3GA2q5JzGhT";
            var service = new TweetSharp.TwitterService(key, secret);
            service.AuthenticateWith(token, tokenSecret);
            string message = "Peringatan telah terjadi gempa susulan dengan skala 6,3 dengan pusat gempa di lokasi Bandung";
            for (int i = 0; i < 40; i++)
            {
                resultx = ((double.Parse(accelReport.accelerations[i].x)));
                resulty = ((double.Parse(accelReport.accelerations[i].y)));
                resultz = ((double.Parse(accelReport.accelerations[i].z)));

                perfChart.AddValue((decimal)resultx);                
                perfChart1.AddValue((decimal)resulty);                
                perfChart2.AddValue((decimal)resultz);
                if (resultx > 9 || resulty > 9 || resultz > 9)
                {
                    //MessageBox.Show("TERJADI GEMPA");

                    textBox4.BeginInvoke(new Action(() => { textBox4.Text = "GEMPA"; }));
                    var result = service.SendTweet(new SendTweetOptions
                    {
                        Status = message
                    }
);
                    //lblResult.Text = result.Text.ToString();

                }
                Thread.Sleep(25);                                            

                

                //defx = double.Parse(accelReport.accelerations[i].x);
                //defy = double.Parse(accelReport.accelerations[i].y);
                //defz = double.Parse(accelReport.accelerations[i].z);


            }            
        }

        private void perfchart_fill2(data accelReport, string queue)
        {
            for (int i = 0; i < 40; i++)
            {
                perfChart5.AddValue((decimal)(double.Parse(accelReport.accelerations[i].x)) + 20);
                Thread.Sleep(13);
                //textBox3.Text = accelReport.accelerations[i].x;
                //perfChart3.AddValue(20);
                Thread.Sleep(12);
                perfChart4.AddValue((decimal)(double.Parse(accelReport.accelerations[i].y)) + 20);
                Thread.Sleep(13);
                //textBox1.Text = accelReport.accelerations[i].y;
                //perfChart3.AddValue(20);
                Thread.Sleep(12);
                perfChart3.AddValue((decimal)(double.Parse(accelReport.accelerations[i].z)) + 20);
                Thread.Sleep(13);
                //textBox2.Text = accelReport.accelerations[i].z;
                //perfChart3.AddValue(20);
                Thread.Sleep(12);
            }
        }


        private void perfchart_fill3(data accelReport, string queue)
        {
            for (int i = 0; i < 40; i++)
            {
                perfChart3.AddValue((decimal)(double.Parse(accelReport.accelerations[i].x)) + 20);
                Thread.Sleep(13);
                //textBox3.Text = accelReport.accelerations[i].x;
                //perfChart3.AddValue(20);
                Thread.Sleep(12);
                perfChart7.AddValue((decimal)(double.Parse(accelReport.accelerations[i].y)) + 20);
                Thread.Sleep(13);
                //textBox1.Text = accelReport.accelerations[i].y;
                //perfChart3.AddValue(20);
                Thread.Sleep(12);
                perfChart8.AddValue((decimal)(double.Parse(accelReport.accelerations[i].z)) + 20);
                Thread.Sleep(13);
                //textBox2.Text = accelReport.accelerations[i].z;
                //perfChart3.AddValue(20);
                Thread.Sleep(12);
            }
        }


        private void perfchart_fill4(data accelReport, string queue)
        {
            for (int i = 0; i < 40; i++)
            {
                perfChart11.AddValue((decimal)(double.Parse(accelReport.accelerations[i].x)) + 20);
                Thread.Sleep(13);
                //textBox3.Text = accelReport.accelerations[i].x;
                //perfChart3.AddValue(20);
                Thread.Sleep(12);
                perfChart10.AddValue((decimal)(double.Parse(accelReport.accelerations[i].y)) + 20);
                Thread.Sleep(13);
                //textBox1.Text = accelReport.accelerations[i].y;
                //perfChart3.AddValue(20);
                Thread.Sleep(12);
                perfChart11_.AddValue((decimal)(double.Parse(accelReport.accelerations[i].z)) + 20);
                Thread.Sleep(13);
                //textBox2.Text = accelReport.accelerations[i].z;
                //perfChart3.AddValue(20);
                Thread.Sleep(12);
            }
        }


        private void perfchart_fill5(data accelReport, string queue)
        {
            for (int i = 0; i < 40; i++)
            {
                perfChart12.AddValue((decimal)(double.Parse(accelReport.accelerations[i].x)) + 20);
                Thread.Sleep(13);
                //textBox3.Text = accelReport.accelerations[i].x;
                //perfChart3.AddValue(20);
                Thread.Sleep(12);
                perfChart13.AddValue((decimal)(double.Parse(accelReport.accelerations[i].y)) + 20);
                Thread.Sleep(13);
                //textBox1.Text = accelReport.accelerations[i].y;
                //perfChart3.AddValue(20);
                Thread.Sleep(12);
                perfChart14.AddValue((decimal)(double.Parse(accelReport.accelerations[i].z)) + 20);
                Thread.Sleep(13);
                //textBox2.Text = accelReport.accelerations[i].z;
                //perfChart3.AddValue(20);
                Thread.Sleep(12);
            }
        }


        private void perfchart_fill6(data accelReport, string queue)
        {
            for (int i = 0; i < 40; i++)
            {
                perfChart15.AddValue((decimal)(double.Parse(accelReport.accelerations[i].x)) + 20);
                Thread.Sleep(13);
                //textBox3.Text = accelReport.accelerations[i].x;
                //perfChart3.AddValue(20);
                Thread.Sleep(12);
                perfChart16.AddValue((decimal)(double.Parse(accelReport.accelerations[i].y)) + 20);
                Thread.Sleep(13);
                //textBox1.Text = accelReport.accelerations[i].y;
                //perfChart3.AddValue(20);
                Thread.Sleep(12);
                perfChart17.AddValue((decimal)(double.Parse(accelReport.accelerations[i].z)) + 20);
                Thread.Sleep(13);
                //textBox2.Text = accelReport.accelerations[i].z;
                //perfChart3.AddValue(20);
                Thread.Sleep(12);
            }
        }


        private void connect_msgserver(string key)
        {
            //consume_data();
            ConnectionFactory factory;
            using (StreamReader r = new StreamReader("config1.json"))
            {
                string json = r.ReadToEnd();
                Config config = JsonConvert.DeserializeObject<Config>(json);

                factory = new ConnectionFactory();// { HostName = config.host, UserName = config.user, VirtualHost = config.vhost, Password = config.password };                              
                factory.Uri = "amqp://sensor_gempa:12345@167.205.7.226/%2fdisaster";
            }

            factory.Protocol = Protocols.DefaultProtocol;
            factory.Port = AmqpTcpEndpoint.UseDefaultPort;

            //SENSOR ecn
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.BasicQos(0, 1, false);
                channel.QueueDeclare(queue: key , //"emergency_gui",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                channel.QueueBind(queue: key, //"emergency_gui",
                                     exchange: "amq.topic",
                                     routingKey: "amq.topic." + key //emergency"
                                     );

                Console.WriteLine("Queue Declare Emergency GUI");
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    //Console.WriteLine(" [x] Received {0}", message);
                    //Console.WriteLine(" ///////////////////////////////////////////////////////////////////////////////////////////////////");
                    data = message;
                    //accel_data = ParsingMessage(data);
                    try
                    {
                        //AccelerationReport accelReport = JsonConvert.DeserializeObject<AccelerationReport>(message);
                        data accelReport = JsonConvert.DeserializeObject<data>(message);
                        //Console.WriteLine("{0} {1} {2}", accel_data.accel_x, accel_data.accel_y, accel_data.accel_z);
                        //console.writeline("{0} {1} {2} {3}", accelreport.geometry.geometry.coordinates, accelreport.accelerations[0].x, accelreport.accelerations[0].y, accelreport.accelerations[0].z);
                        //perfchart.addvalue((decimal)accelreport.accelerations[0].x * 1000);
                        //perfchart1.addvalue((decimal)accelreport.accelerations[0].y * 1000);
                        //perfchart2.addvalue((decimal)accelreport.accelerations[0].z * 1000);
                        //Console.WriteLine("{0} {1} {2} {3} {4}", accelReport.geometry.geometryData.coordinate.lon, accelReport.geometry.geometryData.coordinate.lng, accelReport.accelerations.x, accelReport.accelerations.y, accelReport.accelerations.z);                        Console.WriteLine(accelReport.geojson.geometry.coordinates[0]);

                        Console.WriteLine(accelReport.pointTime);
                        Console.WriteLine(accelReport.timeZone);
                        Console.WriteLine(accelReport.interval);
                        Console.WriteLine(accelReport.clientID);
                        Console.WriteLine(accelReport.geojson.geometry.coordinates[0]);
                        Console.WriteLine(accelReport.geojson.geometry.coordinates[1]);
                        Console.WriteLine(accelReport.accelerations[0].x);
                        Console.WriteLine(accelReport.accelerations[0].y);
                        Console.WriteLine(accelReport.accelerations[0].z);
                        perfchart_pick(accelReport,key);
                        textBox4.Text = quake_status;
                        
                        gMapControl1.Position = new GMap.NET.PointLatLng(double.Parse(accelReport.geojson.geometry.coordinates[0]), double.Parse(accelReport.geojson.geometry.coordinates[1]));
                        

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR: {0}", ex);
                        for (int i = 0; i < 40; i++)
                        {
                            Thread.Sleep(25);
                            perfChart5.AddValue(0);
                            perfChart4.AddValue(0);
                            perfChart3.AddValue(0);
                        }
                    }

                };
                channel.BasicConsume(queue: key, //"emergency_gui",
                                     noAck: true,
                                     consumer: consumer);

                Console.WriteLine("Already BasicConsume");
                //Console.ReadLine();
            }

        
        }

        private void bgWrkTimer_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(Convert.ToInt32(e.Argument));
            
            RunTimer();
            
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            connect_msgserver("ecn");
            

            if (chkBxTimerEnabled.Checked)
            {
                RunTimer1();
                //textBox4.Text = quake_status;

            }
            
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            connect_msgserver("ecn2");

            if (chkBxTimerEnabled.Checked)
            {
                RunTimer2();
            }
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            connect_msgserver("ecn3");

            if (chkBxTimerEnabled.Checked)
            {
                RunTimer3();
            }
        }

        private void backgroundWorker4_DoWork(object sender, DoWorkEventArgs e)
        {
            connect_msgserver("ecn4");

            if (chkBxTimerEnabled.Checked)
            {
                RunTimer4();
            }
        }

        private void backgroundWorker5_DoWork(object sender, DoWorkEventArgs e)
        {
            connect_msgserver("ecn5");

            if (chkBxTimerEnabled.Checked)
            {
                RunTimer5();
            }
        }

        private void backgroundWorker6_DoWork(object sender, DoWorkEventArgs e)
        {
            connect_msgserver("ecn6");

            if (chkBxTimerEnabled.Checked)
            {
                RunTimer6();
            }
        }



        public class Config
        {

            public string host;
            public string user;
            public string vhost;
            //public string port;
            public string password;            
            

        }

        private static void consume_data()
        {           
            
        }

        private void bgWrkTimer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {            

            if (chkBxTimerEnabled.Checked)
            {                
                RunTimer();
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (chkBxTimerEnabled.Checked)
            {
                RunTimer1();
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (chkBxTimerEnabled.Checked)
            {
                RunTimer2();
            }
        }

        private void backgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (chkBxTimerEnabled.Checked)
            {
                RunTimer3();
            }
        }

        private void backgroundWorker4_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (chkBxTimerEnabled.Checked)
            {
                RunTimer4();
            }
        }

        private void backgroundWorker5_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (chkBxTimerEnabled.Checked)
            {
                RunTimer5();
            }
        }

        private void backgroundWorker6_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (chkBxTimerEnabled.Checked)
            {
                RunTimer6();
            }
        }



        private void cmbBxBorder_SelectedIndexChanged(object sender, EventArgs e)
        {
            perfChart.BorderStyle = (Border3DStyle)Enum.Parse(
                typeof(Border3DStyle), cmbBxBorder.SelectedItem.ToString()
            );
            perfChart1.BorderStyle = (Border3DStyle)Enum.Parse(
                typeof(Border3DStyle), cmbBxBorder.SelectedItem.ToString()
            );
            perfChart2.BorderStyle = (Border3DStyle)Enum.Parse(
                typeof(Border3DStyle), cmbBxBorder.SelectedItem.ToString()
            );
        }

        private void cmbBxScaleMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            perfChart.ScaleMode = (SpPerfChart.ScaleMode)Enum.Parse(
                typeof(SpPerfChart.ScaleMode), cmbBxScaleMode.SelectedItem.ToString()
            );
            
            perfChart1.ScaleMode = (SpPerfChart.ScaleMode)Enum.Parse(
                typeof(SpPerfChart.ScaleMode), cmbBxScaleMode.SelectedItem.ToString()
            );
            perfChart2.ScaleMode = (SpPerfChart.ScaleMode)Enum.Parse(
                typeof(SpPerfChart.ScaleMode), cmbBxScaleMode.SelectedItem.ToString()
            );
        }

        private void cmbBxTimerMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            perfChart.TimerMode = (SpPerfChart.TimerMode)Enum.Parse(
                typeof(SpPerfChart.TimerMode), cmbBxTimerMode.SelectedItem.ToString()
            );
            perfChart1.TimerMode = (SpPerfChart.TimerMode)Enum.Parse(
               typeof(SpPerfChart.TimerMode), cmbBxTimerMode.SelectedItem.ToString()
           );
            perfChart2.TimerMode = (SpPerfChart.TimerMode)Enum.Parse(
               typeof(SpPerfChart.TimerMode), cmbBxTimerMode.SelectedItem.ToString()
           );
        }

        private void numUpDnTimerInterval_ValueChanged(object sender, EventArgs e)
        {
            perfChart.TimerInterval = Convert.ToInt32(numUpDnTimerInterval.Value);
            perfChart1.TimerInterval = Convert.ToInt32(numUpDnTimerInterval.Value);
            perfChart2.TimerInterval = Convert.ToInt32(numUpDnTimerInterval.Value);
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            valueGenFrom = Convert.ToInt32(numUpDnValFrom.Value);
            valueGenTo = Convert.ToInt32(numUpDnValTo.Value);
            if (valueGenTo < valueGenFrom)
            {
                valueGenTo = valueGenFrom;
                numUpDnValTo.Value = valueGenTo;
            }

            valueGenTimerFrom = Convert.ToInt32(numUpDnFromInterval.Value);
            valueGenTimerTo = Convert.ToInt32(numUpDnToInterval.Value);
            if (valueGenTimerTo < valueGenTimerFrom)
            {
                valueGenTimerTo = valueGenTimerFrom;
                numUpDnToInterval.Value = valueGenTimerTo;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            perfChart.Clear();
            perfChart1.Clear();
            perfChart2.Clear();
            string a;
           
            string key = "agOz9gCZATM58FI3ubCubyuot";
            string secret = "57m1Xh5SGyuogn4zttjFpz6SHwRYBA9mpJdPNJHbXPoisf9KGn";
            string token = "860367448268087296-yahVsCIdXOyDQzs7R9to8ITUurBaHHA";
            string tokenSecret = "wrGpbaIF98cGPXYuXZxMm0BYDLoyb7w4hq3GA2q5JzGhT";
            var service = new TweetSharp.TwitterService(key, secret);
            service.AuthenticateWith(token, tokenSecret);
            Console.WriteLine("Tweet Sent");
            string message = "Peringatan telah terjadi gempa susulan dengan skala 6,3 dengan pusat gempa di lokasi Bandung";

            {

                var result = service.SendTweet(new SendTweetOptions
                {
                    Status = message
                   
                    
            }
                
);
                
                try
                {
                    a = result.Text.ToString();
                    Console.WriteLine(a);
                }
                catch(System.NullReferenceException z)
                {
                    Console.WriteLine("Tweet Failed");
                }
            }
            
        }

        

        private void FrmTestingForm_Load(object sender, EventArgs e)
        {

        }

        private void gMapControl1_Load(object sender, EventArgs e)
        {

            gMapControl1.MapProvider = GMap.NET.MapProviders.BingMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            gMapControl1.Position = new GMap.NET.PointLatLng(-6.9175, 107.6191);
            gMapControl1.ShowCenter = false;
            GMapOverlay markers = new GMapOverlay("markers");
            GMapMarker marker = new GMarkerGoogle(
                new PointLatLng(-6.890903, 107.610378),
                GMarkerGoogleType.blue_pushpin);
            markers.Markers.Add(marker);
            gMapControl1.Overlays.Add(markers);

        }

        private accel ParsingMessage(string messages)
        {
            accel accel_data;
            string data_x, data_y, data_z;
            char[] delimiters = { '<', '>' };
            string[] parseMessage = messages.Split(delimiters);
            data_x = parseMessage[1];
            data_y = parseMessage[2];
            data_z = parseMessage[3];
            accel_data.accel_x = (int)Math.Ceiling(float.Parse(data_x) * 1000);
            accel_data.accel_y = (int)Math.Ceiling(float.Parse(data_y) * 1000);
            accel_data.accel_z = (int)Math.Ceiling(float.Parse(data_z) * 1000);

            return accel_data;
        }

        private void perfChart_Load(object sender, EventArgs e)
        {
            //receive();
        }

        private void perfChart4_Load(object sender, EventArgs e)
        {

        }

        private void perfChart6_Load(object sender, EventArgs e)
        {

        }

        private void perfChart_Load_1(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox4.Text = "";
            quake_status = "";
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void perfChart3_Load(object sender, EventArgs e)
        {

        }

        private void groupBox13_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox12_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox11_Enter(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }
    }



    class data
    {
        public string pointTime;
        public string timeZone;
        public string interval;
        public string clientID;
        public geojson geojson;
        public acc[] accelerations;
    };

    class geojson
    {        
        public geometry geometry;
        public prop property;
    };

    class geometry
    {
        public string type;
        public string[] coordinates;
    };

    class prop
    {
        public string name;
    };


    class acc
    {
        public string x;
        public string y;
        public string z;
    };
          
}
