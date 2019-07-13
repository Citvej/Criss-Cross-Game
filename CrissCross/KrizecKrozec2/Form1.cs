using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace KrizecKrozec2
{
    public partial class Form1 : Form
    {
        TcpClient client = null;
        TcpListener listener = null;
        IPAddress ip = IPAddress.Parse("192.168.56.1");
        int port = 7653;

        Thread thClient = null;
        Thread thListener = null;

        NetworkStream dataStream = null;

        string[,] polje = new string[3, 3] { {" ", " ", " "}, {" ", " ", " "}, {" ", " ", " "} };
        string igralec = " ";
        bool naVrsti;
        string zmagovalec = " ";
        string protokol = "";
        bool running = true;
        string prejeto;
        int steviloPotez = 0;

        public Form1()
        {
            InitializeComponent();
        }

        static string Recieve(NetworkStream ns)
        {
            try
            {
                byte[] recv = new byte[1024];
                int length = ns.Read(recv, 0, recv.Length);
                return Encoding.UTF8.GetString(recv, 0, length);
            }
            catch (Exception e)
            {
                Console.WriteLine("Napaka pri prejemanju: " + e.Message + "\n" + e.StackTrace);
                return null;
            }
        }

        static void Send(NetworkStream ns, string message)
        {
            try
            {
                byte[] send = Encoding.UTF8.GetBytes(message.ToCharArray(), 0, message.Length);
                ns.Write(send, 0, send.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("Napaka pri pošiljanju: " + e.Message + "\n" + e.StackTrace);
            }
        }

        private void Host()
        {
            try
            {
                listener = new TcpListener(ip, port);
                listener.Start();
                client = listener.AcceptTcpClient();
                this.BackColor = ColorTranslator.FromHtml("#3498db");
                using (dataStream = client.GetStream())
                {
                    while (running)
                    {
                        prejeto = Recieve(dataStream);
                        razberi();
                        while (naVrsti) ;
                        Send(dataStream, protokol);
                        naVrsti = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Vrglo napako " + ex);
            }
        }

        private void Client()
        {
            client = new TcpClient();
            client.Connect("192.168.56.1", port);
            this.BackColor = ColorTranslator.FromHtml("#2ecc71");
            while (running)
            {
                while (naVrsti) ;
                dataStream = client.GetStream();
                Send(dataStream, protokol);
                prejeto = Recieve(dataStream);
                razberi();
                naVrsti = true;
            }

        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            naVrsti = true;
            igralec = "O";

            thClient = new Thread(Host);
            thClient.IsBackground = true;
            thClient.Start();

        }
        private void button2_Click(object sender, EventArgs e)
        {
            naVrsti = true;
            igralec = "X";

            thClient = new Thread(Client);
            thClient.IsBackground = true;
            thClient.Start();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            button3.Text = igralec;
            polje[0, 0] = igralec;
            pripraviNaPosiljanje();
            naVrsti = false;
            
        }
        private void button4_Click(object sender, EventArgs e)
        {
            button4.Text = igralec;
            polje[0, 1] = igralec;
            pripraviNaPosiljanje();
            naVrsti = false;
        }
        private void button5_Click(object sender, EventArgs e)
        {
            button5.Text = igralec;
            polje[0, 2] = igralec;
            pripraviNaPosiljanje();
            naVrsti = false;
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            button6.Text = igralec;
            polje[1, 0] = igralec;
            pripraviNaPosiljanje();
            naVrsti = false;
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            button7.Text = igralec;
            polje[1, 1] = igralec;
            pripraviNaPosiljanje();
            naVrsti = false;
            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            button8.Text = igralec;
            polje[1, 2] = igralec;
            pripraviNaPosiljanje();
            naVrsti = false;
            
        }

        private void button9_Click(object sender, EventArgs e)
        {
            button9.Text = igralec;
            polje[2, 0] = igralec;
            pripraviNaPosiljanje();
            naVrsti = false;
            
        }

        private void button10_Click(object sender, EventArgs e)
        {
            button10.Text = igralec;
            polje[2, 1] = igralec;
            pripraviNaPosiljanje();
            naVrsti = false;
            
        }

        private void button11_Click(object sender, EventArgs e)
        {
            button11.Text = igralec;
            polje[2, 2] = igralec;
            pripraviNaPosiljanje();
            naVrsti = false;
            
        }
        public void pripraviNaPosiljanje()
        {
            steviloPotez++;
            protokol = "";

            dolociZmagovalca(); //1. kdo je zmagal
            protokol += zmagovalec;

            if(igralec == "X") protokol += "O"; //2. kdo je naslednji na vrsti
            else if(igralec == "O")protokol += "X";


            //protokol += Convert.ToString(steviloPotez); 
            //3. polje
            for(int i=0; i<3; i++)
            {
                for(int j=0; j<3; j++)
                {
                    protokol += polje[i,j];
                }
            }

            if (zmagovalec != " ")
            {
                switch (zmagovalec)
                {
                    case "X":
                        MessageBox.Show("Zmagovalec je X!");
                        break;
                    case "O":
                        MessageBox.Show("Zmagovalec je O!");
                        break;
                    case "N":
                        MessageBox.Show("Izid je neodločen!");
                        break;
                }
            }
        }
        public void dolociZmagovalca()
        {
            if (polje[0, 0] + polje[0, 1] + polje[0, 2] == "XXX") zmagovalec = "X";
            else if (polje[1, 0] + polje[1, 1] + polje[1, 2] == "XXX") zmagovalec = "X";
            else if (polje[2, 0] + polje[2, 1] + polje[2, 2] == "XXX") zmagovalec = "X";
            else if (polje[0, 0] + polje[1, 0] + polje[2, 0] == "XXX") zmagovalec = "X";
            else if (polje[0, 1] + polje[1, 1] + polje[2, 1] == "XXX") zmagovalec = "X";
            else if (polje[0, 2] + polje[1, 2] + polje[2, 2] == "XXX") zmagovalec = "X";
            else if (polje[0, 0] + polje[1, 1] + polje[2, 2] == "XXX") zmagovalec = "X";
            else if (polje[0, 2] + polje[1, 1] + polje[2, 0] == "XXX") zmagovalec = "X";
            else if (polje[0, 0] + polje[0, 1] + polje[0, 2] == "OOO") zmagovalec = "O";
            else if (polje[1, 0] + polje[1, 1] + polje[1, 2] == "OOO") zmagovalec = "O";
            else if (polje[2, 0] + polje[2, 1] + polje[2, 2] == "OOO") zmagovalec = "O";
            else if (polje[0, 0] + polje[1, 0] + polje[2, 0] == "OOO") zmagovalec = "O";
            else if (polje[0, 1] + polje[1, 1] + polje[2, 1] == "OOO") zmagovalec = "O";
            else if (polje[0, 2] + polje[1, 2] + polje[2, 2] == "OOO") zmagovalec = "O";
            else if (polje[0, 0] + polje[1, 1] + polje[2, 2] == "OOO") zmagovalec = "O";
            else if (polje[0, 2] + polje[1, 1] + polje[2, 0] == "OOO") zmagovalec = "O";
            else if (steviloPotez >= 5) zmagovalec = "N"; //rečemo, da v protokolu N naznanja da je bilo preseženo število potez in je neodločeno
            
            //if(protokol[0] != ' ')
            //{
            //    if(zmagovalec != " ")
            //        if(igralec == "O")
            //        {

            //            //dataStream.Close();
            //        }else if(igralec == "X")
            //        {
            //            //dataStream.Close();
            //            //client.Close();
            //        }
            //}
        }
        public void onemogociGumbe()
        {
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button8.Enabled = false;
            button9.Enabled = false;
            button10.Enabled = false;
            button11.Enabled = false;
        }
        public void razberi()
        {
            zmagovalec = Convert.ToString(prejeto[0]);
            polje[0, 0] = Convert.ToString(prejeto[2]);
            polje[0, 1] = Convert.ToString(prejeto[3]);
            polje[0, 2] = Convert.ToString(prejeto[4]);
            polje[1, 0] = Convert.ToString(prejeto[5]);
            polje[1, 1] = Convert.ToString(prejeto[6]);
            polje[1, 2] = Convert.ToString(prejeto[7]);
            polje[2, 0] = Convert.ToString(prejeto[8]);
            polje[2, 1] = Convert.ToString(prejeto[9]);
            polje[2, 2] = Convert.ToString(prejeto[10]);

            button3.Text = polje[0, 0];
            button4.Text = polje[0, 1];
            button5.Text = polje[0, 2];
            button6.Text = polje[1, 0];
            button7.Text = polje[1, 1];
            button8.Text = polje[1, 2];
            button9.Text = polje[2, 0];
            button10.Text = polje[2, 1];
            button11.Text = polje[2, 2];

            if (zmagovalec != " ")
            {
                switch (zmagovalec)
                {
                    case "X":
                        MessageBox.Show("Zmagovalec je X!");
                        zakljuci();
                        break;
                    case "O":
                        MessageBox.Show("Zmagovalec je O!");
                        zakljuci();
                        break;
                    case "N":
                        MessageBox.Show("Izid je neodločen!");
                        zakljuci();
                        break;
                }
            }
        }
        private void zakljuci()
        {
            if (igralec == "O")
            {
                client.Close();
                dataStream.Close();
                thClient.Join();
                Application.Exit();
            }else if(igralec == "X")
            {
                dataStream.Close();
                client.Close();
                thClient.Join();
                Application.Exit();
            }
        }
    }
}
