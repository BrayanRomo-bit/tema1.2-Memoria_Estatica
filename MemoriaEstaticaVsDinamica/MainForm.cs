using System.Windows.Forms;

namespace MemoriaEstaticaVsDinamica;

public class MainForm : Form
{
    public MainForm()
    {
        Text = "Memoria Estatica vs. Memoria Dinamica";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(760, 520);
        Size = new Size(820, 560);
    }
}
