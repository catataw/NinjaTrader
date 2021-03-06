﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AlsiTrade_Backend;
using AlsiUtils;
using AlsiUtils.Data_Objects;
using AlsiUtils.Strategies;
using BrightIdeasSoftware;
using Communicator;
using System.IO;
using System.Reflection;

namespace FrontEnd
{

    public partial class MainForm : Form
    {
        private PrepareForTrade p;
        private UpdateTimer U5;
        private WebUpdate service;
        MarketOrder marketOrder;


        ManualTrade MT = new ManualTrade();
        GlobalObjects.TimeInterval _Interval;
        private Statistics _Stats = new Statistics();
        private OLVColumn ColStamp;
        private OLVColumn ColReason;
        private OLVColumn ColBuySell;
        private OLVColumn ColNotes;
        private OLVColumn ColPosition;
        private OLVColumn ColRunningProf;
        private OLVColumn ColCurrentDirection;
        private OLVColumn ColCurrentPrice;
        private OLVColumn ColTotalProf;
        private OLVColumn ColInstrument;
        private OLVColumn ColTradeVol;

        AlsiTrade_Backend.HiSat.LiveFeed feed;

        DateTime masterStart, masterEnd, allhistoStart, allhistoEnd;

        //GENETIC PLUG IN
       
        private GeneticBackend gb;
        private GeneticAlgoForm gf = new GeneticAlgoForm(); 



        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            WebSettings.GetSettings();
            var start = new LoadingStartupEvent();
            start.Progress = 10;
            onStartupLoad(this, start);

            CheckForIllegalCrossThreadCalls = false;
           //  Debug.WriteLine("Time Synched " + DoStuff.SynchronizeTime());
            _Interval = GlobalObjects.TimeInterval.Minute_5;

            start.Progress = 10;
            onStartupLoad(this, start);
            PopulateControls();

            start.Progress = 20;
            onStartupLoad(this, start);

            U5 = new UpdateTimer(_Interval);
            p = new PrepareForTrade(_Interval, WebSettings.General.HISAT_INST, GetParameters(), WebSettings.General.LIVE_START_DATE);
            marketOrder = new MarketOrder();


            p.onPriceSync += new PrepareForTrade.PricesSynced(p_onPriceSync);
            U5.onStartUpdate += new UpdateTimer.StartUpdate(U5_onStartUpdate);
            U5.OnManualCloseTrigger += new UpdateTimer.ManualCloseTrigger(U5_OnManualCloseTrigger);
            U5.OnConnectionExpired += new UpdateTimer.ConnectionExpired(U5_OnConnectionExpired);
            marketOrder.onOrderSend += new MarketOrder.OrderSend(marketOrder_onOrderSend);
            marketOrder.onOrderMatch += new MarketOrder.OrderMatch(marketOrder_onOrderMatch);
            _Stats.OnStatsCaculated += new Statistics.StatsCalculated(_Stats_OnStatsCaculated);


            start.Progress = 30;
            onStartupLoad(this, start);

            feed = new AlsiTrade_Backend.HiSat.LiveFeed(WebSettings.General.HISAT_INST);

            start.Progress = 60;
            onStartupLoad(this, start);

            BuildListViewColumns();

            start.Progress = 70;
            onStartupLoad(this, start);


            p._LastTrade = DoStuff.GetLastTrade(GetParameters(), _Interval);

            start.Progress = 80;
            onStartupLoad(this, start);


          
            Debug.WriteLine("LAST TRADE : " + p._LastTrade.TimeStamp + "   " + p._LastTrade);
            service = new WebUpdate();


            start.Progress = 100;
            onStartupLoad(this, start);

            WebUpdate.SetManualTradeTrigger(false);


            //test
            comboBox1.Items.Add(Trade.Trigger.None);
            comboBox1.Items.Add(Trade.Trigger.OpenLong);
            comboBox1.Items.Add(Trade.Trigger.OpenShort);
            comboBox1.Items.Add(Trade.Trigger.CloseLong);
            comboBox1.Items.Add(Trade.Trigger.CloseShort);

            comboBox2.Items.Add(Trade.BuySell.None);
            comboBox2.Items.Add(Trade.BuySell.Buy);
            comboBox2.Items.Add(Trade.BuySell.Sell);

           //Genetic plugin
            gb = GeneticBackend.GetSingletonGB();
            gb.Mf = this;
            gb.onUpdate += gb_onUpdate;
            gf.Show();
        }

        void gb_onUpdate(object sender, GeneticBackend.GeneticUpdateEventsArgs e)
        {
            Debug.WriteLine(e.Msg);

            var currentOrderTRADE = e.oTrade;
            var currentOrder = e.FinalTrigger;
            Debug.WriteLine("Open Trade {0}   Direction {1}  Traded Price {2}  Running Prof {3}", currentOrder.Trade.OpenPosition, currentOrder.Trade.TradeDirection, currentOrder.Trade.TradedPrice[0],
              currentOrder.Trade.RunningProfit);
            Debug.WriteLine("Open Trigger {0}  Close Trigger {1}", currentOrder.Trade.TradeTrigger_Open, currentOrder.Trade.TradeTrigger_Close);

            marketOrder.SendOrderToMarketGenetic(e.oTrade);
            if (e.oTrade.BuyorSell != Trade.BuySell.None) 
            gf.WriteText(e.FinalTrigger.Trade.ToString() + " vol: " + e.oTrade.TradeVolume);
            
        }

       
                       
        void U5_OnConnectionExpired(object sender, UpdateTimer.ConnectionExpiredEventArgs e)
        {
            feed = new AlsiTrade_Backend.HiSat.LiveFeed(WebSettings.General.HISAT_INST);
            Debug.WriteLine("RE-CONNECTING TO HI SAT");
        }
        
        void marketOrder_onOrderMatch(object sender, MarketOrder.OrderMatchEvent e)
        {
            EmailMsg msg = new EmailMsg();
            msg.Title = "Order Matched";
            msg.Body = e.Trade.ToString();
            WebUpdate.SendOrder(e.Trade, true);
            SmsMsg sms = new SmsMsg()
            {
                text = "Order Matched",
            };
            DoStuff.SMS.SendSms(e.Trade, sms, true);
            DoStuff.Email.SendEmail(e.Trade, msg, true);
            WebUpdate.SendOrderToWebDB(e.Trade);

        }

