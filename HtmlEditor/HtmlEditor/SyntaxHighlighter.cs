using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace HtmlEditor
{
    class SyntaxHighlighter
    {
        // Constantes para desativar e ativar o redimensionamento visual
        private const int WM_SETREDRAW = 0x0B;

        // Importa a função SendMessage da API do Windows
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        // Método para suspender a pintura do controle
        public void BeginUpdate(RichTextBox richTextBox)
        {
            SendMessage(richTextBox.Handle, WM_SETREDRAW, (IntPtr)0, IntPtr.Zero);
        }

        // Método para reativar a pintura do controle
        public void EndUpdate(RichTextBox richTextBox)
        {
            SendMessage(richTextBox.Handle, WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);
            richTextBox.Invalidate();  // Solicita a repintura do controle
        }

        //Realçar Href
        public void HighlightHref(RichTextBox richTextBox)
        {
            BeginUpdate(richTextBox);  // Suspende a pintura do RichTextBox
            // Salva a posição atual do cursor
            int originalSelectionStart = richTextBox.SelectionStart;
            int originalSelectionLength = richTextBox.SelectionLength;

            // Expressão regular para capturar href="...".
            string pattern = @"\s*\s*[""'][^""']+[""']";
            
            MatchCollection matches = Regex.Matches(richTextBox.Text, pattern);
            foreach (Match match in matches)
            {
                int startIndex = match.Index;
                int length = match.Length;
                richTextBox.Select(startIndex, length);
                richTextBox.SelectionColor = Color.Brown; // Cor para o valor do atributo href
                richTextBox.DeselectAll();
            }

            // Restaura a posição original do cursor
            richTextBox.Select(originalSelectionStart, originalSelectionLength);
            richTextBox.SelectionColor = richTextBox.ForeColor; // Reseta a cor da seleção
            EndUpdate(richTextBox);  // Reativa a pintura do RichTextBox

        }

        // Método para realçar a sintaxe em um RichTextBox
        public void HighlightSyntax(RichTextBox richTextBox)
        {
            BeginUpdate(richTextBox);  // Suspende a pintura do RichTextBox

            // Palavras-chave HTML (tags)
            string[] keywords = {"<script","<section", "<small>", "</small>", "<html", "<meta", "<link",
        "<!DOCTYPE", "<footer>", "</footer>", "<section>", "</section>", "<main>", "</main>", "<a", "<header>", "</header>",
        "<nav>", "</nav>", "<head>", "</head>", "<html>", "</html>", "<body>", "</body>", "<h1>", "</h1>", "<p>", "</p>",
        "<li>", "</li>", "<div>", "</div>", "<h2>", "</h2>", "<h3>", "</h3>", "<h4>", "</h4>", "<h5>", "</h5>", "<h6>", "</h6>",
        "<span>", "</span>", "<a>", "</a>", "<img>", "<br>", "</br>", "<ol>", "</ol>", "<table>", "</table>", "<ul>", "</ul>",
        "<strong>", "</strong>", "<em>", "</em>", "<tr>", "</tr>", "<script>", "</script>", "<style>", "</style>", "<meta>", "</meta>",
        "<title>", "</title>"
    };

            // Atributos HTML que precisam de destaque (exemplo: href)
            string[] attributes = { "href=" , "rel=", "name=", "content=", "charset=", "id="};

            // Salva a posição original do cursor
            int originalIndex = richTextBox.SelectionStart;
            int originalLength = richTextBox.SelectionLength;
            Color originalColor = richTextBox.SelectionColor;

            // Limpa todas as cores
            richTextBox.SelectAll();
            richTextBox.SelectionColor = Color.Black;

            // Realça palavras-chave (tags)
            foreach (string keyword in keywords)
            {
                int startIndex = 0;
                while ((startIndex = richTextBox.Text.IndexOf(keyword, startIndex)) != -1)
                {
                    richTextBox.Select(startIndex, keyword.Length);
                    richTextBox.SelectionColor = Color.Blue;  // Cor da palavra-chave
                    startIndex += keyword.Length;
                }
            }

            // Realça atributos (como href)
            foreach (string attribute in attributes)
            {
                int startIndex = 0;
                while ((startIndex = richTextBox.Text.IndexOf(attribute, startIndex)) != -1)
                {
                    richTextBox.Select(startIndex, attribute.Length);
                    richTextBox.SelectionColor = Color.Red;  // Cor do atributo (href em vermelho)
                    startIndex += attribute.Length;
                }
            }

            // Realça comentários HTML
            int commentStart = 0;
            while ((commentStart = richTextBox.Text.IndexOf("<!--", commentStart)) != -1)
            {
                int commentEnd = richTextBox.Text.IndexOf("-->", commentStart);
                if (commentEnd == -1) break; // Se não encontrar o final do comentário, sai do loop

                richTextBox.Select(commentStart, commentEnd - commentStart + 3);
                richTextBox.SelectionColor = Color.Green;  // Cor para comentários
                commentStart = commentEnd + 3; // Continua a busca após o final do comentário
            }


            // Restaura o estado original do cursor
            richTextBox.SelectionStart = originalIndex;
            richTextBox.SelectionLength = originalLength;
            richTextBox.SelectionColor = originalColor;

            EndUpdate(richTextBox);  // Reativa a pintura do RichTextBox
        }
    }
}
