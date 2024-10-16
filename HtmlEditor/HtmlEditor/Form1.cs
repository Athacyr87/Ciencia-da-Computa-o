using System.Windows.Forms;
using System;
using System.Drawing;
namespace HtmlEditor
{
    public partial class Form1 : Form
    {
        // No seu Form1, adicione um ToolStripStatusLabel
        private ToolStripStatusLabel wordCountStatusLabel;

        // No seu Form1, adicione um ToolStripStatusLabel para o contador de linhas
        private ToolStripStatusLabel lineCountStatusLabel;

        // Variável para controlar alterações não salvas
        public bool unsavedChanges = false; 

        // Armazena o nome do arquivo atual
        public string currentFileName = null; 

        // Crie uma instância da classe SyntaxHighlighter
        private SyntaxHighlighter syntaxHighlighter;
        private Apertar_TAB tabHandler;

        private MenuManager menuManager;

        public Form1()
        {
            InitializeComponent();
            tabHandler = new Apertar_TAB();
            this.richTextBox1.KeyDown += new KeyEventHandler(tabHandler.RichTextBox1_KeyDown);
            // Inicialize o SyntaxHighlighter
            syntaxHighlighter = new SyntaxHighlighter();

            menuManager = new MenuManager(richTextBox1); // Passa o RichTextBox para o MenuManager
            richTextBox1.TextChanged += richTextBox1_TextChanged;

            // Ativa o uso da tecla Tab no RichTextBox
            this.richTextBox1.AcceptsTab = true;
            richTextBox1.KeyDown += richTextBox1_KeyDown;

            // Inicializa o ToolStripStatusLabel
            wordCountStatusLabel = new ToolStripStatusLabel();
            lineCountStatusLabel = new ToolStripStatusLabel();
            statusStrip1.Items.Add(wordCountStatusLabel); // Supondo que o seu StatusStrip é chamado statusStrip1
            statusStrip1.Items.Add(lineCountStatusLabel);  // Contador de linhas
        }