        void marketOrder_onOrderSend(object sender, MarketOrder.OrderSendEvent e)
        {
            if (e.Success)
            {
                EmailMsg msg = new EmailMsg();
                msg.Title = "New Trade";
                msg.Body = "an Order was generated and sucessfully send to the Market. \n" + e.Trade.ToString() +
                  "go to " + @"http://www.alsitm.com/Trades.aspx" + " to view trade history\n"
                            + @"http://www.alsitm.com/charts/Statssummary.html" + " to view profit summary chart\n"
                            + @"http://www.alsitm.com/charts/TradeHistory.html" + " to view trade history chart\n"
                            + @"http://www.alsitm.com/charts/TradeDifference.html" + " to view actual trade difference chart";

                SmsMsg sms = new SmsMsg()
                {
                    text="New Order",
                };
                DoStuff.Email.SendEmail(e.Trade, msg, false);
                DoStuff.SMS.SendSms(e.Trade,sms , false);
                WebUpdate.SendOrder(e.Trade, false);
                WebUpdate.SendOrderToWebDB(e.Trade);
            }
            else
            {
                EmailMsg msg = new EmailMsg();
                msg.Title = "Trade input Failed";
                msg.Body = "an Order was generated but could not be send to Excel. \n" + e.Trade.ToString();

                SmsMsg sms = new SmsMsg()
                {
                    text = "Order Submission Failed",
                };                
                DoStuff.SMS.SendSms(e.Trade, sms, true);
                DoStuff.Email.SendEmail(e.Trade, msg, true);
            }
        }

        void U5_onStartUpdate(object sender, UpdateTimer.StartUpDateEvent e)
        {
            Debug.WriteLine(e.Message + "  " + e.Interval);           
            AlsiTrade_Backend.HiSat.LivePrice.EndOfDay = e.EndOfDay;
            p.GetPricesFromTick();

        }

        private static int timeout = 0;
        void p_onPriceSync(object sender, PrepareForTrade.PricesSyncedEvent e)
        {
            Debug.WriteLine("Prices Synced : " + e.ReadyForTradeCalcs);
            if (e.ReadyForTradeCalcs)
            {
                var trades = DoStuff.GetDataFromTick.DoYourThing(WebSettings.General.HISAT_INST, GetParameters(), WebSettings.General.LIVE_START_DATE);
                var NewTrades = AlsiUtils.Strategies.TradeStrategy.Expansion.ApplyRegressionFilter(11, trades);
                NewTrades = _Stats.CalcExpandedTradeStats(NewTrades);
                var Final = CompletedTrade.CreateList(NewTrades);

                foreach (var t in trades.Where(z => z.TimeStamp > DateTime.Now.AddDays(-5)))
                {
                    Debug.WriteLine(t.InstrumentName + "   " + t.TimeStamp + "  reason " + t.Reason + " " + t.TradedPrice + "  " + t.IndicatorNotes + "  " + t.CurrentDirection);
                }
                Debug.WriteLine("##############################################################");
                Debug.WriteLine("##############################################################");
                foreach (var ct in Final.Where(z => z.TimeStamp > DateTime.Now.AddDays(-5)))
                {
                   // Debug.WriteLine(ct.InstrumentName + "  " + ct.TimeStamp + "  reason " + ct.Reason + " " + ct.TradedPrice + "  " + ct.IndicatorNotes + "  " + ct.CurrentDirection + " Vol " + ct.TradeVolume);
                }

                var algotime = DoStuff.GetAlgoTime();
                var check = trades.Any(z => z.TimeStamp.Hour >= algotime.Hour && z.TimeStamp.Minute >= algotime.Minute && z.TimeStamp.Date == algotime.Date);
                if (!check && timeout < 5)
                {
                    timeout++;
                    Debug.WriteLine("Update Failed...Trying Again");
                    p.GetPricesFromTick();
                    return;
                }
                Trade currentOrder;
                if (!AlsiTrade_Backend.HiSat.LivePrice.EndOfDay)
                    currentOrder = trades.Where(z => z.TimeStamp.Hour >= algotime.Hour && z.TimeStamp.Minute >= algotime.Minute && z.TimeStamp.Date == algotime.Date).First();
                else
                    currentOrder = trades.Last();

                //Manual Trade
               
                MT.LastTrade = trades.Where(z => z.TimeStamp <= currentOrder.TimeStamp && z.Reason != Trade.Trigger.None).Last();
                MT.LastTrade.TradedPrice = AlsiTrade_Backend.HiSat.LivePrice.Last;
               
               

                currentOrder.TradeVolume = Final.Last().TradeVolume * WebSettings.General.VOL;
                currentOrder.InstrumentName = WebSettings.General.OTS_INST;
                Debug.WriteLine("Sending " + currentOrder);
                //original model entry
                //marketOrder.SendOrderToMarket(currentOrder);
                //algo order entry

                UpdateTradeLog(SetTradeLogColor(currentOrder), true);
            }
            else
            {
                timeout++;
                if (timeout == 3)
                {
                    timeout = 0;
                    p.GetPricesFromTick();
                }
                else
                {
                    Debug.WriteLine(timeout);
                    System.Threading.Thread.Sleep(1000);
                    p.GetPricesFromWeb();
                }
            }

            var ps = new NotifyOnPriceSynch();
            OnGeneticNotified(this, ps);

        }

        void _Stats_OnStatsCaculated(object sender, Statistics.StatsCalculatedEvent e)
        {
            Debug.WriteLine("============STATS==============");
            Debug.WriteLine("Total PL " + e.SumStats.TotalProfit);
            Debug.WriteLine("# Trades " + e.SumStats.TradeCount);
            Debug.WriteLine("Tot Avg PL " + e.SumStats.Total_Avg_PL);
            Debug.WriteLine("Prof % " + e.SumStats.Pct_Prof);
            Debug.WriteLine("Loss % " + e.SumStats.Pct_Loss);
            PopulateStatBox(e.SumStats);
        }

