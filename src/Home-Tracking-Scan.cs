using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.Plugin
{
    public class HomeTrackingScan : IPlugin
    {
        public string Name => "Home Tracking Scan";
        public int Priority => 1;
        public ISaveFileProvider SaveFileEditor { get; private set; }

        public void Initialize(params object[] args)
        {
            SaveFileEditor = (ISaveFileProvider)Array.Find(args, z => z is ISaveFileProvider);
            var menu = (ToolStrip)Array.Find(args, z => z is ToolStrip);
            
            // Localiza o menu "Tools" (Ferramentas) do PKHeX
            var toolsMenu = (ToolStripMenuItem)menu.Items["Menu_Tools"];

            // Cria o novo item de menu
            var exportItem = new ToolStripMenuItem("Exportar Trackers das Boxes");
            exportItem.Click += (s, e) => ExportarDados();
            
            // Adiciona ao menu Tools
            toolsMenu.DropDownItems.Add(exportItem);
        }

        private void ExportarDados()
        {
            if (SaveFileEditor.SAV == null)
            {
                MessageBox.Show("Por favor, carregue um save primeiro!");
                return;
            }

            var sav = SaveFileEditor.SAV;
            var results = new List<string> { "Box,Slot,Especie,Nickname,HomeTracker" };

            for (int i = 0; i < sav.BoxCount; i++)
            {
                for (int j = 0; j < sav.BoxSlotCount; j++)
                {
                    var pkm = sav.GetBoxSlotAtIndex(i, j);
                    if (pkm.Species <= 0) continue;

                    string trackerHex = pkm.Tracker.ToString("X16");
                    results.Add($"{i + 1},{j + 1},{pkm.Species},{pkm.Nickname},{trackerHex}");
                }
            }

            string fileName = "trackers_home.csv";
            File.WriteAllLines(fileName, results);
            
            MessageBox.Show($"Relatório '{fileName}' gerado na pasta do seu PKHeX!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void NotifySaveLoaded() { }
        public bool TryModifyFile(string path) => false;
    }
}
