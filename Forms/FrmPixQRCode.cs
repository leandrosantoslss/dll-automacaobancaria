using QRCoder;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace AutomacaoBancaria.Forms
{
    internal sealed class FrmPixQRCode : Form
    {
        private readonly string conteudo;
        private readonly PictureBox imagemQr = new PictureBox();
        private readonly TextBox copiaECola = new TextBox();
        private readonly Label status = new Label();

        public FrmPixQRCode(string conteudo)
        {
            this.conteudo = conteudo;
            Text = "Pagamento PIX";
            ClientSize = new Size(420, 535);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            imagemQr.BorderStyle = BorderStyle.FixedSingle;
            imagemQr.Location = new Point(30, 20);
            imagemQr.Size = new Size(360, 360);
            imagemQr.SizeMode = PictureBoxSizeMode.Zoom;

            status.AutoSize = true;
            status.Location = new Point(30, 394);
            status.Text = "Aponte a camera do banco para pagar";

            copiaECola.Location = new Point(30, 420);
            copiaECola.Multiline = true;
            copiaECola.ReadOnly = true;
            copiaECola.ScrollBars = ScrollBars.Vertical;
            copiaECola.Size = new Size(360, 48);
            copiaECola.Text = conteudo;

            var copiar = new Button { Location = new Point(234, 482), Size = new Size(75, 28), Text = "Copiar" };
            copiar.Click += Copiar_Click;
            var fechar = new Button { Location = new Point(315, 482), Size = new Size(75, 28), Text = "Fechar" };
            fechar.Click += (sender, args) => Close();

            Controls.Add(imagemQr);
            Controls.Add(status);
            Controls.Add(copiaECola);
            Controls.Add(copiar);
            Controls.Add(fechar);
            Load += FrmPixQRCode_Load;
        }

        private void FrmPixQRCode_Load(object sender, EventArgs e)
        {
            try
            {
                using (var gerador = new QRCodeGenerator())
                using (var dados = gerador.CreateQrCode(conteudo, QRCodeGenerator.ECCLevel.Q))
                using (var qrCode = new QRCode(dados))
                {
                    imagemQr.Image = new Bitmap(qrCode.GetGraphic(8));
                }
            }
            catch (Exception ex)
            {
                status.Text = "Nao foi possivel gerar o QR Code: " + ex.Message;
            }
        }

        private void Copiar_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(conteudo);
            status.Text = "Codigo PIX copiado";
        }

        public static void Exibir(string conteudo)
        {
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                using (var formulario = new FrmPixQRCode(conteudo)) formulario.ShowDialog();
                return;
            }

            Exception erro = null;
            var thread = new Thread(() =>
            {
                try { using (var formulario = new FrmPixQRCode(conteudo)) formulario.ShowDialog(); }
                catch (Exception ex) { erro = ex; }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            if (erro != null) throw erro;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FrmPixQRCode
            // 
            this.ClientSize = new System.Drawing.Size(322, 281);
            this.Name = "FrmPixQRCode";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "RECEBIMENTO PIX";
            this.ResumeLayout(false);

        }
    }
}