        public static Parameter_EMA_Scalp GetParameters()
        {
            AlsiUtils.Strategies.Parameter_EMA_Scalp E = new AlsiUtils.Strategies.Parameter_EMA_Scalp()
            {
                A_EMA1 = WebSettings.Indicators.EmaScalp.A1,
                A_EMA2 = WebSettings.Indicators.EmaScalp.A2,
                B_EMA1 = WebSettings.Indicators.EmaScalp.B1,
                B_EMA2 = WebSettings.Indicators.EmaScalp.B2,
                C_EMA = WebSettings.Indicators.EmaScalp.C1,
                TakeProfit = WebSettings.General.TAKE_PROFIT,
                StopLoss = WebSettings.General.STOPLOSS,
                CloseEndofDay = false,
                //Period = GlobalObjects.Prices.Count,


            };

            return E;
        }

        private void PopulateControls()
        {
            emaA1TextBox.Text = WebSettings.Indicators.EmaScalp.A1.ToString();
            emaA2TextBox.Text = WebSettings.Indicators.EmaScalp.A2.ToString();
            emaB1TextBox.Text = WebSettings.Indicators.EmaScalp.B1.ToString();
            emaB2TextBox.Text = WebSettings.Indicators.EmaScalp.B2.ToString();
            emaCTextBox.Text = WebSettings.Indicators.EmaScalp.C1.ToString();

            stopLossTextBox.Text = WebSettings.General.STOPLOSS.ToString();
            takeProfTextBox.Text = WebSettings.General.TAKE_PROFIT.ToString();
            tradeVolTextBox.Text = WebSettings.General.VOL.ToString();
            hiSatInstrumentTextBox.Text = WebSettings.General.HISAT_INST;
            otsInstrumentTextBox.Text = WebSettings.General.OTS_INST;

            runningMinuteTooltripLabel.Text = _Interval.ToString().Replace("_", " ");
            endDateTimePicker.Value = DateTime.Now;
            startDateTimePicker.Value = DateTime.Now.AddDays(-50);
            endDateTimePicker.MaxDate = DateTime.Now;
            liveStartTimePicker.Value = WebSettings.General.LIVE_START_DATE;

            smsCheckBox.Checked = WebSettings.General.ENABLE_SMS;


            //TradeMode
            tradeModeNORMALRadioButton.Checked = (WebSettings.TradeApproach.Mode == WebSettings.TradeApproach.TradeMode.Normal);
            tradeModeHITRadioButton.Checked = (WebSettings.TradeApproach.Mode == WebSettings.TradeApproach.TradeMode.Hit);
            tradeModeAGGRESSIVERadioButton.Checked = (WebSettings.TradeApproach.Mode == WebSettings.TradeApproach.TradeMode.Aggressive);
            tradeModeBESTRadioButton.Checked = (WebSettings.TradeApproach.Mode == WebSettings.TradeApproach.TradeMode.BestBidOffer);
            tradeModeBESTAGGRESSIVERadioButton.Checked = (WebSettings.TradeApproach.Mode == WebSettings.TradeApproach.TradeMode.BestAgressive);
            spreadNumericUpDown.Value = (decimal)WebSettings.TradeApproach.Spread;
        }

        private void PopulateStatBox(SummaryStats Stats)
        {
            statsListView.Items.Clear();

            ListViewItem n = new ListViewItem("Total Profit");
            n.SubItems.Add(Stats.TotalProfit.ToString());
            statsListView.Items.Add(n);

            ListViewItem n1 = new ListViewItem("Trade Count");
            n1.SubItems.Add(Stats.TradeCount.ToString());
            statsListView.Items.Add(n1);

            ListViewItem n2 = new ListViewItem("Total avg Profit");
            n2.SubItems.Add(Stats.Total_Avg_PL.ToString());
            statsListView.Items.Add(n2);

            ListViewItem n3 = new ListViewItem("Win Trades");
            n3.SubItems.Add(Stats.Pct_Prof.ToString());
            statsListView.Items.Add(n3);

            ListViewItem n4 = new ListViewItem("Loss Trades");
            n4.SubItems.Add(Stats.Pct_Loss.ToString());
            statsListView.Items.Add(n4);
        }

        private void BuildListViewColumns()
        {

            ColStamp = new OLVColumn
            {
                Name = "cStamp",
                AspectName = "TimeStamp",
                Text = "Time",
                Groupable = false,
                IsVisible = true,
                Width = 150
            };
            histListview.AllColumns.Add(ColStamp);

            ColReason = new OLVColumn
            {
                Name = "cReason",
                AspectName = "Reason",
                Text = "Reason",
                IsVisible = true,
                Width = 70
            };
            histListview.AllColumns.Add(ColReason);

            ColBuySell = new OLVColumn
            {
                Name = "cBuySell",
                AspectName = "BuyorSell",
                Text = "Buy or Sell",
                IsVisible = true,
                Width = 70
            };
            histListview.AllColumns.Add(ColBuySell);

            ColTradeVol = new OLVColumn
             {
                 Name = "cTradeVolume",
                 AspectName = "TradeVolume",
                 Text = "Volume",
                 IsVisible = true,
                 Width = 70
             };
            histListview.AllColumns.Add(ColTradeVol);



            ColCurrentDirection = new OLVColumn
             {
                 Name = "cDirection",
                 AspectName = "CurrentDirection",
                 Text = "Direction",
                 IsVisible = true,
                 Width = 60
             };
            histListview.AllColumns.Add(ColCurrentDirection);

            ColCurrentPrice = new OLVColumn
            {
                Name = "cPrice",
                AspectName = "CurrentPrice",
                Text = "Price",
                IsVisible = true,
                Width = 50
            };
            histListview.AllColumns.Add(ColCurrentPrice);

            ColPosition = new OLVColumn
            {
                Name = "cPosition",
                ShowTextInHeader = false,
                AspectName = "Position",
                Text = "Position",
                Sortable = false,
                Groupable = false,
                Width = 60
            };
            histListview.AllColumns.Add(ColPosition);

            ColRunningProf = new OLVColumn
            {
                Name = "cRunningProfit",
                AspectName = "RunningProfit",
                Text = "Running Profit",
                IsVisible = true,
                Width = 50
            };
            histListview.AllColumns.Add(ColRunningProf);

            ColTotalProf = new OLVColumn
            {
                Name = "cTotalProf",
                AspectName = "TotalPL",
                Text = "Total Profit",
                IsVisible = true,
                Width = 50
            };
            histListview.AllColumns.Add(ColTotalProf);

            ColInstrument = new OLVColumn
            {
                Name = "cInstrument",
                AspectName = "InstrumentName",
                Text = "Instrument",
                IsVisible = true,
                Width = 80
            };
            histListview.AllColumns.Add(ColInstrument);

            ColNotes = new OLVColumn
            {
                Name = "cNotes",
                AspectName = "IndicatorNotes",
                Text = "Notes",
                IsVisible = true,
                Width = 150
            };
            histListview.AllColumns.Add(ColNotes);






        }