        /* Verificar tags automaticamente*/
        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                MessageBox.Show("Desfazendo ação!"); // Para depuração
                desfazerToolStripMenuItem.PerformClick();
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Enter)
            {
                // Pega o texto atual antes da posição do cursor
                int currentPosition = richTextBox1.SelectionStart;
                string currentLine = GetCurrentLineText();

                // Verifica se é uma tag de abertura válida
                if (IsOpeningTag(currentLine))
                {
                    string tagName = GetTagName(currentLine);
                    InsertClosingTag(tagName);
                }
            }
        }

        // Método para pegar a linha atual do RichTextBox
        private string GetCurrentLineText()
        {
            int lineIndex = richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart);
            string currentLine = richTextBox1.Lines[lineIndex].Trim();
            return currentLine;
        }

        // Verifica se o texto é uma tag HTML de abertura
        private bool IsOpeningTag(string text)
        {
            return text.StartsWith("<") && !text.Contains("</") && text.EndsWith(">");
        }

        // Pega o nome da tag (sem os caracteres < e >)
        private string GetTagName(string openingTag)
        {
            openingTag = openingTag.Trim('<', '>', '/');
            return openingTag.Split(' ')[0]; // Para lidar com tags como <div class="abc">
        }

        // Insere a tag de fechamento automaticamente
        private void InsertClosingTag(string tagName)
        {
            // Adiciona uma nova linha e a tag de fechamento
            int currentPosition = richTextBox1.SelectionStart;
            string closingTag = $"\n</{tagName}>";

            richTextBox1.Text = richTextBox1.Text.Insert(currentPosition, closingTag);

            // Coloca o cursor entre as tags <div> | </div>
            richTextBox1.SelectionStart = currentPosition;

            this.desfazerToolStripMenuItem.Click += new EventHandler(desfazerToolStripMenuItem_Click);

        }

        public string RichTextBoxText
        {

            get { return richTextBox1.Text; }
            set { richTextBox1.Text = value; }
        }

        public void UpdateWindowTitle()
        {
            string fileName = currentFileName ?? "Novo Arquivo"; // Se currentFileName for null, usa "Novo Arquivo"

            if (unsavedChanges)
            {
                this.Text = $"*{fileName} - Meu Editor de HTML"; // Adiciona o asterisco se houver mudanças
            }
            else
            {
                this.Text = $"{fileName} - Meu Editor de HTML"; // Remove o asterisco quando salvo
            }
        }


        // Evento TextChanged do RichTextBox
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            desfazerToolStripMenuItem.Enabled = richTextBox1.CanUndo;
            
            // Marca como alterado quando o texto é modificado
            unsavedChanges = true;
            UpdateWindowTitle();
            // Chama o método de realce de sintaxe
            syntaxHighlighter.HighlightSyntax(richTextBox1);
            syntaxHighlighter.HighlightHref(richTextBox1);

            UpdateWordCount();
            UpdateLineCount();
        }

        private void UpdateWordCount()
        {
            string text = richTextBox1.Text;
            string[] words = text.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int wordCount = words.Length;

            // Atualiza o ToolStripStatusLabel com o número de palavras
            toolStripStatusLabel1.Text = $"Palavras: {wordCount}";
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            menuManager.OpenFile(this); // Chama o método OpenFile da classe MenuManager
        }
        private void UpdateLineCount()
        {
            string text = richTextBox1.Text;
            int lineCount = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;

            // Atualiza o ToolStripStatusLabel com o número de linhas
            toolStripStatusLabel2.Text = $"Linhas: {lineCount}";
        }
        private void novoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            menuManager.CreateNewFile(this); // Passa a referência do Form1
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (unsavedChanges)
            {
                var result = MessageBox.Show("Você não salvou as alterações. Deseja salvar antes de sair?", 
                    "Aviso", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    menuManager.SaveFile(this);
                    unsavedChanges = false; // Marca como salvo após salvar
                }
                else if (result == DialogResult.No)
                {
                    unsavedChanges = false; // Marca como salvo para evitar a mensagem na próxima vez
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true; // Cancela o fechamento se o usuário escolher cancelar
                }
            }
        }

        private void salvarToolStripMenuItem_Click(object sender, EventArgs e)
        {

            menuManager.Save_Normal(this);
        }

        private void salvarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            menuManager.SaveFile(this); // Passa o Form1 atual para salvar o arquivo
        }
     

        private void ChangeFont()
        {
            using (FontDialog fontDialog = new FontDialog())
            {
                // Define a fonte atual na caixa de diálogo
                fontDialog.Font = this.richTextBox1.Font;

                // Abre a caixa de diálogo e verifica se o usuário clicou em OK
                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    // Aplica a nova fonte ao RichTextBox
                    this.richTextBox1.Font = fontDialog.Font;
                }
            }
        }

        private void fonteToolStripMenuItem_Click(object sender, EventArgs e)
        {

            using (FontDialog fontDialog = new FontDialog())
            {
                // Define a fonte atual na caixa de diálogo
                fontDialog.Font = this.richTextBox1.Font;

                // Abre a caixa de diálogo e verifica se o usuário clicou em OK
                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    // Aplica a nova fonte ao RichTextBox
                    this.richTextBox1.Font = fontDialog.Font;
                }
            }

        }

        private void mudarCorDeFundoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Cria a caixa de diálogo para selecionar a cor
            ColorDialog colorDialog = new ColorDialog();

            // Mostra a caixa de diálogo e verifica se o usuário clicou em "OK"
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                // Define a cor de fundo selecionada no RichTextBox
                richTextBox1.BackColor = colorDialog.Color;
            }

        }

        private void desfazerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.CanUndo)
            {
                
                richTextBox1.Undo();
                richTextBox1.ClearUndo();
            }
        }

        private void inserirEstruturaHtmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Inserir estrutura
            string estruturaHtml5 = @"<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Título da Página</title>
    <link rel=""stylesheet"" href=""styles.css""> <!-- Link para CSS externo -->
</head>
<body>
    <header>
        <h1>Bem-vindo ao meu site</h1>
        <nav>
            <ul>
                <li><a href=""#home"">Home</a></li>
                <li><a href=""#sobre"">Sobre</a></li>
                <li><a href=""#contato"">Contato</a></li>
            </ul>
        </nav>
    </header>
    
    <main>
        <section id=""home"">
            <h2>Home</h2>
            <p>Conteúdo da página inicial.</p>
        </section>
        <section id=""sobre"">
            <h2>Sobre</h2>
            <p>Informações sobre o site.</p>
        </section>
        <section id=""contato"">
            <h2>Contato</h2>
            <p>Formulário de contato ou informações.</p>
        </section>
    </main>

    <footer>
        <p>&copy; 2024 Meu Site. Todos os direitos reservados.</p>
    </footer>
    
    <script src=""script.js""></script> <!-- Link para JavaScript externo -->
</body>
</html>";

            // Insere a estrutura no RichTextBox
            richTextBox1.AppendText(estruturaHtml5);
        }
    }
}
