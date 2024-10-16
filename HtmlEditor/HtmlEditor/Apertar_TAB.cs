using System;
using System.Windows.Forms;

namespace HtmlEditor
{
    class Apertar_TAB
    {
        public void RichTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            RichTextBox richTextBox = sender as RichTextBox;

            if (richTextBox != null && e.KeyCode == Keys.Tab)
            {
                e.SuppressKeyPress = true; // Impede o comportamento padrão do Tab
                int start = richTextBox.SelectionStart;
                richTextBox.Text = richTextBox.Text.Insert(start, "    "); // Adiciona 4 espaços
                richTextBox.SelectionStart = start + 4; // Move o cursor para a posição correta
            }
        }
    }
}
