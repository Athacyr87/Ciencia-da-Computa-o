using System;
using System.Windows.Forms;
using System.IO;
namespace HtmlEditor
{
    class MenuManager
    {
        private RichTextBox richTextBox;


        public MenuManager(RichTextBox richTextBox)
        {
            this.richTextBox = richTextBox;
        }

        public void OpenFile(Form1 form)
        {
            if (form.unsavedChanges)
            {
                // Pergunta ao usuário se deseja salvar antes de abrir um novo arquivo
                DialogResult result = MessageBox.Show("Você tem alterações não salvas. Deseja salvar o arquivo atual?",
                                                       "Salvar Arquivo",
                                                       MessageBoxButtons.YesNoCancel,
                                                       MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Salva o arquivo atual
                    SaveFile(form); // Salva o arquivo
                }
                else if (result == DialogResult.Cancel)
                {
                    // Se o usuário cancelar, não faça nada
                    return;
                }
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "HTML Files|*.html|Text Files|*.txt|All Files|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Carrega o arquivo no RichTextBox
                form.RichTextBoxText = File.ReadAllText(openFileDialog.FileName);
                form.currentFileName = openFileDialog.FileName;

                // Atualiza o título da janela para o nome do arquivo
                form.unsavedChanges = false;  // Marca como salvo
                form.UpdateWindowTitle(); // Atualiza o título da janela
            }
        }


        public void Save_Normal(Form1 form)
        {
            // Verifica se o arquivo já foi salvo antes
            if (form.currentFileName != null)
            {
                // Salva o arquivo diretamente no local existente
                File.WriteAllText(form.currentFileName, form.RichTextBoxText);
                form.unsavedChanges = false;  // Marca como salvo
                form.UpdateWindowTitle(); // Atualiza o título da janela
            }
            else
            {
                // Se o arquivo não tem nome, chama a função "Salvar Como"
                SaveFile(form);
            }
        }

        public void CreateNewFile(Form1 form)
        {
            if (form.unsavedChanges)
            {
                // Pergunta ao usuário se deseja salvar antes de criar um novo arquivo
                DialogResult result = MessageBox.Show("Você tem alterações não salvas. Deseja salvar o arquivo atual?",
                                                       "Salvar Arquivo",
                                                       MessageBoxButtons.YesNoCancel,
                                                       MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    // Salva o arquivo atual
                    SaveFile(form); // Salva o arquivo
                }
                else if (result == DialogResult.Cancel)
                {
                    // Se o usuário cancelar, não faça nada
                    return;
                }
            }

            // Se o usuário não tem alterações ou escolheu não salvar, cria um novo arquivo
            form.RichTextBoxText = string.Empty; // Limpa o RichTextBox
            form.currentFileName = null; // Reseta o nome do arquivo atual
            form.unsavedChanges = false; // Reseta a flag de alterações
            form.UpdateWindowTitle(); // Atualiza o título da janela
        }

        public void SaveFile(Form1 form)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "HTML Files|*.html|Text Files|*.txt|All Files|*.*";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Salva o conteúdo no arquivo selecionado
                File.WriteAllText(saveFileDialog.FileName, form.RichTextBoxText);
                form.currentFileName = saveFileDialog.FileName; // Atualiza o nome do arquivo
                form.unsavedChanges = false;  // Marca como salvo
                form.UpdateWindowTitle(); // Atualiza o título da janela
            }
        }
    }
}
