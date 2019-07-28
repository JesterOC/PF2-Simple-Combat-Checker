using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PathFInder_Damage_Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static void Log(string logMessage, TextWriter w)
        {
            w.Write($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
            w.WriteLine($"  :{logMessage}");
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int CommonACValue = 0;
            int ShieldACValue = 0;
            int SwordAndBoardShieldHardnessValue = 0;
            int SwordAndBoardShieldHPValue = 0;
            int FighterHPValue = 0;
            int FighterFightsValue = 0;
            int FighterToHitValue = 0;
            int sbWins = 0;
            int twoHandWinds = 0;
            int brokenBoards = 0;
            int despUses = 0;

            if (!Int32.TryParse(CommonAC.Text, out CommonACValue))
            {
                return;
            }
            if (!Int32.TryParse(ShieldAC.Text, out ShieldACValue))
            {
                return;
            }
            if (!Int32.TryParse(SwordAndBoardShield_Hardness.Text, out SwordAndBoardShieldHardnessValue))
            {
                return;
            }
            if (!Int32.TryParse(SwordAndBoardShield_HP.Text, out SwordAndBoardShieldHPValue))
            {
                return;
            }
            if (!Int32.TryParse(FighterHP.Text, out FighterHPValue))
            {
                return;
            }
            if (!Int32.TryParse(FighterFights.Text, out FighterFightsValue))
            {
                return;
            }
            if (!Int32.TryParse(FighterToHit.Text, out FighterToHitValue))
            {
                return;
            }

            using (StreamWriter w = File.CreateText("log.txt"))
            {
                DiceExpression twoHandedDamge = new DiceExpression(TwoHandedDamage.Text);
                twoHandedDamge.GetCalculatedAverage();
                DiceExpression SwordAndBoardDamge = new DiceExpression(SwordAndBoardDam.Text);
                SwordAndBoardDamge.GetCalculatedAverage();

                SwordBoardDude sbDude = new SwordBoardDude(FighterHPValue, FighterToHitValue, SwordAndBoardDamge, CommonACValue, ShieldACValue, SwordAndBoardShieldHPValue, SwordAndBoardShieldHardnessValue, ShieldStrat,w);
                TwoHDude thDude = new TwoHDude(FighterHPValue, FighterToHitValue, twoHandedDamge, CommonACValue,w);

                for (int loop = 0; loop < FighterFightsValue; loop++)
                {
                    sbDude.Reset();
                    thDude.Reset();

                    while ((sbDude.HP > 0) && (thDude.HP > 0))
                    {
                        if (loop % 2 == 0)
                        {
                            Log("SB attacks=======", w);
                            sbDude.RenewReaction();
                            sbDude.Attack(thDude, 0);
                            sbDude.Attack(thDude, -5);
                            if (sbDude.BrokenShield || ShieldStrat.SelectedIndex == 2 )
                            {
                                sbDude.Attack(thDude, -10);
                            }
                            
                            if (thDude.HP > 0)
                            {
                                Log("TH attacks=======", w);
                                thDude.Attack(sbDude, 0);
                                thDude.Attack(sbDude, -5);
                                thDude.Attack(sbDude, -10);
                            }
                        }
                        else
                        {
                            Log("TH attacks=======", w);
                            thDude.Attack(sbDude, 0);
                            thDude.Attack(sbDude, -5);
                            thDude.Attack(sbDude, -10);
                            
                            if (sbDude.HP > 0)
                            {
                                Log("SB attacks=======", w);
                                sbDude.RenewReaction();
                                sbDude.Attack(thDude, 0);
                                sbDude.Attack(thDude, -5);
                                if (sbDude.BrokenShield || ShieldStrat.SelectedIndex == 2)
                                {
                                    sbDude.Attack(thDude, -10);
                                }
                            }
                        }
                    }
                    if (sbDude.HP > 0)
                    {
                        sbWins++;
                        Log("Sword n Board win!<--------", w);
                    }
                    else
                    {
                        twoHandWinds++;
                        Log("Two Handed win!<-----------", w);
                    }
                    Log("TH Hit Points: " + thDude.HP, w);
                    Log("SB Hit Points: " + sbDude.HP, w);
                    Log("SB Shield Hit Points: " + sbDude.CurrentShieldHP,w);
                    Log("SB Desp Shield Uses : " + sbDude.DespShieldUse, w);
                    Log("**************************" + sbDude.DespShieldUse, w);
                    if (sbDude.BrokenShield)
                    {
                        brokenBoards++;
                    }
                    if(sbDude.DespShieldUse>0)
                    {
                        despUses+= sbDude.DespShieldUse;
                    }
                }
                Application.Current.Dispatcher.Invoke(new Action(() => { sbResults.Content = sbWins; }));
                Application.Current.Dispatcher.Invoke(new Action(() => { thResults.Content = twoHandWinds; }));
                Application.Current.Dispatcher.Invoke(new Action(() => { bbResults.Content = brokenBoards; }));
                Application.Current.Dispatcher.Invoke(new Action(() => { duResults.Content = despUses; }));

            }
        }

    }
}