        private void UpdateTradeLog(Trade t, bool WritetoDB)
        {

            string time = WritetoDB ? DateTime.UtcNow.AddHours(2).ToString() : t.TimeStamp.ToString();
            ListViewItem lvi = new ListViewItem(time);
            lvi.SubItems.Add(t.BuyorSell.ToString());
            lvi.SubItems.Add(t.Reason.ToString());
            lvi.SubItems.Add(t.CurrentPrice.ToString());
            lvi.SubItems.Add(t.TradeVolume.ToString());
            lvi.SubItems.Add(t.IndicatorNotes.ToString());
            lvi.ForeColor = t.ForeColor;
            lvi.BackColor = t.BackColor;
            liveTradeListView.Items.Add(lvi);


            if (WritetoDB) DataBase.InsertTradeLog(t);
            var Bid = AlsiTrade_Backend.HiSat.LivePrice.Bid;
            var Offer = AlsiTrade_Backend.HiSat.LivePrice.Offer;
            var Last = AlsiTrade_Backend.HiSat.LivePrice.Last;





        }

        private Trade SetTradeLogColor(Trade t)
        {

            switch (t.Reason)
            {
                case Trade.Trigger.None:
                    t.ForeColor = Color.DarkGray;
                    t.BackColor = Color.White;
                    break;

                case Trade.Trigger.OpenLong:
                    t.ForeColor = Color.Green;
                    t.BackColor = Color.White;
                    break;

                case Trade.Trigger.CloseLong:
                    t.ForeColor = Color.Green;
                    t.BackColor = Color.WhiteSmoke;
                    break;

                case Trade.Trigger.OpenShort:
                    t.ForeColor = Color.Red;
                    t.BackColor = Color.White;
                    break;

                case Trade.Trigger.CloseShort:
                    t.ForeColor = Color.Red;
                    t.BackColor = Color.WhiteSmoke;
                    break;


            }
            return t;
        }

        private void LoadTradeLog(bool todayOnly, bool onlyTrades, int periods)
        {
            AlsiDBDataContext dc = new AlsiDBDataContext();
            liveTradeListView.Items.Clear();

            IQueryable<AlsiUtils.TradeLog> tl = null;
            int count = dc.TradeLogs.Count();
            if (periods > count) return;

            if (todayOnly)
            {
                if (onlyTrades)
                {
                    tl = from x in dc.TradeLogs
                         where x.Time == DateTime.Now.Date
                         where x.BuySell != "None"
                         select x;
                }
                else
                {
                    tl = from x in dc.TradeLogs
                         where x.Time == DateTime.Now.Date
                         select x;
                }
            }
            else
            {
                if (onlyTrades)
                {
                    tl = (from x in dc.TradeLogs
                          select x).Skip(count - periods);

                    tl = tl.Where(z => z.BuySell != "None");
                }
                else
                {
                    tl = (from x in dc.TradeLogs
                          select x).Skip(count - periods);
                }
            }



            liveTradeListView.BeginUpdate();
            Cursor = Cursors.WaitCursor;
            foreach (var v in tl)
            {
                Trade t = new Trade()
                {
                    TimeStamp = v.Time,
                    TradeVolume = (int)v.Volume,
                    BuyorSell = Trade.BuySellFromString(v.BuySell),
                    IndicatorNotes = v.Notes,
                    CurrentPrice = (double)v.Price,
                    ForeColor = Color.FromName(v.ForeColor),
                    BackColor = Color.FromName(v.BackColor),
                    Reason = GetTradeReasonFromString(v.Reason),
                };
                UpdateTradeLog(t, false);
            }
            liveTradeListView.EndUpdate();
            Cursor = Cursors.Default;
        }

        private Trade.Trigger GetTradeReasonFromString(string Reason)
        {
            switch (Reason)
            {
                case "OpenLong":
                    return Trade.Trigger.OpenLong;
                    break;

                case "CloseLong":
                    return Trade.Trigger.CloseLong;
                    break;

                case "OpenShort":
                    return Trade.Trigger.OpenShort;
                    break;

                case "CloseShort":
                    return Trade.Trigger.CloseShort;
                    break;

                default:
                    return Trade.Trigger.None;
            }

        }

        private List<Trade> _FullTradeList = new List<Trade>();
        private List<Trade> _TradeOnlyList = new List<Trade>();
        private void runHistCalcButton_Click(object sender, EventArgs e)
        {
            RunHisroticTrades();
        }

