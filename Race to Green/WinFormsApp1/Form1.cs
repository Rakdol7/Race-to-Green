using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {

        private Stopwatch stopwatch;
        private Random random;
        private bool waitingForClick = false;

        public Form1()
        {
            InitializeComponent();
            stopwatch = new Stopwatch();
            random = new Random();

            // Imposta proprietà iniziali
            lblMessage.Text = "Premi START per iniziare";
            this.BackColor = Color.White;

            // Aggiunge evento click sulla finestra
            this.Click += new EventHandler(Form1_Click);
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            if (waitingForClick)
            {
                stopwatch.Stop();
                long reactionTime = stopwatch.ElapsedMilliseconds;
                lblMessage.Text = $"Tempo di reazione: {reactionTime} ms";
                this.BackColor = Color.White;
                btnStart.Enabled = true;
                waitingForClick = false;
            }
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            lblMessage.Text = "Aspetta il verde...";
            this.BackColor = Color.Red;

            // Attesa casuale tra 2 e 5 secondi
            int waitTime = random.Next(2000, 5000);
            await Task.Delay(waitTime);

            this.BackColor = Color.Green;
            lblMessage.Text = "PREMI ORA!";
            stopwatch.Restart();
            waitingForClick = true;
        }


    }
}
