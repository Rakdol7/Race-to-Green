﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection.Metadata;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private Stopwatch cronometro = new Stopwatch();
        private Random random = new Random();
        private bool aspettaClick = false;
        private bool inizioTest = false; // Per controllare se il test è in corso
        private string filePath;
        private string nomeUtente;

        public Form1()
        {
            InitializeComponent();
            nomeUtente = Microsoft.VisualBasic.Interaction.InputBox("Inserisci il tuo nome utente:", "Nome Utente", "");
            lblMessage.Text = "Premi START per iniziare";
            BackColor = Color.White;
        }

        private async void StartTest()
        {
            btnStart.Enabled = false;
            lblMessage.Text = "Aspetta il verde...";
            BackColor = Color.Red;
            inizioTest = true;

            int attesa = random.Next(2000, 5000);
            await Task.Delay(attesa);

            if (inizioTest==false) return; // Se il test è stato annullato, esce

            BackColor = Color.Green;
            lblMessage.Text = "PREMI ORA!";
            cronometro.Restart();
            aspettaClick = true;
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            if (inizioTest==true && aspettaClick==false) // Se preme prima del verde
            {
                lblMessage.Text = "Troppo presto! -1000 ms";
                var oStream = new FileStream(@"./"+ nomeUtente+".txt", FileMode.Append, FileAccess.Write, FileShare.Read);
                StreamWriter sw = new StreamWriter(oStream);
                sw.WriteLine(DateTime.Now.ToString() + ": -1000 ms (Click prematuro)\n");
                sw.Close();
                ResetTest();
            }
            else if (aspettaClick==true) // Se preme nel momento giusto
            {
                cronometro.Stop();
                long tempoMisurato = cronometro.ElapsedMilliseconds;
                lblMessage.Text = "Tempo di reazione: " + tempoMisurato + " ms";
                var oStream = new FileStream(@"./" + nomeUtente + ".txt", FileMode.Append, FileAccess.Write, FileShare.Read);
                StreamWriter sw = new StreamWriter(oStream);
                sw.WriteLine(DateTime.Now.ToString() + ": " + tempoMisurato + " ms\n");
                sw.Close();
                ResetTest();
            }
        }

        private void ResetTest()
        {
            BackColor = Color.White;
            btnStart.Enabled = true;
            aspettaClick = false;
            inizioTest = false;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartTest();
        }
    }
}