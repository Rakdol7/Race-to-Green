using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
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
        private bool prematureClick = false;
        private string username;
        private string filePath;
        private string userDirectory = "UserData";
        private CancellationTokenSource cts; // Token per interrompere il timer

        public Form1()
        {
            InitializeComponent();
            stopwatch = new Stopwatch();
            random = new Random();

            // Richiede il nome utente all'avvio
            username = PromptUsername();
            Directory.CreateDirectory(userDirectory); // Crea la cartella se non esiste
            filePath = Path.Combine(userDirectory, $"{username}.txt");

            lblMessage.Text = "Premi START per iniziare";
            this.BackColor = Color.White;
            this.Click += new EventHandler(Form1_Click);
        }

        private string PromptUsername()
        {
            string input = "";
            while (string.IsNullOrWhiteSpace(input))
            {
                input = Microsoft.VisualBasic.Interaction.InputBox("Inserisci il tuo nome utente:", "Nome Utente", "");
            }
            return input;
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            if (prematureClick)
            {
                lblMessage.Text = "Hai cliccato troppo presto! Punteggio: -1000 ms. Premi START per riprovare.";
                this.BackColor = Color.White;
                btnStart.Enabled = true;
                prematureClick = false;
                waitingForClick = false;

                // **Interrompi il timer se attivo**
                cts?.Cancel();

                // **Registra punteggio negativo**
                string logEntry = $"{DateTime.Now}: -1000 ms (Click prematuro)\n";
                File.AppendAllText(filePath, logEntry);
            }
            else if (waitingForClick)
            {
                stopwatch.Stop();
                long reactionTime = stopwatch.ElapsedMilliseconds;
                lblMessage.Text = $"Tempo di reazione: {reactionTime} ms";
                this.BackColor = Color.White;
                btnStart.Enabled = true;
                waitingForClick = false;

                // **Salva il tempo nel file**
                string logEntry = $"{DateTime.Now}: {reactionTime} ms\n";
                File.AppendAllText(filePath, logEntry);
            }
        }

        private async void StartTest()
        {
            btnStart.Enabled = false;
            lblMessage.Text = "Aspetta il verde...";
            this.BackColor = Color.Red;
            prematureClick = true;
            waitingForClick = false;

            // **Nuovo token per gestire la cancellazione**
            cts = new CancellationTokenSource();
            int waitTime = random.Next(2000, 5000);

            try
            {
                await Task.Delay(waitTime, cts.Token); // Aspetta, ma si può interrompere
            }
            catch (TaskCanceledException)
            {
                return; // Se l'attesa viene interrotta, esce senza cambiare colore
            }

            this.BackColor = Color.Green;
            lblMessage.Text = "PREMI ORA!";
            stopwatch.Restart();
            waitingForClick = true;
            prematureClick = false;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartTest();
        }
    }
}
