using PKHeX.Core;

namespace PKHeX.Plugin
{
    public class TrackerExport : IPlugin
    {
        public string Name => "Exportador de Home Tracking";
        public int Priority => 1;
        public ISaveFileProvider SaveFileEditor { get; private set; }

        public void Initialize(params object[] args)
        {
            SaveFileEditor = (ISaveFileProvider)Array.Find(args, z => z is ISaveFileProvider);
            var menu = (ToolStrip)Array.Find(args, z => z is ToolStrip);
            var baseMenu = (ToolStripDropDownItem)menu.Items.Find("Menu_Tools", false)[0];

            var exportItem = new ToolStripMenuItem("Exportar Trackers das Boxes");
            exportItem.Click += (s, e) => ExportarDados();
            baseMenu.DropDownItems.Add(exportItem);
        }

        private void ExportarDados()
        {
            var sav = SaveFileEditor.SAV;
            var results = new List<string> { "Box,Slot,Especie,HomeTracker" };

            for (int i = 0; i < sav.BoxCount; i++)
            {
                for (int j = 0; j < sav.BoxSlotCount; j++)
                {
                    var pkm = sav.GetBoxSlotAtIndex(i, j);
                    if (pkm.Species <= 0) continue;
                    results.Add($"{i + 1},{j + 1},{pkm.Species},{pkm.Tracker:X16}");
                }
            }

            File.WriteAllLines("trackers_home.csv", results);
            MessageBox.Show("Relatório 'trackers_home.csv' gerado na pasta do PKHeX!");
        }

        public void NotifySaveLoaded() { }
        public bool TryModifyFile(string path) => false;
    }
}
