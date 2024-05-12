using Pick_Bandit.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pick_Bandit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private struct EnemyData
        {
            public ImageSource s;
            public int level;
        }

        private static ResourceManager manager;
        public static Random rnd = new();
        private static string[] images;
        private EnemyData[] enemies;
        private Agent agent;
        private const int MYLEVEL = 10;
        private const int AGENTRUNTIME_PERCLICK = 600;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Init(object sender, EventArgs e)
        {
            images = LoadImagesNames();
            manager = Properties.Resources.ResourceManager;
            enemies = new EnemyData[3];
        }

        private static string[] LoadImagesNames()
        {
            ResourceManager resourceClass = new(typeof(Resources));
            ResourceSet? resourceSet = resourceClass.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            List<string> titles = [];

            if (resourceSet == null)
                throw new Exception("Can't get resource set from file");

            foreach (DictionaryEntry entry in resourceSet)
                titles.Add(entry.Key.ToString());

#pragma warning disable IDE0305
            return titles.ToArray();
#pragma warning restore IDE0305
        }

        private void SelectThree()
        {
            string[] choices = rnd.GetItems(images, 3);

            for (int i = 0; i < 3; i++) 
            {
                enemies[i].s = Utils.ByteToImage((byte[])manager.GetObject(choices[i]));
                enemies[i].level = rnd.Next((int)Math.Abs(MYLEVEL - 5), MYLEVEL + 5);
            }

            bandit1.Source = enemies[0].s;
            bandit2.Source = enemies[1].s;
            bandit3.Source = enemies[2].s;
            b1_l.Content = enemies[0].level.ToString();
            b2_l.Content = enemies[1].level.ToString();
            b3_l.Content = enemies[2].level.ToString();
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            SelectThree();
            agent = new(MYLEVEL, [enemies[0].level, enemies[1].level, enemies[2].level], Agent.DEFUALT_ALPHA);
        }

        private async void btnCA_Click(object sender, RoutedEventArgs e)
        {
            btnCA.IsEnabled = false;
            btnRA.IsEnabled = false;

            try
            {
                agent.ChangeExplorationRate(int.Parse(txtEx.Text));
            }
            catch (Exception) 
            {
                // Failed do nothing and keep the alpha as it is
            }

            var t = Task.Factory.StartNew<int[]>(() => 
            {
                for (int i = 0; i < AGENTRUNTIME_PERCLICK; i++)
                    agent.MakeMove();

                return agent.ActionCopy();
            });

            await t.WaitAsync(CancellationToken.None);
            int[] tempExpected = t.Result;

            logList.Items.Add(string.Format("Enemy 0: {0},\n Enemy 1: {1},\n Enemy 2: {2}\n",
                tempExpected[0], tempExpected[1], tempExpected[2]));

            logList.ScrollIntoView(logList.Items[logList.Items.Count-1]);

            t.Dispose();
            btnCA.IsEnabled = true;
            btnRA.IsEnabled = true;
        }

        private void btnRA_Click(object sender, RoutedEventArgs e)
        {
            SelectThree();
            agent.UpdateEnemysLevels([enemies[0].level, enemies[1].level, enemies[2].level]);
        }

        private void btnCl_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmbAlgo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (agent != null)
            {
                if (cmbAlgo.SelectedIndex == 0)
                    agent.isLCB = true;
                else if (cmbAlgo.SelectedIndex == 1)
                    agent.isLCB = false;
            }
        }

    }
}