        private bool isListReversed = false;
        private void RunHisroticTrades()
        {
            isListReversed = false;
            Cursor = Cursors.WaitCursor;
            exportToTextButton.Enabled = false;
            synchWebTradesButton.Enabled = false;
            uploadButton.Visible = false;
            drawChartButton.Enabled = false;
            GlobalObjects.TimeInterval t = GlobalObjects.TimeInterval.Minute_1;
            if (_2minRadioButton.Checked) t = GlobalObjects.TimeInterval.Minute_2;
            if (_5minRadioButton.Checked) t = GlobalObjects.TimeInterval.Minute_5;
            if (_10minRadioButton.Checked) t = GlobalObjects.TimeInterval.Minute_10;

            DataBase.dataTable dt;
            if (recentRadioButton.Checked)
            {
                dt = DataBase.dataTable.MasterMinute;
            }
            else
            { dt = DataBase.dataTable.AllHistory; }

            _FullTradeList = AlsiTrade_Backend.RunCalcs.RunEMAScalp(GetParameters(), t, onlyTradesRadioButton.Checked, startDateTimePicker.Value, endDateTimePicker.Value.AddHours(5), dt);
            _FullTradeList = _Stats.CalcBasicTradeStats_old(_FullTradeList);

            var NewTrades = AlsiUtils.Strategies.TradeStrategy.Expansion.ApplyRegressionFilter(11, _FullTradeList);
            NewTrades = _Stats.CalcExpandedTradeStats(NewTrades);
            _TradeOnlyList = CompletedTrade.CreateList(NewTrades);


            var MList = new List<Trade>();
            if (!onlyTradesRadioButton.Checked) MList = _FullTradeList;
            else
                MList = _TradeOnlyList;

            if (!isListReversed)
            {
                _TradeOnlyList.Reverse();
                _FullTradeList.Reverse();
                isListReversed = true;
            }
            histListview.Items.Clear();
            histListview.RebuildColumns();
            histListview.SetObjects(MList);
            Cursor = Cursors.Default;
            dataTabControl.SelectedIndex = 1;
            exportToTextButton.Enabled = true;
            synchWebTradesButton.Enabled = true;
            drawChartButton.Enabled = true;
        }

        private void loadTradeLogButton_Click(object sender, EventArgs e)
        {
            LoadTradeLog(loadTodayRadioButton.Checked, tradelogOnlyTradesCheckBox.Checked, (int)loadTradeLogNumericUpDown.Value);
        }

