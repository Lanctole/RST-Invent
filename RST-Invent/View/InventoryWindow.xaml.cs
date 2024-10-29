using RST_Invent.ViewModel;
using System.ComponentModel;
using System.Windows;

namespace RST_Invent.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class InventoryWindow : Window
    {
        public InventoryWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            var viewModel = (AppViewModel)this.DataContext;
            if (viewModel.SaveOnExit)
            {
                viewModel.SaveNomenclature();
            }
        }
    }
}
