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
        public ISaveFileProvider SaveFileEditor { get; private set; } = null!;

        public void Initialize(params object[] args)
        {
            SaveFileEditor = (ISaveFileProvider)Array.Find(args, z => z is ISaveFileProvider)!;
            var menu = (ToolStrip)Array.Find(args, z => z is ToolStrip)!;
            var toolsMenu = (ToolStripMenuItem)menu.Items["Menu_Tools"]!;

            var exportItem = new ToolStripMenuItem("Exportar Trackers das Boxes");
            exportItem.Click += (s, e) => ExportarDados();
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

                    // Formata o valor do Tracker (u64) em Hexadecimal de 16 dígitos
                    string trackerHex = pkm.Tracker.ToString("X16");
                    results.Add($"{i + 1},{j + 1},{pkm.Species},{pkm.Nickname},{trackerHex}");
                }
            }

            string fileName = "trackers_home.csv";
            try 
            {
                File.WriteAllLines(fileName, results);
                MessageBox.Show($"Relatório '{fileName}' gerado com sucesso na pasta do seu PKHeX!", "Sucesso");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar arquivo: {ex.Message}");
            }
        }

        // Métodos exigidos pela interface IPlugin da versão 26.3.20+
        public void NotifySaveLoaded() { }
        public bool TryLoadFile(string filePath) => false; 
        public bool TryModifyFile(string filePath) => false;
    }
}