        private void loadNumberRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (loadNumberRadioButton.Checked) loadTradeLogNumericUpDown.Enabled = true;
            else loadTradeLogNumericUpDown.Enabled = false;
        }

        private void emaA1TextBox_TextChanged(object sender, EventArgs e)
        {
            int em = 0;
            if (int.TryParse(emaA1TextBox.Text, out em))
            {
                Cursor = Cursors.WaitCursor;
                WebSettings.Indicators.EmaScalp.A1 = em;
                Cursor = Cursors.Default;
            }

        }

        private void emaA2TextBox_TextChanged(object sender, EventArgs e)
        {
            int em = 0;
            if (int.TryParse(emaA2TextBox.Text, out em))
            {
                Cursor = Cursors.WaitCursor;
                WebSettings.Indicators.EmaScalp.A2 = em;
                Cursor = Cursors.Default;
            }
        }

        private void emaB1TextBox_TextChanged(object sender, EventArgs e)
        {
            int em = 0;
            if (int.TryParse(emaB1TextBox.Text, out em))
            {
                Cursor = Cursors.WaitCursor;
                WebSettings.Indicators.EmaScalp.B1 = em;
                Cursor = Cursors.Default;
            }
        }

        private void emaB2TextBox_TextChanged(object sender, EventArgs e)
        {
            int em = 0;
            if (int.TryParse(emaB2TextBox.Text, out em))
            {
                Cursor = Cursors.WaitCursor;
                WebSettings.Indicators.EmaScalp.B2 = em;
                Cursor = Cursors.Default;
            }
        }

        private void emaCTextBox_TextChanged(object sender, EventArgs e)
        {
            int em = 0;
            if (int.TryParse(emaCTextBox.Text, out em))
            {
                Cursor = Cursors.WaitCursor;
                WebSettings.Indicators.EmaScalp.C1 = em;
                Cursor = Cursors.Default;
            }
        }

        private void fullHistoUpdateButton_Click(object sender, EventArgs e)
        {
            masterStart = new DateTime();
            Cursor = Cursors.WaitCursor;
            AlsiTrade_Backend.UpdateDB.FullHistoricUpdate_MasterMinute(WebSettings.General.HISAT_INST);
            Cursor = Cursors.Default;
        }

        private void dataTabControl_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPageIndex == 2) settingsTabControl.SelectedIndex = 2;
        }

        private void LoadDBPrices_Click(object sender, EventArgs e)
        {

            Cursor = Cursors.WaitCursor;
            DataBase.dataTable tb = DataBase.dataTable.Temp;
            if (TableMastrMinRadioButton.Checked) tb = DataBase.dataTable.MasterMinute;
            if (TableAllHistMinRadioButton.Checked) tb = DataBase.dataTable.AllHistory;
            ViewDBPrices((int)VieDBNumericUpDown.Value, tb);
            Cursor = Cursors.Default;

        }

        private void ViewDBPrices(int N, DataBase.dataTable Table)
        {
            AlsiDBDataContext dc = new AlsiDBDataContext();

            if (Table == DataBase.dataTable.MasterMinute)
            {
                int count = dc.MasterMinutes.Count();
                DBPricesGridView.DataSource = dc.MasterMinutes.Skip(count - N).OrderByDescending(z => z.Stamp);
            }

            if (Table == DataBase.dataTable.AllHistory)
            {
                int count = dc.AllHistoricMinutes.Count();
                DBPricesGridView.DataSource = dc.AllHistoricMinutes.Skip(count - N).OrderByDescending(z => z.Stamp);
            }

        }

        private void TableAllHistMinRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            AlsiDBDataContext dc = new AlsiDBDataContext();
            int x = dc.AllHistoricMinutes.Count();
            VieDBNumericUpDown.Maximum = x;
            dbPriceCountLabel.Text = "Datapoints : " + x.ToString();
        }

        private void TableMastrMinRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            AlsiDBDataContext dc = new AlsiDBDataContext();
            int x = dc.MasterMinutes.Count();
            VieDBNumericUpDown.Maximum = x;
            dbPriceCountLabel.Text = "Datapoints : " + x.ToString();
        }

        private void settingsTabControl_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPageIndex == 2) dataTabControl.SelectedIndex = 2;
            if (e.TabPageIndex == 3) PopulateEmailTab();
            if (e.TabPageIndex == 4) PopulateSmsTab();
        }

        private void UpdateAllHistoPrices_Click(object sender, EventArgs e)
        {
            masterStart = new DateTime();
            allhistoStart = new DateTime();
            Cursor = Cursors.WaitCursor;
            AlsiDBDataContext dc = new AlsiDBDataContext();
            dc.UpadteAllHistory();
            Cursor = Cursors.Default;
        }

        private void tradeVolTextBox_TextChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            int em = 0;
            if (int.TryParse(tradeVolTextBox.Text, out em))
            {
                WebSettings.General.VOL = em;
            }
            Cursor = Cursors.Default;
        }

        private void takeProfTextBox_TextChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            int em = 0;
            if (int.TryParse(takeProfTextBox.Text, out em))
            {
                WebSettings.General.TAKE_PROFIT = em;
            }
            Cursor = Cursors.Default;
        }

        private void stopLossTextBox_TextChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            int em = 0;
            if (int.TryParse(stopLossTextBox.Text, out em))
            {
                WebSettings.General.STOPLOSS = em;
            }
            Cursor = Cursors.Default;
        }

        private void hiSatInstrumentTextBox_TextChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            WebSettings.General.HISAT_INST = hiSatInstrumentTextBox.Text;
            Cursor = Cursors.Default;
        }

        private void otsInstrumentTextBox_TextChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            WebSettings.General.OTS_INST = otsInstrumentTextBox.Text;
            Cursor = Cursors.Default;
        }

        private void recentRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            if (masterStart.Year == 1)
            {
                AlsiDBDataContext dc = new AlsiDBDataContext();
                masterStart = dc.MasterMinutes.AsEnumerable().First().Stamp;
                masterEnd = dc.MasterMinutes.AsEnumerable().Last().Stamp;
                startDateTimePicker.MinDate = masterStart;
                endDateTimePicker.MaxDate = masterEnd;
                startDateTimePicker.Refresh();
                endDateTimePicker.Refresh();
            }
            if (recentRadioButton.Checked)
            {
                startDateTimePicker.MinDate = masterStart;
                endDateTimePicker.MaxDate = masterEnd;
                if (startDateTimePicker.Value < masterStart) startDateTimePicker.Value = masterStart;

                startDateTimePicker.Refresh();
                endDateTimePicker.Refresh();
            }
            Cursor = Cursors.Default;
        }

        private void AllHistoRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            if (allhistoStart.Year == 1)
            {
                AlsiDBDataContext dc = new AlsiDBDataContext();
                allhistoStart = dc.AllHistoricMinutes.AsEnumerable().First().Stamp;
                allhistoEnd = dc.AllHistoricMinutes.AsEnumerable().Last().Stamp;
                startDateTimePicker.MinDate = allhistoStart;
                endDateTimePicker.MaxDate = allhistoEnd;
                startDateTimePicker.Refresh();
                endDateTimePicker.Refresh();
            }
            if (AllHistoRadioButton.Checked)
            {
                startDateTimePicker.MinDate = allhistoStart;
                endDateTimePicker.MaxDate = allhistoEnd;
                startDateTimePicker.Refresh();
                endDateTimePicker.Refresh();
            }

            Cursor = Cursors.Default;
        }

        private void histListview_CellClick(object sender, CellClickEventArgs e)
        {
            var z = (Trade)histListview.GetSelectedObject();
            Debug.WriteLine(z.Position);
        }

        private void exportToTextButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            DoStuff.ExportToText(_FullTradeList);
            Cursor = Cursors.Default;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ////p.GetPricesFromWeb();
            //p.GetPricesFromTick();
            Debug.WriteLine(Communicator.SMS.GetSmsBalance());
            DoStuff.SMS.SendSms(new Trade(), new SmsMsg { text = "Test SMS" }, false);
        }


        public event LoadingStartup onStartupLoad;
        public delegate void LoadingStartup(object sender, LoadingStartupEvent e);
        public class LoadingStartupEvent : EventArgs
        {
            public int Progress { get; set; }
        }

        private void synchWebTradesButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            var trades = _FullTradeList.Where(z => z.BuyorSell != Trade.BuySell.None).ToList();
            AlsiTrade_Backend.WebUpdate.SyncOnlineDbTradeHistory(trades);
            Cursor = Cursors.Default;
        }

        private AlsiCharts.MultiAxis_3 c = new AlsiCharts.MultiAxis_3();
        private int chartsize = 750;
        private void chartSizeTextBox_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(chartSizeTextBox.Text, out chartsize);
        }

        private void drawChartButton_Click(object sender, EventArgs e)
        {
          
            c.Height = chartsize;
            c.SharedTootltip = true;
            c.XaxisLabels = _TradeOnlyList.Where(z => z.RunningProfit != 0).OrderBy(z => z.TimeStamp).Select(z => z.TimeStamp.ToString()).ToList();

            c.Title = "Alsi Trade System Profit";
            c.Subtitle = _TradeOnlyList.Last().TimeStamp.ToLongDateString() + " - " + _TradeOnlyList.First().TimeStamp.ToLongDateString();

            c.Series_A.Data = _TradeOnlyList.Where(z => z.RunningProfit != 0).OrderBy(z => z.TimeStamp).Select(z => z.CurrentPrice).ToList();
            c.Series_B.Data = _TradeOnlyList.Where(z => z.RunningProfit != 0).OrderBy(z => z.TimeStamp).Select(z => z.RunningProfit).ToList();
            c.Series_C.Data = _TradeOnlyList.Where(z => z.RunningProfit != 0).OrderBy(z => z.TimeStamp).Select(z => z.TotalPL).ToList();


          
            c.Series_A.LineStyle = AlsiCharts.Series.LineStyles.spline;
            c.Series_A.AxisOppositeSide = false;
            c.Series_A.YaxixLabel = "Market Price";
            c.Series_A.YaxisTitleColor = Color.Orange;
            c.Series_A.YaxisUnitColor = Color.Orange;
            c.Series_A.YaxisNumber = 0;


          
            c.Series_B.LineStyle = AlsiCharts.Series.LineStyles.column;
            c.Series_B.YaxisTitleColor = Color.SteelBlue;
            c.Series_B.YaxisUnitColor = Color.SteelBlue;
            c.Series_B.YaxixLabel = "Profit";
            c.Series_B.Unit = "pts";
            c.Series_B.AxisOppositeSide = false;
            c.Series_B.YaxisNumber = 1;


         
            c.Series_C.LineStyle = AlsiCharts.Series.LineStyles.spline;
            c.Series_C.AxisOppositeSide = true;
            c.Series_C.YaxixLabel = "Total Profit";
            c.Series_C.YaxisTitleColor = Color.DarkGreen;
            c.Series_C.YaxisUnitColor = Color.DarkGreen;
            c.Series_C.Unit = "pts";
            c.Series_C.YaxisNumber = 2;





            c.PopulateScript();
            c.OutputFileName = "TradeHistory";
            c.ShowChartInBrowser();
            uploadButton.Visible = true;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
           
            Environment.Exit(0);
        }

        #region Email setup
        private void PopulateEmailTab()
        {
            emailListView.Items.Clear();
            int index = 0;
            foreach (var v in WebUpdate._EmailList)
            {
                int x = ((bool)v.Active) ? 1 : 2;
                ListViewItem lvi = new ListViewItem("", x);
                lvi.SubItems.Add(v.Name);
                lvi.SubItems.Add(v.Email);
                lvi.Tag = v;
                emailListView.Items.Add(lvi);
            }
        }

        private void emailListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            WebUpdate.CheckUncheckEmailListUser(((tblEmail)emailListView.SelectedItems[0].Tag).Email_ID);
            PopulateEmailTab();
            Cursor = Cursors.Default;
        }


        private void addEmailButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            var contat = new tblEmail ()
            {
                Name = name_EmailTextBox.Text.Trim(),
                Email = emailTextBox.Text.Trim(),
            };

            if (WebUpdate.InsertNewUsertoEmailList(contat))
            {
                PopulateEmailTab();
                emailTextBox.Clear();
                name_EmailTextBox.Clear();
                addEmailButton.Enabled = true;
            }
            else
            {
                MessageBox.Show("Email Already Exist");
            }
            Cursor = Cursors.Default;
        }

        private void emailListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.Item != null)
            {
                var u = (tblEmail)e.Item.Tag;
                emailTextBox.Text = u.Email;
                name_EmailTextBox.Text = u.Name;
            }
        }


        private void removeEmailButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            var email = (tblEmail)emailListView.SelectedItems[0].Tag;
            WebUpdate.DeleteUserFromEmailList(email.Email_ID);
            PopulateEmailTab();
            Cursor = Cursors.Default;
        }

        #endregion

        #region SMS Setup
      private void  PopulateSmsTab()
        {
         smsListView.Items.Clear();
            int index = 0;
            foreach (var v in WebUpdate._SMSList)
            {
                int x = ((bool)v.Active) ? 1 : 2;
                ListViewItem lvi = new ListViewItem("", x);
                lvi.SubItems.Add(v.Name);
                lvi.SubItems.Add(v.TelNr);
                lvi.Tag = v;
                smsListView.Items.Add(lvi);
            }
            loadsmsCheckedValue = true;
        }

      private void smsListView_MouseDoubleClick(object sender, MouseEventArgs e)
      {
          Cursor = Cursors.WaitCursor;
          WebUpdate.CheckUncheckSmsListUser(((tblSM)smsListView.SelectedItems[0].Tag).SMS_ID);
          PopulateSmsTab();
          Cursor = Cursors.Default;
      }

      private void smsListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
      {
          if (e.Item != null)
          {
              var u = (tblSM)e.Item.Tag;
              smsTextbox.Text = u.TelNr;
              name_SMSTextBox.Text = u.Name;
          }
      }

      private void removeSMSButton_Click(object sender, EventArgs e)
      {
          Cursor = Cursors.WaitCursor;
          var sms = (tblSM)smsListView.SelectedItems[0].Tag;
          WebUpdate.DeleteUserFromSmsList(sms.SMS_ID);
          PopulateSmsTab();
          Cursor = Cursors.Default;
      }

      private void addSMSButton_Click(object sender, EventArgs e)
      {
          Cursor = Cursors.WaitCursor;

          var contat = new tblSM()
          {
              Name = name_SMSTextBox.Text.Trim(),
              TelNr = smsTextbox.Text.Trim(),
          };

          if (WebUpdate.InsertNewUsertoSmsList(contat))
          {
              PopulateSmsTab();
              smsTextbox.Clear();
              name_SMSTextBox.Clear();
              addSMSButton.Enabled = true;
          }
          else
          {
              MessageBox.Show("Nr Already Exist");
          }
          Cursor = Cursors.Default;
      }

      private bool loadsmsCheckedValue = false;
      private void smsCheckBox_CheckedChanged(object sender, EventArgs e)
      {
          Cursor = Cursors.WaitCursor;
          if (loadsmsCheckedValue) WebSettings.General.ENABLE_SMS = smsCheckBox.Checked;
          Cursor = Cursors.Default;
      }

       

        #endregion
        private void liveStartTimePicker_ValueChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            WebSettings.General.LIVE_START_DATE = liveStartTimePicker.Value;
            Cursor = Cursors.Default;
        }

        private void fullTradesRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            histListview.Items.Clear();
            histListview.RebuildColumns();
            histListview.SetObjects(_FullTradeList);
            Cursor = Cursors.Default;
            dataTabControl.SelectedIndex = 1;
            exportToTextButton.Enabled = true;
            synchWebTradesButton.Enabled = true;
            Cursor = Cursors.Default;
        }

        private void onlyTradesRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            histListview.Items.Clear();
            histListview.RebuildColumns();
            histListview.SetObjects(_TradeOnlyList);
            Cursor = Cursors.Default;
            dataTabControl.SelectedIndex = 1;
            exportToTextButton.Enabled = true;
            synchWebTradesButton.Enabled = true;
            Cursor = Cursors.Default;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var currentOrder = p._LastTrade;
            currentOrder.BuyorSell = (Trade.BuySell)comboBox2.SelectedItem;
            currentOrder.Reason = (Trade.Trigger)comboBox1.SelectedItem;

            marketOrder.SendOrderToMarket(currentOrder);
            UpdateTradeLog(SetTradeLogColor(currentOrder), true);
        }

        private void clearDbButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            clearDbButton.Enabled = false;

            if (clearEmailListCheckBox.Checked)
            {
                clearEmailListCheckBox.ForeColor = Color.Red;
                Refresh();
                WebUpdate.ClearEmailList();
                clearEmailListCheckBox.ForeColor = Color.Green;
                Refresh();
            }

            if (clearTradeLogLocalCheckBox.Checked)
            {
                clearTradeLogLocalCheckBox.ForeColor = Color.Red;
                Refresh();
                UpdateDB.ClearTradeLog();
                clearTradeLogLocalCheckBox.ForeColor = Color.Green;
                Refresh();
            }

            if (clearTradeLogWebCheckBox.Checked)
            {
                clearTradeLogWebCheckBox.ForeColor = Color.Red;
                Refresh();
                WebUpdate.ClearTradeLog();
                clearTradeLogWebCheckBox.ForeColor = Color.Green;
                Refresh();
            }

            if (clearTradeHistoryWebCheckBox.Checked)
            {
                clearTradeHistoryWebCheckBox.ForeColor = Color.Red;
                Refresh();
                WebUpdate.ClearTadeHistory();
                clearTradeHistoryWebCheckBox.ForeColor = Color.Green;
                Refresh();
            }

            if (resetManualCheckBox.Checked)
            {
                resetManualCheckBox.ForeColor = Color.Red;
                Refresh();
                WebSettings.General.MANUAL_CLOSE_TRIGGER = false;
                resetManualCheckBox.ForeColor = Color.Green;
                Refresh();
            }

            clearTradeHistoryWebCheckBox.Checked = false;
            clearTradeLogWebCheckBox.Checked = false;
            clearTradeLogLocalCheckBox.Checked = false;
            clearEmailListCheckBox.Checked = false;
            resetManualCheckBox.Checked = false;

            resetManualCheckBox.ForeColor = Color.Black;
            clearTradeHistoryWebCheckBox.ForeColor = Color.Black;
            clearTradeLogWebCheckBox.ForeColor = Color.Black;
            clearTradeLogLocalCheckBox.ForeColor = Color.Black;
            clearEmailListCheckBox.ForeColor = Color.Black;
            clearDbButton.Enabled = true;

           

            Cursor = Cursors.Default;
        }

        private void tradeModeNORMALRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (tradeModeNORMALRadioButton.Checked)
            {
                Cursor = Cursors.WaitCursor;
                WebSettings.TradeApproach.Mode = WebSettings.TradeApproach.TradeMode.Normal;
                Cursor = Cursors.Default;
            }
            spreadNumericUpDown.Enabled = false;
        }

        private void tradeModeHITRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (tradeModeHITRadioButton.Checked)
            {
                Cursor = Cursors.WaitCursor;
                WebSettings.TradeApproach.Mode = WebSettings.TradeApproach.TradeMode.Hit;
                Cursor = Cursors.Default;
            }
            spreadNumericUpDown.Enabled = false;
        }

        private void tradeModeAGGRESSIVERadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (tradeModeAGGRESSIVERadioButton.Checked)
            {
                Cursor = Cursors.WaitCursor;
                WebSettings.TradeApproach.Mode = WebSettings.TradeApproach.TradeMode.Aggressive;
                spreadNumericUpDown.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void spreadNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            WebSettings.TradeApproach.Spread = (double)spreadNumericUpDown.Value;
            Cursor = Cursors.Default;
        }

        private void saveSettingsButton_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            WebSettings.SaveSettings();
            Cursor = Cursors.Default;
        }

        private void tradeModeBESTRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (tradeModeBESTRadioButton.Checked)
            {
                Cursor = Cursors.WaitCursor;
                WebSettings.TradeApproach.Mode = WebSettings.TradeApproach.TradeMode.BestBidOffer;
                spreadNumericUpDown.Enabled = false;
                Cursor = Cursors.Default;
            }
        }

        private void tradeModeBESTAGGRESSIVERadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (tradeModeBESTAGGRESSIVERadioButton.Checked)
            {
                Cursor = Cursors.WaitCursor;
                WebSettings.TradeApproach.Mode = WebSettings.TradeApproach.TradeMode.BestAgressive;
                spreadNumericUpDown.Enabled = true;
                Cursor = Cursors.Default;
            }
        }
                    
        void U5_OnManualCloseTrigger(object sender, UpdateTimer.ManualCloseTriggerEventArgs e)
        {
            Debug.WriteLine(e.Msg);
            if (WebSettings.General.MANUAL_CLOSE_TRIGGER) return;
            WebUpdate.SetManualTradeTrigger(false);
            WebSettings.General.MANUAL_CLOSE_TRIGGER = true;
            var mantrade = MT.GetCloseTrade();           
            marketOrder.SendOrderToMarketMANUALCLOSE(mantrade);
            UpdateTradeLog(mantrade, true);

        }

        private void uploadButton_Click(object sender, EventArgs e)
        {
            uploadButton.Visible = false;
            c.UploadFile();
           
        }

        private void cnbutton_Click(object sender, EventArgs e)
        {
            //Data Source=ALSI-PC\;Initial Catalog=AlsiTrade;Integrated Security=True
            //Data Source=PIETER-PC\;Initial Catalog=AlsiTrade;Integrated Security=True
            //Data Source=85.214.244.19;Initial Catalog=AlsiTrade;Persist Security Info=True;User ID=Tradebot;Password=boeboe;MultipleActiveResultSets=True

            var path = new FileInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            var file = path + @"\ConnectionString.txt";           
            var Sr = new StreamReader(file);
            Properties.Settings.Default.ConnectionString = Sr.ReadLine();
            Sr.Close();
            Process.Start(file);
        }

       

       //GENETIC ALGO PLUGIN
        public event NotifyGenetic OnGeneticNotified;
        public delegate void NotifyGenetic(object sender, NotifyOnPriceSynch e);
        public class NotifyOnPriceSynch : EventArgs
        {
            
        }

     

       

      


       












    }
}
